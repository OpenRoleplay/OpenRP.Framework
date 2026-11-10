using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Phones.Models.Configurations
{
    public class PhoneMessageModelConfiguration : IEntityTypeConfiguration<PhoneMessageModel>
    {
        public void Configure(EntityTypeBuilder<PhoneMessageModel> builder)
        {
            // 160 characters, the length of an SMS in 1992.
            builder.Property(message => message.Text).HasMaxLength(160).IsRequired();

            builder.HasOne(message => message.Phone)
                   .WithMany()
                   .HasForeignKey(message => message.PhoneId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
