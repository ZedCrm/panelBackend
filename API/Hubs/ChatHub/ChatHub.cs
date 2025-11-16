// API/Hubs/ChatHub.cs
using System.Security.Claims;
using App.Contracts.Object.Chat;
using App.Object.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MyFrameWork.AppTool;

namespace API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatApp _chatApp;
        private readonly IHttpContextAccessor _httpContext;

        public ChatHub(IChatApp chatApp, IHttpContextAccessor httpContext)
        {
            _chatApp = chatApp;
            _httpContext = httpContext;
        }

        private int CurrentUserId
        {
            get
            {
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var id))
                    throw new HubException("User not authenticated or invalid user ID.");

                return id;
            }
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{CurrentUserId}");
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = "Failed to connect to chat.", error = ex.Message });
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{CurrentUserId}");
                await base.OnDisconnectedAsync(exception);
            }
            catch
            {
                // ignore
            }
        }

        public async Task SendMessage(SendMessageDto dto)
        {
            try
            {
                var result = await _chatApp.SendMessageAsync(dto, CurrentUserId);

                if (result.IsSucceeded && result.Data != null)
                {
                    var messageView = result.Data;

                    // ارسال به فرستنده (خودش)
                    await Clients.Group($"User_{CurrentUserId}").SendAsync("ReceiveMessage", messageView);

                    // ارسال به گیرنده
                    await Clients.Group($"User_{dto.ReceiverId}").SendAsync("ReceiveMessage", messageView);
                }
                else
                {
                    // خطا فقط به فرستنده
                    await Clients.Caller.SendAsync("SendError", new
                    {
                        message = result.Message ?? "Failed to send message.",
                        errors = result.Errors
                    });
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("SendError", new
                {
                    message = "An error occurred while sending the message.",
                    error = ex.Message
                });
            }
        }

        public async Task OpenChat(int otherUserId)
        {
            try
            {
                var result = await _chatApp.MarkAsReadAsync(otherUserId, CurrentUserId);

                if (result.IsSucceeded)
                {
                    await Clients.Group($"User_{CurrentUserId}").SendAsync("ChatOpened", otherUserId);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = "Failed to open chat.", error = ex.Message });
            }
        }
    }
}