using OpenRP.Framework.Features.ActorRoleplay.Entities;
using System.Text;

namespace OpenRP.Framework.Features.ActorRoleplay.Helpers
{
    /// <summary>
    /// Builds the system prompt for an AI actor. The shared rules come first and are the same for every actor, so
    /// OpenAI's prompt caching can reuse them; the actor's own description comes after.
    /// </summary>
    public static class ActorRoleplayPromptBuilder
    {
        public const int MaxReplyLines = 3;
        public const int MaxActions = 1;

        private static readonly string SharedRules =
            $"""
            You are a character on a heavy text-based roleplay server set in GTA San Andreas. The people around you
            roleplay with you in chat, and you react to them in character, as a real person standing in that spot would.

            ### What you see
            The user messages are what happened around you, exactly as it appeared in chat:
            - Name says: text            (someone speaking)
            - Name [Irish accent] says: text   (someone speaking with an accent)
            - * Name does something *    (someone's action, /me)
            - Something describes the scene (( Name ))   (a description of the scene, /do)
            Names are shown so you can tell people apart, but you only know someone's name if they introduced
            themselves or you already knew them.

            ### How you reply
            Reply with 1 to {MaxReplyLines} lines, each on its own line and each starting with one of:
            - /say text   (what you say out loud; speech only, no actions)
            - /me text    (your action in third person without your name, for example "/me scratches his beard.")
            - /do text    (a short description of something about you or your surroundings)
            Plain text only: no quotes around speech, no markdown, no names in front of lines.
            Keep each line short, like a real player typing in chat. Not every reply needs an action.

            ### Rules
            - Stay in character at all times, and never mention that you are an AI.
            - React to what just happened; don't repeat yourself and don't answer things that were already answered.
            - Only refer to things that really exist in the game world. Don't invent people, places or mechanics.
            - Never explain roleplay commands or rules to players.
            - If nothing calls for a reaction, a single short /me is enough.
            """;

        /// <summary>
        /// The system prompt for the actor with this name and description, optionally the situation it's in, and the
        /// actions it may perform. Everything after the shared rules is the same for every request of this actor, so it
        /// stays cached too.
        /// </summary>
        public static string Build(string actorName, string actorPrompt, string? situation = null, IReadOnlyList<IActorAction>? actions = null)
        {
            string prompt = $"""
                {SharedRules}

                ### Who you are
                Your name is {actorName}.
                {actorPrompt}
                """;

            if (!String.IsNullOrWhiteSpace(situation))
            {
                prompt = $"{prompt}\n\n### The situation\n{situation}";
            }
            return actions == null || actions.Count == 0 ? prompt : $"{prompt}\n\n{BuildActions(actions)}";
        }

        /// <summary>
        /// The actions the actor may perform, with the line that performs one.
        /// </summary>
        private static string BuildActions(IReadOnlyList<IActorAction> actions)
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("### What you can do");
            text.AppendLine("Besides talking, you can do these things for the person in front of you, by adding one extra line:");
            text.AppendLine("/action <name> <their name exactly as it appears in chat> <detail, if any>");
            foreach (IActorAction action in actions)
            {
                string detail = action.DetailDescription == null ? string.Empty : $" Detail: {action.DetailDescription}.";
                text.AppendLine($"- {action.Name}: {action.Description}{detail}");
            }
            text.AppendLine("Only do something when they clearly ask for it, at most one /action per reply, and still say something.");
            text.AppendLine("Each action opens a form for them, so don't ask for numbers or amounts yourself.");
            text.Append("If they ask for something that isn't listed, politely say you can't help with that here.");
            return text.ToString();
        }
    }
}
