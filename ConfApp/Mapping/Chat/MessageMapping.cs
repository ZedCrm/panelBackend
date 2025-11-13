// ConfApp/Mapping/Chat/MessageMapping.cs
using Domain.Objects.Chat;
using Microsoft.EntityFrameworkCore;

namespace ConfApp.Mapping.Chat
{
    public class MessageMapping : IEntityTypeConfiguration<Message>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Content).HasMaxLength(2000);
            builder.Property(x => x.FileUrl).HasMaxLength(500);

            builder.HasOne(m => m.Sender)
                   .WithMany(u => u.SentMessages)
                   .HasForeignKey(m => m.SenderId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(m => m.Receiver)
                   .WithMany(u => u.ReceivedMessages)
                   .HasForeignKey(m => m.ReceiverId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(m => new { m.SenderId, m.ReceiverId });
        }
    }
}