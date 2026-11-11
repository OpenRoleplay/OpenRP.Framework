using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Services
{
    public class PhoneAlertService : IPhoneAlertService
    {
        private const string RingText = "Their phone rings.";
        private const string BeepText = "Their phone beeps twice.";

        private readonly IPhoneService _phoneService;
        private readonly IChatService _chatService;

        public PhoneAlertService(IPhoneService phoneService, IChatService chatService)
        {
            _phoneService = phoneService;
            _chatService = chatService;
        }

        public void Ring(Phone phone, uint callerNumber)
        {
            string caller = _phoneService.GetNumberLabel(phone, callerNumber);
            Alert(phone, RingText, $"{ChatColor.Highlight}Incoming call{ChatColor.White} from {caller}. Use /answer to answer or /hangup to decline.");
        }

        public void BeepForMessage(Phone phone, uint senderNumber)
        {
            string sender = _phoneService.GetNumberLabel(phone, senderNumber);
            Alert(phone, BeepText, $"{ChatColor.Highlight}New message{ChatColor.White} from {sender}. Use /phone to read it.");
        }

        public void WarnLowBattery(Phone phone, Player holder, double warningLevel)
        {
            string doText = warningLevel switch
            {
                <= 2 => "Their phone beeps rapidly: battery almost dead.",
                <= 5 => "Their phone beeps twice: battery very low.",
                _ => "Their phone beeps: low battery."
            };
            AlertHolder(phone, holder, doText, $"{ChatColor.Highlight}Low battery{ChatColor.White} ({warningLevel}%). Recharge it in /phone.");
        }

        public void AnnounceEmptyBattery(Phone phone, Player holder)
        {
            AlertHolder(phone, holder, "Their phone beeps and dies.", $"{ChatColor.Highlight}The battery is empty{ChatColor.White} and the phone switched off. Recharge it in /phone.");
        }

        /// <summary>
        /// Out loud with a /do, or only to the carrier when the phone is silent. Nothing when it's off or not carried.
        /// </summary>
        private void Alert(Phone phone, string doText, string carrierText)
        {
            Player? carrier = _phoneService.GetCarrier(phone);
            if (carrier == null || !phone.IsOn())
            {
                return;
            }

            AlertHolder(phone, carrier, doText, carrierText);
        }

        private void AlertHolder(Phone phone, Player holder, string doText, string holderText)
        {
            if (!holder.IsComponentAlive)
            {
                return;
            }

            if (!phone.IsSilent())
            {
                _chatService.SendPlayerChatMessage(holder, PlayerChatMessageType.DO, doText);
            }
            holder.SendClientMessage($"{ChatColor.White}[Phone] {holderText}");
        }
    }
}
