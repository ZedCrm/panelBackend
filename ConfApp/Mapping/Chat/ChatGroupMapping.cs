// ConfApp/Mapping/Chat/ChatGroupMapping.cs
using Domain.Objects.Chat;
using Microsoft.EntityFrameworkCore;

namespace ConfApp.Mapping.Chat
{
    public class ChatGroupMapping : IEntityTypeConfiguration<ChatGroup>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ChatGroup> builder)
        {
            builder.ToTable("ChatGroups");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ImageUrl).HasMaxLength(500);

            builder.HasOne(g => g.Creator)
                   .WithMany(u => u.CreatedGroups)
                   .HasForeignKey(g => g.CreatorId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}