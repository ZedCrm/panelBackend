using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Objects.Base;

namespace Domain.Objects.Chat
{
    [Table("ChatGroupMembers")]
    public class ChatGroupMember : BaseDomain
    {
        [Required]
        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public ChatGroup Group { get; set; } = null!;

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public bool IsAdmin { get; set; } = false;
    }
}