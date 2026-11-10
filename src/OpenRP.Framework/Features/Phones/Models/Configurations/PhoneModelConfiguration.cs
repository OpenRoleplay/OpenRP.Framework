using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Phones.Models.Configurations
{
    public class PhoneModelConfiguration : IEntityTypeConfiguration<PhoneModel>
    {
        public void Configure(EntityTypeBuilder<PhoneModel> builder)
        {
            // A number reaches exactly one phone.
            builder.HasIndex(phone => phone.Number).IsUnique();
        }
    }
}
