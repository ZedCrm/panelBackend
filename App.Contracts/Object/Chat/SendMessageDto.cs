// App.Contracts/Object/Chat/SendMessageDto.cs
using Microsoft.AspNetCore.Http;
using Domain.Objects.Chat;

namespace App.Contracts.Object.Chat
{
    public class SendMessageDto
    {
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public IFormFile? File { get; set; }
        public MessageType Type { get; set; } = MessageType.Text;
    }




    public class MessageView
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public MessageType Type { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? SeenAt { get; set; }
        public bool IsMine { get; set; } // برای UI
    }

    public class ChatListItem
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
    }
}