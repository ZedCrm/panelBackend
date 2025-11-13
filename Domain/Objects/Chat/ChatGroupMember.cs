// Domain/Objects/Chat/ChatGroupMember.cs
using Domain.Objects.Base;

namespace Domain.Objects.Chat
{
    public class ChatGroupMember : BaseDomain
    {
        public int GroupId { get; set; }
        public ChatGroup Group { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public bool IsAdmin { get; set; } = false;
    }
}