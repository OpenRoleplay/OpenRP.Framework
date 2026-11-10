using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Phones.Models.Configurations
{
    public class PhoneContactModelConfiguration : IEntityTypeConfiguration<PhoneContactModel>
    {
        public void Configure(EntityTypeBuilder<PhoneContactModel> builder)
        {
            builder.Property(contact => contact.Name).HasMaxLength(24).IsRequired();

            // A number is in a phone book once.
            builder.HasIndex(contact => new { contact.PhoneId, contact.Number }).IsUnique();

            builder.HasOne(contact => contact.Phone)
                   .WithMany()
                   .HasForeignKey(contact => contact.PhoneId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
