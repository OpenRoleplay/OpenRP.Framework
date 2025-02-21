using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.AccountSettingsFeature.Models.Configuration
{
    public class AccountSettingsModelConfiguration : IEntityTypeConfiguration<AccountSettingsModel>
    {
        public void Configure(EntityTypeBuilder<AccountSettingsModel> builder)
        {
            // Map the model to the "AccountSettings" table.
            builder.ToTable("AccountSettings");

            // Configure the primary key.
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                   .ValueGeneratedOnAdd();

            // Configure the AccountId property (foreign key).
            builder.Property(a => a.AccountId)
                   .IsRequired();

            // Configure the AccountGraphicPreset property.
            // This property is an enum; by default, EF Core stores it as an int.
            builder.Property(a => a.AccountGraphicPreset)
                   .IsRequired();

            // Configure the GlobalChatEnabled property.
            builder.Property(a => a.GlobalChatEnabled)
                   .HasDefaultValue(1)
                   .IsRequired();

            // Configure the one-to-one relationship with AccountModel.
            // Adjust the navigation on the AccountModel side if needed.
            builder.HasOne(a => a.AccountModel)
                   .WithOne() // If AccountModel has a navigation property (e.g. .AccountSettings), use .WithOne(m => m.AccountSettings)
                   .HasForeignKey<AccountSettingsModel>(a => a.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
