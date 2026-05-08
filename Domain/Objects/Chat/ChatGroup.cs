using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Objects.Base;

namespace Domain.Objects.Chat
{
    [Table("ChatGroups")]
    public class ChatGroup : BaseDomain
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        public int CreatorId { get; set; }

        [ForeignKey(nameof(CreatorId))]
        public User Creator { get; set; } = null!;

        public ICollection<ChatGroupMember> Members { get; set; } = new List<ChatGroupMember>();
        public ICollection<GroupMessage> Messages { get; set; } = new List<GroupMessage>();
    }
}