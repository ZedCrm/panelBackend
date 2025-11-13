// API/Hubs/ChatHub.cs
using App.Contracts.Object.Chat;
using App.Object.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

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

        private int CurrentUserId => 
            int.TryParse(Context.User?.FindFirst("userId")?.Value, out var id) 
                ? id 
                : throw new UnauthorizedAccessException("User not authenticated.");

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{CurrentUserId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{CurrentUserId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(SendMessageDto dto)
        {
            var result = await _chatApp.SendMessageAsync(dto, CurrentUserId);
            if (result.IsSuccess && result.Data != null)
            {
                var messageView = result.Data;

                await Clients.Group($"User_{CurrentUserId}").SendAsync("ReceiveMessage", messageView);
                await Clients.Group($"User_{dto.ReceiverId}").SendAsync("ReceiveMessage", messageView);
            }
        }

        public async Task OpenChat(int otherUserId)
        {
            await _chatApp.MarkAsReadAsync(otherUserId, CurrentUserId);
            await Clients.Group($"User_{CurrentUserId}").SendAsync("ChatOpened", otherUserId);
        }
    }
}