using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Phones.Models.Configurations
{
    public class PhoneCallLogModelConfiguration : IEntityTypeConfiguration<PhoneCallLogModel>
    {
        public void Configure(EntityTypeBuilder<PhoneCallLogModel> builder)
        {
            builder.HasOne(callLog => callLog.Phone)
                   .WithMany()
                   .HasForeignKey(callLog => callLog.PhoneId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
