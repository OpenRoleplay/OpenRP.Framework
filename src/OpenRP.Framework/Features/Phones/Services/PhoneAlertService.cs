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

            if (!phone.IsSilent())
            {
                _chatService.SendPlayerChatMessage(carrier, PlayerChatMessageType.DO, doText);
            }
            carrier.SendClientMessage($"{ChatColor.White}[Phone] {carrierText}");
        }
    }
}
