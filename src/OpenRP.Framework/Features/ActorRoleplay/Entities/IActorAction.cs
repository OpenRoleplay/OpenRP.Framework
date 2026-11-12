using OpenRP.Framework.Features.Actors.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.ActorRoleplay.Entities
{
    /// <summary>
    /// Something an AI actor can do for a player it talks with, such as opening a bank form. Register one with
    /// AddActorAction. The actor only knows about the actions it has the permission for, and the action itself checks
    /// again before doing anything.
    /// </summary>
    public interface IActorAction
    {
        /// <summary>
        /// The name the actor writes, such as "bank_deposit": lower case, no spaces.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// One line telling the actor when to use it.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// What the actor may add after the player's name, such as "the amount, if they said one", or null for nothing.
        /// </summary>
        string? DetailDescription { get; }

        /// <summary>
        /// The actor permission it needs.
        /// </summary>
        string RequiredPermission { get; }

        /// <summary>
        /// Whether this actor can do it at all right now, beyond the permission, such as standing at its bank.
        /// </summary>
        bool IsAvailableFor(ServerActor actor);

        /// <summary>
        /// Does it for the player. The detail is the actor's free text and must be checked like player input.
        /// </summary>
        void Perform(ServerActor actor, Player player, string? detail);
    }
}
