using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Entities;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Chat.Helpers;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Radios.Services
{
    public class RadioBroadcastService : IRadioBroadcastService
    {
        private const string TalkVia = "radio";
        // How far from a radio playing out loud people overhear it; the last part comes through muffled.
        private const float OverhearRange = 4.0f;
        private const float OverhearClearRange = 2.0f;
        private static readonly Color RadioColor = new Color(130, 200, 160);
        private static readonly Color OverheardColor = new Color(170, 190, 180);

        private readonly IEntityManager _entityManager;
        private readonly IRadioService _radioService;
        private readonly IRadioFrequencyService _radioFrequencyService;
        private readonly IChatService _chatService;
        private readonly IRadioBatteryService _radioBatteryService;

        public RadioBroadcastService(IEntityManager entityManager, IRadioService radioService, IRadioFrequencyService radioFrequencyService, IChatService chatService,
            IRadioBatteryService radioBatteryService)
        {
            _radioBatteryService = radioBatteryService;
            _entityManager = entityManager;
            _radioService = radioService;
            _radioFrequencyService = radioFrequencyService;
            _chatService = chatService;
        }

        public string? TransmitFromPlayer(Player player, string text)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return "You must be playing a character to use a radio.";
            }

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            Radio? radio = _radioService.GetRadioToUse(player);
            string? error = GetTransmitError(radio, character);
            if (error != null)
            {
                return error;
            }

            _chatService.SendTalkMessage(player, text, TalkVia);
            HashSet<Player> heardOverRadio = Transmit(radio!.GetFrequency(), character.GetName(), text, radio);
            SendConfirmation(player, radio.GetFrequency(), character.GetName(), text, heardOverRadio);
            _radioBatteryService.UseTransmitBattery(player, radio);
            return null;
        }

        public void TransmitFromSystem(uint frequency, string senderName, string text)
        {
            Transmit(frequency, senderName, text, null);
        }

        private string? GetTransmitError(Radio? radio, Character character)
        {
            if (radio == null)
                return "You don't have a radio.";
            if (!radio.IsOn())
                return "Your radio is off. Turn it on with /radio.";
            return _radioFrequencyService.GetTalkError(radio.GetFrequency(), character);
        }

        /// <summary>
        /// Holders of every radio that can decode the frequency see the line once; people near a radio that plays out
        /// loud overhear it. The sending radio is skipped: a radio doesn't play its own transmission.
        /// </summary>
        private HashSet<Player> Transmit(uint frequency, string senderName, string text, Radio? sendingRadio)
        {
            List<RadioSpeaker> speakers = _radioService.GetPlayingRadios(frequency)
                .Where(speaker => speaker.Radio.GetId() != sendingRadio?.GetId() && CanDecode(speaker, frequency))
                .ToList();

            List<string> radioLines = ChatLineSplitter.Split($"[Radio {_radioFrequencyService.GetLabel(frequency)}] {senderName}: {text}");
            HashSet<Player> heardOverRadio = new HashSet<Player>();
            foreach (Player holder in speakers.SelectMany(speaker => speaker.Holders).Where(heardOverRadio.Add))
            {
                SendLines(holder, RadioColor, radioLines);
            }

            SendToBystanders(speakers.Where(speaker => speaker.PlaysAloud), text, heardOverRadio);
            return heardOverRadio;
        }

        /// <summary>
        /// A radio plays an encrypted frequency only while someone allowed on it holds it or sits with it.
        /// </summary>
        private bool CanDecode(RadioSpeaker speaker, uint frequency)
        {
            if (_radioFrequencyService.CanListen(frequency, null))
            {
                return true;
            }
            return speaker.Holders.Any(holder => holder.IsPlayerPlayingAsCharacter() && _radioFrequencyService.CanListen(frequency, holder.GetPlayerCurrentlyPlayingAsCharacter()));
        }

        /// <summary>
        /// Each bystander hears the closest radio once, more muffled the further away they stand.
        /// </summary>
        private void SendToBystanders(IEnumerable<RadioSpeaker> loudSpeakers, string text, HashSet<Player> heardOverRadio)
        {
            List<RadioSpeaker> speakers = loudSpeakers.ToList();
            if (speakers.Count == 0)
            {
                return;
            }

            foreach (Player bystander in _entityManager.GetComponents<Player>().Where(player => !heardOverRadio.Contains(player)))
            {
                float? distance = GetDistanceToClosestSpeaker(bystander, speakers);
                if (distance.HasValue)
                {
                    string heardText = text.Muffle(distance.Value, OverhearClearRange, OverhearRange);
                    SendLines(bystander, OverheardColor, ChatLineSplitter.Split($"(Radio) {heardText}"));
                }
            }
        }

        private static float? GetDistanceToClosestSpeaker(Player player, List<RadioSpeaker> speakers)
        {
            return speakers
                .Where(speaker => speaker.VirtualWorld == null || speaker.VirtualWorld == player.VirtualWorld)
                .Select(speaker => (float?)player.GetDistanceFromPoint(speaker.Position))
                .Where(distance => distance <= OverhearRange)
                .Min();
        }

        /// <summary>
        /// The sender sees their own line, so they know it went out, unless another radio of theirs already showed it.
        /// </summary>
        private void SendConfirmation(Player player, uint frequency, string senderName, string text, HashSet<Player> heardOverRadio)
        {
            if (!heardOverRadio.Contains(player))
            {
                SendLines(player, RadioColor, ChatLineSplitter.Split($"[Radio {_radioFrequencyService.GetLabel(frequency)}] {senderName}: {text}"));
            }
        }

        private static void SendLines(Player player, Color color, List<string> lines)
        {
            foreach (string line in lines)
            {
                player.SendClientMessage(color, line);
            }
        }
    }
}
