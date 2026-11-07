using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.AccountSettingsFeature.Components;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Chat.Helpers;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Chat.Services
{
    public class ChatService : IChatService
    {
        private IEntityManager _entityManager;
        public ChatService(IEntityManager entityManager) 
        { 
            _entityManager = entityManager;
        }

        public void SendInfoMessage(PlayerInfoMessageType type, string text)
        {
            string message = TempChatHelper.ReturnPlayerInfoMessage(type, text);

            foreach (Player player in _entityManager.GetComponents<Player>())
            {
                player.SendClientMessage(message);
            }
        }

        private void SendClientRangedMessage(Player player, Color color, float range, string text)
        {
            foreach (Player foreachPlayer in _entityManager.GetComponents<Player>())
            {
                if (foreachPlayer.IsInRangeOfPoint(range, player.Position) && player.VirtualWorld == foreachPlayer.VirtualWorld)
                {
                    foreachPlayer.SendClientMessage(color, text);
                }
            }
        }

        private const float TalkRange = 7.0f;
        private const float TalkClearRange = 5.0f;
        private static readonly Color TalkColor = Color.FromString("F5F5F5", ColorFormat.RGB);

        public void SendTalkMessage(string name, Vector3 position, string text, string accent = null, int virtualWorld = 0)
        {
            foreach (Player listener in GetTalkListeners(position, virtualWorld))
            {
                string heardText = GetTalkTextForDistance(text, listener.GetDistanceFromPoint(position));
                listener.SendClientMessage(TalkColor, FormatTalkMessage(name, accent, heardText));
            }
        }

        private IEnumerable<Player> GetTalkListeners(Vector3 position, int virtualWorld)
        {
            return _entityManager.GetComponents<Player>()
                .Where(player => player.VirtualWorld == virtualWorld && player.GetDistanceFromPoint(position) <= TalkRange);
        }

        /// <summary>
        /// Listeners near the edge of the range hear part of the text; each listener gets their own muffling.
        /// </summary>
        private static string GetTalkTextForDistance(string text, float distance)
        {
            return distance >= TalkClearRange ? text.Muffle(distance, 3, 5) : text;
        }

        private static string FormatTalkMessage(string name, string accent, string text)
        {
            string accentPart = String.IsNullOrEmpty(accent) ? "" : $"[{accent} accent] ";
            return $"{name} {accentPart}says: {text}";
        }

        public void SendTalkMessage(Player player, string text)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();

            if (character != null)
            {
                string name = character.GetName();
                string accent = character.GetAccent();
                Vector3 position = player.Position;

                SendTalkMessage(name, position, text, accent, player.VirtualWorld);
            }
            else
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be logged into a character to be able to talk!");
            }
        }

        public void SendPlayerChatMessage(Player player, PlayerChatMessageType type, string text)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();

            if (character != null)
            {
                switch (type)
                {
                    case PlayerChatMessageType.OOC:
                        string CHAT_ACTION_OOC = String.Format("(( OOC | {0}: {1} ))", character.GetName(), text);

                        foreach (Player foreachPlayer in _entityManager.GetComponents<Player>())
                        {
                            AccountSettings foreachPlayerSettings = foreachPlayer.GetComponent<AccountSettings>();

                            if (foreachPlayerSettings != null && foreachPlayerSettings.GetGlobalChatEnabled())
                            {
                                foreachPlayer.SendClientMessage(Color.LightGray, CHAT_ACTION_OOC);
                            }
                        }
                        break;
                    case PlayerChatMessageType.LOOC:
                        string CHAT_ACTION_LOOC = String.Format("(( L-OOC | {0}: {1} ))", character.GetName(),text);

                        SendClientRangedMessage(player, Color.LightGray, 20, CHAT_ACTION_LOOC);
                        break;
                    case PlayerChatMessageType.NEWBIE:
                        string CHAT_ACTION_NEWBIE = String.Format("(( Newbie | {0} [{1}]: {2} ))", character.GetName(), player.Entity.Handle,text);
                        
                        foreach (Player foreachPlayer in _entityManager.GetComponents<Player>())
                        {
                            AccountSettings foreachPlayerSettings = foreachPlayer.GetComponent<AccountSettings>();

                            if (foreachPlayerSettings != null && foreachPlayerSettings.GetNewbieChatEnabled())
                            {
                                foreachPlayer.SendClientMessage(Color.LightGreen, CHAT_ACTION_NEWBIE);
                            }
                        }
                        break;
                    case PlayerChatMessageType.ME:
                        string CHAT_ACTION_ME = String.Format("* {0} {1} *", character.GetName(), text);

                        SendClientRangedMessage(player, ServerColor.RoleplayActionColor, 20, CHAT_ACTION_ME);
                        break;
                    case PlayerChatMessageType.DO:
                        string CHAT_ACTION_DO = String.Format("{0} (( {1} ))", text, character.GetName());

                        SendClientRangedMessage(player, ServerColor.RoleplayActionColor, 20, CHAT_ACTION_DO);
                        break;
                    case PlayerChatMessageType.AME:
                        string CHAT_ACTION_AME = String.Format("* {0} {1} *", character.GetName(), text);

                        player.SendClientMessage(ServerColor.RoleplayActionColor, CHAT_ACTION_AME);
                        player.SetChatBubble(CHAT_ACTION_AME, ServerColor.RoleplayActionColor, 20, CHAT_ACTION_AME.Length * 60);
                        break;
                    case PlayerChatMessageType.MY:
                        string characterName = character.GetName();
                        string nameSuffix = characterName.EndsWith("s") ? "'" : "'s";
                        string CHAT_ACTION_MY = String.Format("* {0}{1} {2}", characterName, nameSuffix, text);

                        SendClientRangedMessage(player, ServerColor.RoleplayActionColor, 20, CHAT_ACTION_MY);
                        break;
                    case PlayerChatMessageType.TALK:
                        SendTalkMessage(player, text);
                        break;
                    case PlayerChatMessageType.LOW:
                        string CHAT_ACTION_LOW = String.Format("{0} says quietly: {1}", character.GetName(), text);

                        SendClientRangedMessage(player, Color.LightSlateGray, 3, CHAT_ACTION_LOW);
                        break;
                    case PlayerChatMessageType.SHOUT:
                        string CHAT_ACTION_SHOUT = String.Format("{0} shouts: {1}", character.GetName(), text);

                        SendClientRangedMessage(player, Color.Orange, 30, CHAT_ACTION_SHOUT);
                        break;
                }
            }
            else
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be logged into a character to be able to send a message!");
            }
}
    }
}
