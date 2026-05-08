// ConfApp/Mapping/Chat/ChatGroupMapping.cs
using Domain.Objects.Chat;
using Microsoft.EntityFrameworkCore;

namespace ConfApp.Mapping.Chat
{
    public class ChatGroupMapping : IEntityTypeConfiguration<ChatGroup>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ChatGroup> builder)
        {

            builder.HasOne(g => g.Creator)
                   .WithMany(u => u.CreatedGroups)
                   .HasForeignKey(g => g.CreatorId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}