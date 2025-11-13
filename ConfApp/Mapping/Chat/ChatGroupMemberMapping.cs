// ConfApp/Mapping/Chat/ChatGroupMemberMapping.cs
using Domain.Objects.Chat;
using Microsoft.EntityFrameworkCore;

namespace ConfApp.Mapping.Chat
{
    public class ChatGroupMemberMapping : IEntityTypeConfiguration<ChatGroupMember>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ChatGroupMember> builder)
        {
            builder.ToTable("ChatGroupMembers");
            builder.HasKey(x => x.Id);

            builder.HasOne(m => m.Group)
                   .WithMany(g => g.Members)
                   .HasForeignKey(m => m.GroupId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(m => m.User)
                   .WithMany(u => u.GroupMemberships)
                   .HasForeignKey(m => m.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(m => new { m.GroupId, m.UserId }).IsUnique();
        }
    }
}