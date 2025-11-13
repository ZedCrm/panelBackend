// Domain/Objects/Chat/Message.cs
using Domain.Objects;
using Domain.Objects.Base;
using System;

namespace Domain.Objects.Chat
{
    public class Message : BaseDomain
    {
        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;

        public int ReceiverId { get; set; }
        public User Receiver { get; set; } = null!;

        public string Content { get; set; } = string.Empty;
        public string? FileUrl { get; set; } // برای عکس/فایل
        public MessageType Type { get; set; } = MessageType.Text; // متن، عکس، فایل

        public DateTime SentAt { get; set; } = DateTime.Now;
        public DateTime? DeliveredAt { get; set; }
        public DateTime? SeenAt { get; set; }

        public bool IsDeletedForSender { get; set; } = false;
        public bool IsDeletedForReceiver { get; set; } = false;
    }

    public enum MessageType
    {
        Text = 1,
        Image = 2,
        File = 3
    }



}




