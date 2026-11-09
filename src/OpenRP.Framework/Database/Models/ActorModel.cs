using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    public class ActorModel : BaseModel
    {
        public string Name { get; set; }
        public int Skin { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Angle { get; set; }
        // The GTA interior the actor stands in, so "go to" and its name label use the right one; 0 is the open world.
        public int Interior { get; set; }
        // Null when the actor just stands, without a looping animation.
        public string? AnimLibrary { get; set; }
        public string? AnimName { get; set; }
        public float AnimSpeed { get; set; }
        // Who the actor is and how it talks, for AI actors; null for actors that don't talk.
        public string? Prompt { get; set; }
        // The property whose virtual world the actor stands in; null when it's outside, in the shared world.
        public ulong? InsidePropertyId { get; set; }
        public ulong? ActorLinkedToMainMenuSceneId { get; set; }

        // No longer read: the name and prompt moved into Name and Prompt. Dropped in a later migration.
        public ulong? ActorCharacterId { get; set; }
        public ulong? ActorPromptId { get; set; }

        // Navigational Properties
        public MainMenuSceneModel? ActorLinkedToMainMenuScene { get; set; }
        public PropertyModel? InsideProperty { get; set; }
        public ActorPromptModel? ActorPrompt { get; set; }
        public CharacterModel? ActorCharacter { get; set; }

        // Constructor
        public ActorModel()
        {
            Name = string.Empty;
            AnimSpeed = 4.1f;
            ActorPrompt = null;
            ActorPromptId = null;
            ActorLinkedToMainMenuScene = null;
            ActorLinkedToMainMenuSceneId = null;
        }
    }
}
