using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Services;
using OpenRP.Framework.Features.Phones.Services.Dialogs;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Systems
{
    public class PhoneServerSystem : IServerSystem
    {
        /// <summary>
        /// What a character says nearby goes into the phone too, when they're in a call.
        /// </summary>
        [ServerEvent]
        public void OnPlayerChatMessageSent(OnPlayerChatMessageSentEventArgs args, IPhoneCallService phoneCallService)
        {
            if (args.Player == null || !IsSpeech(args.Type))
            {
                return;
            }
            phoneCallService.PassOnSpeech(args.Player, args.Text);
        }

        [ServerEvent]
        public void OnPhoneRinging(OnPhoneRingingEventArgs args, IPhoneService phoneService, IPhoneDialogService phoneDialogService)
        {
            Phone? phone = phoneService.GetPhone(args.PhoneId);
            if (args.Carrier == null || phone == null)
            {
                return;
            }
            phoneDialogService.OpenIncomingCall(args.Carrier, phone, args.CallerNumber);
        }

        /// <summary>
        /// Tells the player what came in on their phones while they were away.
        /// </summary>
        [ServerEvent]
        public void OnCharacterSelected(OnCharacterSelectedEventArgs args, IPhoneService phoneService)
        {
            if (args.Player == null)
            {
                return;
            }

            foreach (Phone phone in phoneService.GetCarriedPhones(args.Player))
            {
                string? news = phone.IsOn() ? phoneService.GetNewsSummary(phone) : null;
                if (news != null)
                {
                    args.Player.SendClientMessage($"{ChatColor.White}[Phone] {news}. Use /phone to see.");
                }
            }
        }

        private static bool IsSpeech(PlayerChatMessageType type)
        {
            return type == PlayerChatMessageType.TALK || type == PlayerChatMessageType.LOW || type == PlayerChatMessageType.SHOUT;
        }
    }
}
