using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Factions.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Factions.Commands
{
    public class FactionOocCommand : ISystem
    {
        // The "Faction" group holds the permissions every faction has, so every faction member can use this.
        [ServerCommand(PermissionGroups = new string[] { "Faction" },
            Description = "Send an out-of-character message to the online members of your active faction.",
            CommandGroups = new string[] { "Factions", "Chat" })]
        public void Fooc(Player player, IFactionChatService factionChatService, string text)
        {
            factionChatService.SendFactionOocMessage(player, text);
        }

        [ServerCommand(PermissionGroups = new string[] { "Faction" },
            Description = "Send an out-of-character message to the online members of your active faction.",
            CommandGroups = new string[] { "Factions", "Chat" })]
        public void Fooc(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/fooc [message]");
        }

        [ServerCommand(PermissionGroups = new string[] { "Faction" },
            Description = "Send an out-of-character message to the online members of your active faction. Short for /fooc.",
            CommandGroups = new string[] { "Factions", "Chat" })]
        public void Fo(Player player, IFactionChatService factionChatService, string text)
        {
            Fooc(player, factionChatService, text);
        }

        [ServerCommand(PermissionGroups = new string[] { "Faction" },
            Description = "Send an out-of-character message to the online members of your active faction. Short for /fooc.",
            CommandGroups = new string[] { "Factions", "Chat" })]
        public void Fo(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/fo [message]");
        }
    }
}
