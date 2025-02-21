using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Characters.Models.Configuration
{
    public class CharacterModelConfiguration : IEntityTypeConfiguration<CharacterModel>
    {
        public void Configure(EntityTypeBuilder<CharacterModel> builder)
        {
            // Table Name
            builder.ToTable("Characters");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(35);

            builder.Property(c => c.MiddleName)
                .HasMaxLength(30)
                .IsRequired(false);

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(35);

            builder.Property(c => c.DateOfBirth)
                .HasColumnType("date");

            builder.Property(c => c.Accent)
                .IsRequired(false)
                .HasMaxLength(30);

            builder.Property(c => c.Skin)
                .IsRequired()
                .HasDefaultValue(26);

            // Relationships

            // One-to-One with Inventory
            builder.HasOne(c => c.Inventory)
                .WithOne()
                .HasForeignKey<CharacterModel>(c => c.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-One with AccountModel
            builder.HasOne(c => c.Account)
                .WithMany(a => a.Characters)
                .HasForeignKey("AccountId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-One with CharacterPreferencesModel
            builder.HasOne(c => c.Preferences)
                .WithOne(cp => cp.Character)
                .HasForeignKey<CharacterPreferencesModel>(cp => cp.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with CharacterSkillModel
            builder.HasMany(c => c.Skills)
                .WithOne(cs => cs.Character)
                .HasForeignKey(cs => cs.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with VehicleModel
            builder.HasMany(c => c.Vehicles)
                .WithOne(v => v.OwnerCharacter)
                .HasForeignKey(v => v.OwnerCharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
