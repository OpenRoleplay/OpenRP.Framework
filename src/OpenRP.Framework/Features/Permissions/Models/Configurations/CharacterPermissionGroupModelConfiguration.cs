using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Permissions.Models.Configurations
{
    public class CharacterPermissionGroupModelConfiguration : IEntityTypeConfiguration<CharacterPermissionGroupModel>
    {
        public void Configure(EntityTypeBuilder<CharacterPermissionGroupModel> builder)
        {
            // Set primary key
            builder.HasKey(cpg => cpg.Id);

            // Configure relationship to Character
            builder.HasOne(cpg => cpg.Character)
                   .WithMany() // If Character has a collection navigation, specify it here, e.g., .WithMany(c => c.CharacterPermissionGroups)
                   .HasForeignKey(cpg => cpg.CharacterId)
                   .OnDelete(DeleteBehavior.Cascade); // Adjust deletion behavior as needed

            // Configure relationship to PermissionGroupModel
            builder.HasOne(cpg => cpg.PermissionGroup)
                   .WithMany() // If PermissionGroupModel has a collection navigation, specify it here, e.g., .WithMany(pg => pg.CharacterPermissionGroups)
                   .HasForeignKey(cpg => cpg.PermissionGroupId)
                   .OnDelete(DeleteBehavior.Cascade); // Adjust deletion behavior as needed
        }
    }
}
