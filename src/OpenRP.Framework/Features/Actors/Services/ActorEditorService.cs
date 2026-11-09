using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.VirtualWorlds.Services;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Actors.Services
{
    public class ActorEditorService : IActorEditorService
    {
        private const int NameMaxLength = 64;
        private const float NearbyRange = 3.0f;
        private const int MinimumSkin = 0;
        private const int MaximumSkin = 311;
        // Skin 74 doesn't exist in GTA San Andreas; using it crashes clients.
        private const int MissingSkin = 74;
        // "Go to" puts the admin this far from the actor, so they don't stand inside it.
        private const float GoToDistance = 1.0f;

        private readonly IActorManager _actorManager;
        private readonly IActorService _actorService;
        private readonly IVirtualWorldManager _virtualWorldManager;

        public ActorEditorService(IActorManager actorManager, IActorService actorService, IVirtualWorldManager virtualWorldManager)
        {
            _actorManager = actorManager;
            _actorService = actorService;
            _virtualWorldManager = virtualWorldManager;
        }

        public List<ServerActor> GetEditableActors()
        {
            return _actorService.GetServerActors()
                .Where(actor => !actor.IsInMainMenuScene())
                .OrderBy(actor => actor.GetName(), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public ServerActor? GetNearbyActor(Player player)
        {
            return GetEditableActors()
                .Where(actor => _actorService.GetActorVirtualWorld(actor) == player.VirtualWorld)
                .Where(actor => player.IsInRangeOfPoint(NearbyRange, actor.GetPosition()))
                .OrderBy(actor => Vector3.Distance(player.Position, actor.GetPosition()))
                .FirstOrDefault();
        }

        public (ServerActor? Actor, string? Error) CreateActor(string name, Player player)
        {
            string? error = GetNameError(name);
            if (error != null)
            {
                return (null, error);
            }

            ActorModel actorModel = new ActorModel
            {
                Name = name.Trim(),
                Skin = player.Skin,
                X = player.Position.X,
                Y = player.Position.Y,
                Z = player.Position.Z,
                Angle = player.Angle,
                Interior = player.Interior,
                InsidePropertyId = GetPropertyIdOfPlayerVirtualWorld(player)
            };
            return (_actorManager.CreateAndLoad(actorModel), null);
        }

        public string? RenameActor(ServerActor actor, string name)
        {
            string? error = GetNameError(name);
            if (error != null)
            {
                return error;
            }

            actor.SetName(name.Trim());
            _actorService.CreateActorInWorld(actor);
            return null;
        }

        public void MoveActorToPlayer(ServerActor actor, Player player)
        {
            actor.SetPosition(player.Position);
            actor.SetAngle(player.Angle);
            actor.SetInterior(player.Interior);
            actor.SetInsidePropertyId(GetPropertyIdOfPlayerVirtualWorld(player));
            _actorService.CreateActorInWorld(actor);
        }

        public string? SetActorSkin(ServerActor actor, string skinText)
        {
            if (!int.TryParse(skinText.Trim(), out int skin) || skin < MinimumSkin || skin > MaximumSkin || skin == MissingSkin)
            {
                return $"The skin must be a number from {MinimumSkin} to {MaximumSkin}, except {MissingSkin}.";
            }

            actor.SetSkin(skin);
            _actorService.CreateActorInWorld(actor);
            return null;
        }

        public void SetActorAnimation(ServerActor actor, string? library, string? name)
        {
            actor.SetAnimation(library?.Trim(), name?.Trim());
            actor.ApplyIdleAnimation();
        }

        public void SetActorInsideProperty(ServerActor actor, ulong? propertyId)
        {
            actor.SetInsidePropertyId(propertyId);
            _actorService.CreateActorInWorld(actor);
        }

        public void MovePlayerToActor(Player player, ServerActor actor)
        {
            player.Interior = actor.GetInterior();
            player.VirtualWorld = _actorService.GetActorVirtualWorld(actor);
            player.Position = GetPointInFrontOf(actor);
            player.Angle = actor.GetAngle() + 180.0f;
        }

        public void DeleteActor(ServerActor actor)
        {
            _actorService.DestroyActorInWorld(actor);
            actor.ProcessDeletion();
        }

        public void MoveActorsOutsideOf(ulong propertyId)
        {
            foreach (ServerActor actor in _actorService.GetServerActors().Where(actor => actor.GetInsidePropertyId() == propertyId).ToList())
            {
                SetActorInsideProperty(actor, null);
            }
        }

        /// <summary>
        /// The property whose virtual world the player is in, or null when they're outside or somewhere else.
        /// </summary>
        private ulong? GetPropertyIdOfPlayerVirtualWorld(Player player)
        {
            return _virtualWorldManager.GetPropertyIdOfVirtualWorld(player.VirtualWorld);
        }

        private static Vector3 GetPointInFrontOf(ServerActor actor)
        {
            float angleInRadians = -actor.GetAngle() * MathF.PI / 180.0f;
            Vector3 position = actor.GetPosition();
            return new Vector3(position.X + GoToDistance * MathF.Sin(angleInRadians), position.Y + GoToDistance * MathF.Cos(angleInRadians), position.Z);
        }

        private static string? GetNameError(string name)
        {
            string trimmedName = name.Trim();
            if (trimmedName.Length == 0)
                return "The actor's name can't be empty.";
            if (trimmedName.Length > NameMaxLength)
                return $"The actor's name can be at most {NameMaxLength} characters long.";
            return null;
        }
    }
}
