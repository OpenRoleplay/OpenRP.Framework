using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Services
{
    public class PhoneRoleplayService : IPhoneRoleplayService
    {
        private const string DialText = "takes out their phone and dials a number.";
        private const string AnswerText = "takes out their phone and answers it.";
        private const string DeclineText = "glances at their phone and declines the call.";
        private const string HangUpText = "hangs up and puts the phone away.";
        private const string LowerPhoneText = "lowers the phone from their ear.";
        private const string TextMessageText = "types out a message on their phone.";

        private readonly IChatService _chatService;

        public PhoneRoleplayService(IChatService chatService)
        {
            _chatService = chatService;
        }

        public void SendDialAction(Player player)
        {
            SendAction(player, DialText);
        }

        public void SendAnswerAction(Player player)
        {
            SendAction(player, AnswerText);
        }

        public void SendDeclineAction(Player player)
        {
            SendAction(player, DeclineText);
        }

        public void SendHangUpAction(Player player)
        {
            SendAction(player, HangUpText);
        }

        public void SendLowerPhoneAction(Player player)
        {
            SendAction(player, LowerPhoneText);
        }

        public void SendTextAction(Player player)
        {
            SendAction(player, TextMessageText);
        }

        private void SendAction(Player player, string text)
        {
            _chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, text);
        }
    }
}
