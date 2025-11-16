// API/Controllers/ChatController.cs
using System.Security.Claims;
using API.Attributes;
using API.utility;
using App.Contracts.Object.Chat;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;


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
        public async Task<IActionResult> SendMessage([FromForm] SendMessageDto dto)
        {
            var result = await _chatApp.SendMessageAsync(dto, CurrentUserId);
            return result.ToActionResult();
        }

        [HttpGet("history/{otherUserId}")]
        public async Task<IActionResult> GetChatHistory(int otherUserId, [FromQuery] Pagination pagination)
        {
            var result = await _chatApp.GetChatHistoryAsync(otherUserId, pagination, CurrentUserId);
            return result.ToActionResult();
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetChatList()
        {
            var result = await _chatApp.GetChatListAsync(CurrentUserId);
            return result.ToActionResult();
        }

        [HttpPost("mark-as-read/{senderId}")]
        public async Task<IActionResult> MarkAsRead(int senderId)
        {
            var result = await _chatApp.MarkAsReadAsync(senderId, CurrentUserId);
            return result.ToActionResult();
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetTotalUnreadCount()
        {
            var result = await _chatApp.GetTotalUnreadCountAsync(CurrentUserId);
            return result.ToActionResult();
        }
    }
}