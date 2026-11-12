using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Actors.Models.Configurations
{
    public class ActorPermissionModelConfiguration : IEntityTypeConfiguration<ActorPermissionModel>
    {
        public void Configure(EntityTypeBuilder<ActorPermissionModel> builder)
        {
            builder.Property(permission => permission.Permission).HasMaxLength(64).IsRequired();

            // An actor has each permission once.
            builder.HasIndex(permission => new { permission.ActorId, permission.Permission }).IsUnique();

            builder.HasOne(permission => permission.Actor)
                   .WithMany()
                   .HasForeignKey(permission => permission.ActorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
