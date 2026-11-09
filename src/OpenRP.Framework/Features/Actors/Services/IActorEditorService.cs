using OpenRP.Framework.Features.Actors.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Actors.Services
{
    /// <summary>
    /// Creating, changing and deleting actors, for the admin tools. Changes show in the game right away and are saved
    /// with the next actor sync. Methods that can fail return the reason, or null.
    /// </summary>
    public interface IActorEditorService
    {
        /// <summary>
        /// Every actor admins can edit, by name. Main menu scene actors are left out: they belong to a scene and its camera.
        /// </summary>
        List<ServerActor> GetEditableActors();

        /// <summary>
        /// The editable actor within a few meters of the player, in their virtual world, or null.
        /// </summary>
        ServerActor? GetNearbyActor(Player player);

        /// <summary>
        /// Creates an actor where the player stands, facing their way, with their skin. It's inside the property whose
        /// virtual world the player is in, otherwise outside. The actor is null when the name isn't valid.
        /// </summary>
        (ServerActor? Actor, string? Error) CreateActor(string name, Player player);

        string? RenameActor(ServerActor actor, string name);

        /// <summary>
        /// Moves the actor to where the player stands, facing their way. Which property it's inside is picked again.
        /// </summary>
        void MoveActorToPlayer(ServerActor actor, Player player);

        /// <summary>
        /// Sets the actor's skin from text the admin typed, such as "239".
        /// </summary>
        string? SetActorSkin(ServerActor actor, string skinText);

        /// <summary>
        /// Sets the looping animation; null for both makes the actor just stand.
        /// </summary>
        void SetActorAnimation(ServerActor actor, string? library, string? name);

        /// <summary>
        /// Puts the actor inside the property with this id, in its virtual world, or outside when it's null.
        /// </summary>
        void SetActorInsideProperty(ServerActor actor, ulong? propertyId);

        /// <summary>
        /// Puts the player next to the actor, in its interior and virtual world.
        /// </summary>
        void MovePlayerToActor(Player player, ServerActor actor);

        void DeleteActor(ServerActor actor);

        /// <summary>
        /// Puts every actor inside this property outside, for when the property is deleted.
        /// </summary>
        void MoveActorsOutsideOf(ulong propertyId);
    }
}
