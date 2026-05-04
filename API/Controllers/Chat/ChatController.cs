// API/Controllers/ChatController.cs
using System.Security.Claims;
using API.Attributes;
using API.utility;
using App.Contracts.Object.Chat;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [RequirePermission("Chat.Access")]
    public class ChatController : ControllerBase
    {
        private readonly IChatApp _chatApp;
        private readonly IHttpContextAccessor _httpContext;

        public ChatController(IChatApp chatApp, IHttpContextAccessor httpContext)
        {
            _chatApp = chatApp;
            _httpContext = httpContext;
        }

        private int CurrentUserId => 
            int.TryParse(_httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) 
            
                ? id 
                : throw new UnauthorizedAccessException("User ID not found in token.");

        [HttpPost("send")]
        public async Task<ActionResult<StatusResult>> SendMessage([FromForm] SendMessageDto dto)
        {
            var result = await _chatApp.SendMessageAsync(dto, CurrentUserId);
            return result;
        }

        [HttpGet("history/{otherUserId}")]
        public async Task<ActionResult<ListDataResult<MessageView>>> GetChatHistory(int otherUserId, [FromQuery] Pagination pagination)
        {
            var result = await _chatApp.GetChatHistoryAsync(otherUserId, pagination, CurrentUserId);
            return result;
        }

        [HttpGet("list")]
        public async Task<ActionResult<ListDataResult<ChatListItem>>> GetChatList()
        {
            var result = await _chatApp.GetChatListAsync(CurrentUserId);
            return result;
        }

        [HttpPost("mark-as-read/{senderId}")]
        public async Task<ActionResult<StatusResult>> MarkAsRead(int senderId)
        {
            var result = await _chatApp.MarkAsReadAsync(senderId, CurrentUserId);
            return result;
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<SingleDataResult<int>>> GetTotalUnreadCount()
        {
            var result = await _chatApp.GetTotalUnreadCountAsync(CurrentUserId);
            return result;
        }
    }
}