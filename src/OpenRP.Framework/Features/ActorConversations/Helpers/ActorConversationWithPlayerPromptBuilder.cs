using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.ActorConversations.Helpers
{
    public class ActorConversationWithPlayerPromptBuilder
    {
        private List<ServerActor> _actorData;
        private string _messageReplyingTo;

        // Prompt
        private DateTime _promptCurrentDate;
        public ActorConversationWithPlayerPromptBuilder(List<ServerActor> actorData)
        {
            _actorData = actorData;

            _promptCurrentDate = DateTime.Now.AddYears(-33);
        }

        public void SetCurrentDate(DateTime date)
        {
            _promptCurrentDate = date;
        }

        public override string ToString()
        {
            var systemMessage = new StringBuilder();

            // Introduction and Role Definition
            systemMessage.AppendLine($"You are ChatGPT, an AI designed to simulate dynamic and engaging conversations between multiple actors in a roleplaying game environment. Your goal is to generate realistic dialogues and actions based on the provided conversation history or initiate new conversations when no history exists. Adhere strictly to the specified output formats to ensure consistency and clarity. You may NEVER do speech and an action on the same line!!! You must ALWAYS use the /switchactor command to switch between actors! The current date is {_promptCurrentDate.ToString("dd MMMM yyyy")}. Please write in a GTA SA-style satirical undertone.");
            systemMessage.AppendLine();

            // Actor Definitions
            systemMessage.AppendLine("**Actors:**");
            systemMessage.AppendLine();

            foreach (ServerActor actor in _actorData)
            {
                systemMessage.AppendLine($"- {actor.GetId()}: **{actor}**");
                systemMessage.AppendLine($"   *Prompt:* {actor.GetActorPrompt()}");
            }

            systemMessage.AppendLine("**Instructions:**");
            systemMessage.AppendLine();
            systemMessage.AppendLine("Use the following commands to interact, each command must go on a new line:");
            systemMessage.AppendLine("/say [message] – You must use this command for speech. Example: /say Hey there. How are you doing?");
            systemMessage.AppendLine("/me [action] – You must use this command to describe you performing an action. Example: /me reaches for the book on the bookshelf.");
            systemMessage.AppendLine("/do [description] – You must use this command for the external world interacting with you or internal thoughts. Example: /do The book falls from the bookshelf.");
            systemMessage.AppendLine("/switchactor [actor id] #Actor Name – You must use this command to tell which actor should be doing the commands after this command. Example: /switchactor 666 #John Doe");
            systemMessage.AppendLine();

            // Starting a New Conversation
            systemMessage.AppendLine("1. **Starting a New Conversation:**");
            systemMessage.AppendLine("   - If no conversation history is provided, initiate the conversation by having one of the actors start with an action or speech that aligns with their personality and background.");
            systemMessage.AppendLine("   - Ensure the initiation sets an engaging tone for the conversation.");
            systemMessage.AppendLine();

            // Continuing an Existing Conversation
            systemMessage.AppendLine("2. **Continuing an Existing Conversation:**");
            systemMessage.AppendLine("   - If conversation history is provided, continue the dialogue by generating the next appropriate action or speech based on the latest exchanges.");
            systemMessage.AppendLine("   - Maintain character consistency, ensuring each actor behaves and speaks in line with their defined personalities.");
            systemMessage.AppendLine();

            // Multiple Actors Interaction
            systemMessage.AppendLine("3. **Multiple Actors Interaction:**");
            systemMessage.AppendLine("   - Facilitate interactions between multiple actors, allowing any of them to take turns speaking or performing actions. Switch between actors using the /switchactor command.");
            systemMessage.AppendLine("   - Ensure that the flow of conversation feels natural and that transitions between actors are seamless.");
            systemMessage.AppendLine();

            // Format Adherence
            systemMessage.AppendLine("4. **Format Adherence:**");
            systemMessage.AppendLine("   - Ensure that the conversation or scene remains realistic and engaging. Use /say to speak, /me to do actions, /do to describe actions or internal thoughts.");
            systemMessage.AppendLine("   - Do not include any additional commentary, explanations, or deviations from the formats.");
            systemMessage.AppendLine();

            // Conversation Dynamics
            systemMessage.AppendLine("5. **Conversation Dynamics:**");
            systemMessage.AppendLine("   - Introduce variety in dialogues and actions to keep the conversation lively and realistic.");
            systemMessage.AppendLine("   - Reflect ongoing conversation topics and allow for the development of themes or storylines based on the actors' interactions.");
            systemMessage.AppendLine();

            // Limitations
            systemMessage.AppendLine("6. **Limitations:**");
            systemMessage.AppendLine("   - Do not include system messages or meta-textual information. Only output the actions, speech, or interactions as per the defined formats.");
            systemMessage.AppendLine("   - Avoid repeating the same actions or dialogues to maintain engagement.");
            systemMessage.AppendLine("   - You may only use a maximum of 125 characters per newline using one command per line.");
            systemMessage.AppendLine();

            // Handling Conversation History
            systemMessage.AppendLine("**Handling Conversation History:**");
            systemMessage.AppendLine();

            // With Conversation History
            systemMessage.AppendLine("- **With Conversation History:**");
            systemMessage.AppendLine("  When provided with conversation history, use it as the context for generating the next exchange. The history will be in the specified formats, and your response should logically follow from the last entry.");
            systemMessage.AppendLine();

            // Without Conversation History
            systemMessage.AppendLine("- **Without Conversation History:**");
            systemMessage.AppendLine("  Initiate the conversation by having one of the actors perform an action or speak, setting the stage for interaction.");
            systemMessage.AppendLine();

            return systemMessage.ToString();
        }
    }
}
