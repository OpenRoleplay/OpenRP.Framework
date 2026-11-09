using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Actors.Models.Configurations
{
    public class ActorModelConfiguration : IEntityTypeConfiguration<ActorModel>
    {
        public void Configure(EntityTypeBuilder<ActorModel> builder)
        {
            // Primary key
            builder.HasKey(e => e.Id);

            // Anim Speed Default Value
            builder.Property(a => a.AnimSpeed)
                  .HasDefaultValue(4.1f);

            builder.Property(a => a.Name)
                  .HasMaxLength(64)
                  .HasDefaultValue(string.Empty);

            // An actor can stand inside a property, in its virtual world. Deleting that property puts it outside.
            builder.HasOne(a => a.InsideProperty)
                  .WithMany()
                  .HasForeignKey(a => a.InsidePropertyId)
                  .OnDelete(DeleteBehavior.SetNull);

            // ActorPromptModel
            builder.Property(e => e.ActorPromptId)
                  .HasColumnName("ActorPromptId");

            builder.HasOne(a => a.ActorPrompt)
                  .WithMany()
                  .HasForeignKey(a => a.ActorPromptId)
                  .OnDelete(DeleteBehavior.Cascade);

            // ActorLinkedToMainMenuScene
            builder.Property(e => e.ActorLinkedToMainMenuSceneId)
                  .HasColumnName("ActorLinkedToMainMenuSceneId");

            builder.HasOne(a => a.ActorLinkedToMainMenuScene)
                  .WithMany()
                  .HasForeignKey(a => a.ActorLinkedToMainMenuSceneId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
