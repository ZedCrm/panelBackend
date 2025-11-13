// Domain/Objects/Chat/ChatGroup.cs
using System.Collections.Generic;
using Domain.Objects.Base;

namespace Domain.Objects.Chat
{
    public class ChatGroup : BaseDomain
    {
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; } = null!;

        public ICollection<ChatGroupMember> Members { get; set; } = new List<ChatGroupMember>();
        public ICollection<GroupMessage> Messages { get; set; } = new List<GroupMessage>();
    }
}