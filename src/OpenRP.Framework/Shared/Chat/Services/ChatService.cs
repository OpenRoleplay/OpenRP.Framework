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

        public void SendTalkMessage(string name, Vector3 position, string text, string accent = null)
        {
            foreach (Player foreachPlayer in _entityManager.GetComponents<Player>())
            {
                float distance = foreachPlayer.GetDistanceFromPoint(position);

                if (distance <= 7.0f)
                {
                    Color min_color = Color.FromString("F5F5F5", ColorFormat.RGB);
                    Color max_color = Color.FromString("C0C0C0", ColorFormat.RGB);
                    Color result_color;

                    if (distance >= 5.0f)
                    {
                        // TODO: Think of a more future-proof solution for this
                        /*Random change_words_perc = new Random();
                        if (change_words_perc.Next(0, 100) <= 5)
                        {
                            new_text = Regex.Replace(new_text, "([Ff])at(?!tie)", "$1attie");
                            new_text = Regex.Replace(new_text, "[Gg]reat(?! [pP]olzin(?:i)?)", "Great Polzini");
                            new_text = Regex.Replace(new_text, "([Kk])im ([Jj])ong ([Uu])n", "$1ir $2ong $3n");
                            new_text = Regex.Replace(new_text, "([Kk])im", "$1ir");
                        }*/

                        text = text.Muffle(distance, 3, 5);

                        float distance_in_perc = (100 / 2) * distance;

                        double resultRed = min_color.R + distance_in_perc * (max_color.R - min_color.R);
                        double resultGreen = min_color.G + distance_in_perc * (max_color.G - min_color.G);
                        double resultBlue = min_color.B + distance_in_perc * (max_color.B - min_color.B);

                        result_color = new Color(Convert.ToByte(resultRed), Convert.ToByte(resultGreen), Convert.ToByte(resultBlue));

                        result_color = min_color;
                    }
                    else
                    {
                        result_color = min_color;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.Append(String.Format("{0} ", name));

                    if (!String.IsNullOrEmpty(accent))
                    {
                        sb.Append(String.Format("[{0} accent] ", accent));
                    }

                    sb.Append(String.Format("says: {0}", text));

                    foreachPlayer.SendClientMessage(result_color, sb.ToString());
                }
            }
        }

        public void SendTalkMessage(Player player, string text)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();

            if (character != null)
            {
                string name = character.GetCharacterName();
                string accent = character.GetAccent();
                Vector3 position = player.Position;

                SendTalkMessage(name, position, text, accent);
            }
            else
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be logged into a character to be able to talk!");
            }
        }

        private bool ValidateOOCSender(Player player)
        {
            var accountSettings = player.GetComponent<AccountSettings>();
            if (accountSettings != null && !accountSettings.GetGlobalChatEnabled())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You have OOC chat disabled.");
                return false;
            }
            return true;
        }

        public void SendPlayerChatMessage(Player player, PlayerChatMessageType type, string text)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();

            if (character != null)
            {
                switch (type)
                {
                    case PlayerChatMessageType.OOC:
                        if (!ValidateOOCSender(player))
                        {
                            return;
                        }

                        string CHAT_ACTION_OOC = String.Format("(( OOC | {0}: {1} ))", character.GetCharacterName(), text);

                        foreach (Player foreachPlayer in _entityManager.GetComponents<Player>())
                        {
                            var foreachPlayerSettings = foreachPlayer.GetComponent<AccountSettings>();

                            if (foreachPlayerSettings != null && foreachPlayerSettings.GetGlobalChatEnabled())
                            {
                                foreachPlayer.SendClientMessage(Color.LightGray, CHAT_ACTION_OOC);
                            }
                        }
                        break;
                    case PlayerChatMessageType.LOOC:
                        string CHAT_ACTION_LOOC = String.Format("(( L-OOC | {0}: {1} ))", character.GetCharacterName(),text);

                        SendClientRangedMessage(player, Color.LightGray, 20, CHAT_ACTION_LOOC);
                        break;
                    case PlayerChatMessageType.NEWBIE:
                        // TODO: display staff lvl on newbie chat
                        // implement cooldown
                        // toggle newbie chat
                  
                        string CHAT_ACTION_NEWBIE = String.Format("(( Newbie | {0}: {1} ))", character.GetCharacterName(), text);
                        
                        foreach (Player foreachPlayer in _entityManager.GetComponents<Player>())
                        {
                            foreachPlayer.SendClientMessage(Color.LightGreen, CHAT_ACTION_NEWBIE);
                        }
                        break;
                    case PlayerChatMessageType.ME:
                        string CHAT_ACTION_ME = String.Format("* {0} {1} *", character.GetCharacterName(), text);

                        SendClientRangedMessage(player, ServerColor.RoleplayActionColor, 20, CHAT_ACTION_ME);
                        break;
                    case PlayerChatMessageType.DO:
                        string CHAT_ACTION_DO = String.Format("{0} (( {1} ))", text, character.GetCharacterName());

                        SendClientRangedMessage(player, ServerColor.RoleplayActionColor, 20, CHAT_ACTION_DO);
                        break;
                    case PlayerChatMessageType.AME:
                        string CHAT_ACTION_AME = String.Format("* {0} {1} *", character.GetCharacterName(), text);

                        player.SendClientMessage(ServerColor.RoleplayActionColor, CHAT_ACTION_AME);
                        player.SetChatBubble(CHAT_ACTION_AME, ServerColor.RoleplayActionColor, 20, CHAT_ACTION_AME.Length * 60);
                        break;
                    case PlayerChatMessageType.MY:
                        string characterName = character.GetCharacterName();
                        string nameSuffix = characterName.EndsWith("s") ? "'" : "'s";
                        string CHAT_ACTION_MY = String.Format("* {0}{1} {2}", characterName, nameSuffix, text);

                        SendClientRangedMessage(player, ServerColor.RoleplayActionColor, 20, CHAT_ACTION_MY);
                        break;
                    case PlayerChatMessageType.TALK:
                        SendTalkMessage(player, text);
                        break;
                    case PlayerChatMessageType.LOW:
                        string CHAT_ACTION_LOW = String.Format("{0} says quietly: {1}", character.GetCharacterName(), text);

                        SendClientRangedMessage(player, Color.LightSlateGray, 3, CHAT_ACTION_LOW);
                        break;
                    case PlayerChatMessageType.SHOUT:
                        string CHAT_ACTION_SHOUT = String.Format("{0} shouts: {1}", character.GetCharacterName(), text);

                        SendClientRangedMessage(player, Color.LightSteelBlue, 30, CHAT_ACTION_SHOUT);
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
