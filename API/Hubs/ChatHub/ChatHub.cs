using System.Security.Claims;
using App.Contracts.Object.Chat;
using App.Object.Chat;
using App.utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

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
                // اعتبارسنجی اولیه DTO
                var validation = ModelValidator.ValidateToStatusResult(dto);
                if (!validation.IsSuccess)
                {
                    await Clients.Caller.SendAsync("SendError", new
                    {
                        message = "Validation failed",
                        errors = validation.Messages
                    });
                    return;
                }

                var result = await _chatApp.SendMessageAsync(dto, CurrentUserId);

                // بررسی موفقیت با استفاده از IsSuccess
              
                if(!result.IsSuccess)
                {
                    // خطا فقط به فرستنده
                    await Clients.Caller.SendAsync("SendError", new
                    {
                        message = result.Messages?.FirstOrDefault() ?? "Failed to send message.",
                        errors = result.Messages
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

                // استفاده از IsSuccess به جای IsSucceeded
                if (result.IsSuccess)
                {
                    await Clients.Group($"User_{CurrentUserId}").SendAsync("ChatOpened", otherUserId);
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", new 
                    { 
                        message = result.Messages?.FirstOrDefault() ?? "Failed to open chat.",
                        errors = result.Messages 
                    });
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = "Failed to open chat.", error = ex.Message });
            }
        }

        // متد جدید: دریافت تاریخچه چت
        public async Task GetChatHistory(int otherUserId, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var pagination = new Pagination { PageNumber = pageNumber, PageSize = pageSize };
                var result = await _chatApp.GetChatHistoryAsync( otherUserId, pagination ,CurrentUserId);

                if (result.IsSuccess)
                {
                    await Clients.Caller.SendAsync("ChatHistory", new
                    {
                        messages = result.Data,
                        pagination = new
                        {
                            totalRecords = result.TotalRecords,
                            pageNumber = result.PageNumber,
                            pageSize = result.PageSize
                        }
                    });
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", new
                    {
                        message = result.Messages?.FirstOrDefault() ?? "Failed to get chat history.",
                        errors = result.Messages
                    });
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = "Failed to get chat history.", error = ex.Message });
            }
        }

        // متد جدید: تایپ کردن
        public async Task Typing(int receiverId, bool isTyping)
        {
            try
            {
                await Clients.Group($"User_{receiverId}").SendAsync("UserTyping", new
                {
                    userId = CurrentUserId,
                    isTyping = isTyping
                });
            }
            catch (Exception ex)
            {
                // لاگ خطا
                Console.WriteLine($"Error in Typing: {ex.Message}");
            }
        }


    }
}