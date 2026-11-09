using OpenRP.Framework.Database.Models;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using OpenRP.OpenMp.Streamer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Actors.Components
{
    public class ServerActor : Component
    {
        private readonly ActorModel _actorModel;
        public ServerActor(ActorModel actorModel) 
        {
            _actorModel = actorModel;
        }

        public Actor GetActor()
        {
            return this.GetComponentInChildren<Actor>();
        }

        public DynamicTextLabel GetNameTag()
        {
            return this.GetComponentInChildren<DynamicTextLabel>();
        }

        public ulong GetId()
        {
            return _actorModel.Id;
        }

        public string GetName()
        {
            if (_actorModel.ActorCharacter != null)
            {
                return String.Format("{0} {1}", _actorModel.ActorCharacter.FirstName, _actorModel.ActorCharacter.LastName);
            }
            return null;
        }

        public string GetActorPrompt()
        {
            if (_actorModel.ActorPrompt != null)
            {
                return _actorModel.ActorPrompt.Prompt;
            }
            return String.Empty;
        }

        /// <summary>
        /// Plays the actor's looping idle animation from its row.
        /// </summary>
        public void ApplyIdleAnimation()
        {
            GetActor().ApplyAnimation(_actorModel.AnimLibrary, _actorModel.AnimName, _actorModel.AnimSpeed, true, true, true, true, TimeSpan.Zero);
        }

        /// <summary>
        /// Puts the actor back on its spot, facing its angle, in its idle animation. Sent again after the actor streams
        /// in, because the mapping it stands on can reach the player later and the actor would fall through.
        /// </summary>
        public void ResetToSpawnPose()
        {
            Actor actor = GetActor();
            actor.Position = new Vector3(_actorModel.X, _actorModel.Y, _actorModel.Z);
            actor.Angle = _actorModel.Angle;

            // Moving the actor can interrupt its animation.
            ApplyIdleAnimation();
        }

        /// <summary>
        /// Whether the actor has an AI prompt describing who they are.
        /// </summary>
        public bool HasActorPrompt()
        {
            return !String.IsNullOrWhiteSpace(_actorModel.ActorPrompt?.Prompt);
        }

        /// <summary>
        /// Whether the actor belongs to a main menu scene instead of the game world.
        /// </summary>
        public bool IsInMainMenuScene()
        {
            return _actorModel.ActorLinkedToMainMenuSceneId != null;
        }
    }
}
