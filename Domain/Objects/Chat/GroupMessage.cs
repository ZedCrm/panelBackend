// Domain/Objects/Chat/GroupMessage.cs
using System;
using Domain.Objects.Base;

namespace Domain.Objects.Chat
{
    public class GroupMessage : BaseDomain
    {
        public int GroupId { get; set; }
        public ChatGroup Group { get; set; } = null!;

        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;

        public string Content { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public MessageType Type { get; set; } = MessageType.Text;

        public DateTime SentAt { get; set; } = DateTime.Now;


    }
}