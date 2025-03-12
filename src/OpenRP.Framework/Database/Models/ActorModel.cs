using OpenRP.Framework.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class ActorModel : BaseModel
    {
        public int Skin { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Angle { get; set; }
        public string AnimLibrary { get; set; }
        public string AnimName { get; set; }
        public float AnimSpeed { get; set; }
        public ulong? ActorCharacterId { get; set; }
        public ulong? ActorPromptId { get; set; }
        public ulong? ActorLinkedToMainMenuSceneId { get; set; }

        // Navigational Properties
        public MainMenuSceneModel? ActorLinkedToMainMenuScene { get; set; }
        public ActorPromptModel? ActorPrompt { get; set; }
        public CharacterModel? ActorCharacter { get; set; }

        // Constructor
        public ActorModel()
        {
            AnimSpeed = 4.1f;
            ActorPrompt = null;
            ActorPromptId = null;
            ActorLinkedToMainMenuScene = null;
            ActorLinkedToMainMenuSceneId = null;
        }
    }
}
