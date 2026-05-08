using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Objects.Base;

namespace Domain.Objects.Chat
{
    [Table("Messages")]
    public class Message : BaseDomain
    {
        [Required]
        public int SenderId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public User Sender { get; set; } = null!;

        [Required]
        public int ReceiverId { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public User Receiver { get; set; } = null!;

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? FileUrl { get; set; }

        public MessageType Type { get; set; } = MessageType.Text;

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