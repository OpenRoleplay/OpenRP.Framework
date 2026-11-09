using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using OpenRP.OpenMp.Streamer;

namespace OpenRP.Framework.Features.Actors.Components
{
    /// <summary>
    /// An actor from the Actors table: its game actor and name label are child entities. Changes are saved by the
    /// ActorManager on its next sync.
    /// </summary>
    public class ServerActor : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly ActorModel _actorModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public ServerActor(ActorModel actorModel)
        {
            _actorModel = actorModel;
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        public bool IsDeleted()
        {
            return _isDeleted;
        }

        public void ProcessDeletion(bool processDeletion = true)
        {
            _isDeleted = processDeletion;
        }

        public ActorModel GetRawActorModel()
        {
            return _actorModel;
        }

        /// <summary>
        /// The game actor, or null when it isn't created (yet).
        /// </summary>
        public Actor? GetActor()
        {
            return GetComponentInChildren<Actor>();
        }

        public ulong GetId()
        {
            return _actorModel.Id;
        }

        public string GetName()
        {
            return _actorModel.Name;
        }

        public void SetName(string name)
        {
            _actorModel.Name = name;
            ProcessChanges();
        }

        public int GetSkin()
        {
            return _actorModel.Skin;
        }

        public void SetSkin(int skin)
        {
            _actorModel.Skin = skin;
            ProcessChanges();
        }

        public Vector3 GetPosition()
        {
            return new Vector3(_actorModel.X, _actorModel.Y, _actorModel.Z);
        }

        public void SetPosition(Vector3 position)
        {
            _actorModel.X = position.X;
            _actorModel.Y = position.Y;
            _actorModel.Z = position.Z;
            ProcessChanges();
        }

        public float GetAngle()
        {
            return _actorModel.Angle;
        }

        public int GetInterior()
        {
            return _actorModel.Interior;
        }

        public void SetInterior(int interior)
        {
            _actorModel.Interior = interior;
            ProcessChanges();
        }

        public void SetAngle(float angle)
        {
            _actorModel.Angle = angle;
            ProcessChanges();
        }

        public bool HasAnimation()
        {
            return !String.IsNullOrWhiteSpace(_actorModel.AnimLibrary) && !String.IsNullOrWhiteSpace(_actorModel.AnimName);
        }

        /// <summary>
        /// The animation as "LIBRARY/name", or null when the actor just stands.
        /// </summary>
        public string? GetAnimationText()
        {
            return HasAnimation() ? $"{_actorModel.AnimLibrary}/{_actorModel.AnimName}" : null;
        }

        /// <summary>
        /// Sets the looping animation; null for both makes the actor just stand.
        /// </summary>
        public void SetAnimation(string? library, string? name)
        {
            _actorModel.AnimLibrary = library;
            _actorModel.AnimName = name;
            ProcessChanges();
        }

        public string GetActorPrompt()
        {
            return _actorModel.Prompt ?? String.Empty;
        }

        /// <summary>
        /// Whether the actor has an AI prompt describing who they are.
        /// </summary>
        public bool HasActorPrompt()
        {
            return !String.IsNullOrWhiteSpace(_actorModel.Prompt);
        }

        /// <summary>
        /// The id of the property whose virtual world the actor stands in, or null when it's outside.
        /// </summary>
        public ulong? GetInsidePropertyId()
        {
            return _actorModel.InsidePropertyId;
        }

        public void SetInsidePropertyId(ulong? insidePropertyId)
        {
            _actorModel.InsidePropertyId = insidePropertyId;
            ProcessChanges();
        }

        /// <summary>
        /// Whether the actor belongs to a main menu scene instead of the game world.
        /// </summary>
        public bool IsInMainMenuScene()
        {
            return _actorModel.ActorLinkedToMainMenuSceneId != null;
        }

        public ulong? GetMainMenuSceneId()
        {
            return _actorModel.ActorLinkedToMainMenuSceneId;
        }

        /// <summary>
        /// Plays the actor's looping idle animation, or stops any animation when it has none.
        /// </summary>
        public void ApplyIdleAnimation()
        {
            Actor? actor = GetActor();
            if (actor == null)
            {
                return;
            }

            if (HasAnimation())
            {
                actor.ApplyAnimation(_actorModel.AnimLibrary!, _actorModel.AnimName!, _actorModel.AnimSpeed, true, true, true, true, TimeSpan.Zero);
            }
            else
            {
                actor.ClearAnimations();
            }
        }

        /// <summary>
        /// Puts the actor back on its spot, facing its angle, in its idle animation. Sent again after the actor streams
        /// in, because the mapping it stands on can reach the player later and the actor would fall through.
        /// </summary>
        public void ResetToSpawnPose()
        {
            Actor? actor = GetActor();
            if (actor == null)
            {
                return;
            }

            actor.Position = GetPosition();
            actor.Angle = GetAngle();

            // Moving the actor can interrupt its animation.
            ApplyIdleAnimation();
        }

        /// <summary>
        /// The actor's name label, or null when it has none.
        /// </summary>
        public DynamicTextLabel? GetNameTag()
        {
            return GetComponentInChildren<DynamicTextLabel>();
        }
    }
}
