using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Objects.Base;

namespace Domain.Objects.Chat
{
    [Table("GroupMessages")]
    public class GroupMessage : BaseDomain
    {
        [Required]
        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public ChatGroup Group { get; set; } = null!;

        [Required]
        public int SenderId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public User Sender { get; set; } = null!;

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? FileUrl { get; set; }

        public MessageType Type { get; set; } = MessageType.Text;

        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}