// ConfApp/Mapping/Chat/GroupMessageMapping.cs
using Domain.Objects.Chat;
using Microsoft.EntityFrameworkCore;

namespace ConfApp.Mapping.Chat
{
    public class GroupMessageMapping : IEntityTypeConfiguration<GroupMessage>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<GroupMessage> builder)
        {
            builder.ToTable("GroupMessages");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Content).HasMaxLength(2000);
            builder.Property(x => x.FileUrl).HasMaxLength(500);

            builder.HasOne(m => m.Group)
                   .WithMany(g => g.Messages)
                   .HasForeignKey(m => m.GroupId);

            builder.HasOne(m => m.Sender)
                   .WithMany(u => u.GroupMessages)
                   .HasForeignKey(m => m.SenderId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}