using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Vehicles.Models.Configurations
{
    public class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
    {
        public void Configure(EntityTypeBuilder<VehicleModel> builder)
        {
            // Set the table name
            builder.ToTable("Vehicles");

            // Configure the primary key
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id)
                   .ValueGeneratedOnAdd();

            // Optional properties
            builder.HasOne(x => x.Model)
                   .WithMany()  // Adjust the collection property name as necessary
                   .HasForeignKey(x => x.ModelId)
                   .IsRequired(false) // Because ModelId is nullable.
                   .OnDelete(DeleteBehavior.SetNull); // Or another behavior as required.
            builder.HasOne(x => x.ModelSpawnType)
                   .WithMany()  // Adjust the collection property name as necessary
                   .HasForeignKey(x => x.ModelSpawnTypeId)
                   .IsRequired(false) // Because ModelId is nullable.
                   .OnDelete(DeleteBehavior.SetNull); // Or another behavior as required.
            builder.Property(v => v.LoadedAs)
                   .IsRequired(false);

            // Coordinates and rotation (assumed required)
            builder.Property(v => v.X)
                   .IsRequired();
            builder.Property(v => v.Y)
                   .IsRequired();
            builder.Property(v => v.Z)
                   .IsRequired();
            builder.Property(v => v.Rotation)
                   .IsRequired();

            // Colors (required)
            builder.Property(v => v.Color1)
                   .IsRequired();
            builder.Property(v => v.Color2)
                   .IsRequired();

            // Boolean properties (required)
            builder.Property(v => v.IsEngineOn)
                   .IsRequired();
            builder.Property(v => v.AreLightsOn)
                   .IsRequired();
            builder.Property(v => v.IsAlarmOn)
                   .IsRequired();
            builder.Property(v => v.AreDoorsLocked)
                   .IsRequired();
            builder.Property(v => v.IsBonnetOpen)
                   .IsRequired();
            builder.Property(v => v.IsBootOpen)
                   .IsRequired();
            builder.Property(v => v.IsDriverDoorOpen)
                   .IsRequired();
            builder.Property(v => v.IsPassengerDoorOpen)
                   .IsRequired();
            builder.Property(v => v.IsBackLeftDoorOpen)
                   .IsRequired();
            builder.Property(v => v.IsBackRightDoorOpen)
                   .IsRequired();
            builder.Property(v => v.IsDriverWindowClosed)
                   .IsRequired();
            builder.Property(v => v.IsPassengerWindowClosed)
                   .IsRequired();
            builder.Property(v => v.IsBackLeftWindowClosed)
                   .IsRequired();
            builder.Property(v => v.IsBackRightWindowClosed)
                   .IsRequired();

            // Health properties (required)
            builder.Property(v => v.CurrentHealth)
                   .IsRequired();
            builder.Property(v => v.MaxHealth)
                   .IsRequired();

            // Server Vehicle
            builder.Property(v => v.ServerVehicle)
                   .IsRequired()
                   .HasDefaultValue(true);

            // Configure the optional relationship to OwnerCharacter
            builder.HasOne(v => v.OwnerCharacter)
                   .WithMany(v => v.Vehicles) // adjust if CharacterModel has a collection property
                   .HasForeignKey(v => v.OwnerCharacterId)
                   .OnDelete(DeleteBehavior.SetNull); // or choose DeleteBehavior.Restrict
        }
    }
}
