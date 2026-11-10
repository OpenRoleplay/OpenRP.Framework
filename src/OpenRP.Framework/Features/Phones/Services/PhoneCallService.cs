using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Entities;
using OpenRP.Framework.Features.Phones.Enums;
using OpenRP.Framework.Features.Phones.Helpers;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Phones.Services
{
    public class PhoneCallService : IPhoneCallService
    {
        private static readonly TimeSpan RingInterval = TimeSpan.FromSeconds(6);
        private static readonly TimeSpan RingTimeout = TimeSpan.FromSeconds(30);
        // How long a caller holds the phone before hearing why the number can't be reached.
        private static readonly TimeSpan FailureDelay = TimeSpan.FromSeconds(3);
        private static readonly Color PhoneColor = new Color(240, 220, 130);

        private readonly IEntityManager _entityManager;
        private readonly IPhoneService _phoneService;
        private readonly IPhoneEditorService _phoneEditorService;
        private readonly IPhoneAlertService _phoneAlertService;
        private readonly IPhoneRoleplayService _phoneRoleplayService;
        private readonly IServerEventAggregator _serverEventAggregator;

        public PhoneCallService(IEntityManager entityManager, IPhoneService phoneService, IPhoneEditorService phoneEditorService,
            IPhoneAlertService phoneAlertService, IPhoneRoleplayService phoneRoleplayService, IServerEventAggregator serverEventAggregator)
        {
            _phoneRoleplayService = phoneRoleplayService;
            _entityManager = entityManager;
            _phoneService = phoneService;
            _phoneEditorService = phoneEditorService;
            _phoneAlertService = phoneAlertService;
            _serverEventAggregator = serverEventAggregator;
        }

        public string? Dial(Player caller, Phone phone, uint number)
        {
            string? error = GetDialError(caller, phone, number);
            if (error != null)
            {
                return error;
            }

            caller.SpecialAction = SpecialAction.UseCellphone;
            _phoneRoleplayService.SendDialAction(caller);

            IServerPhoneNumber? serverNumber = _phoneService.GetServerNumber(number);
            if (PhoneNumbers.IsServerNumber(number))
            {
                DialServerNumber(caller, phone, number, serverNumber);
                return null;
            }

            DialPhone(caller, phone, number);
            return null;
        }

        public string? Answer(Player player)
        {
            PhoneCall? call = GetRingingCallFor(player);
            if (call == null)
            {
                return "Your phone isn't ringing.";
            }

            call.Connect(player.Entity);
            player.SpecialAction = SpecialAction.UseCellphone;
            _phoneRoleplayService.SendAnswerAction(player);
            SendPhoneLine(GetCaller(call), "Someone picks up.");
            return null;
        }

        public string? HangUp(Player player)
        {
            if (player.GetComponent<PhoneCallFailing>() != null)
            {
                StopFailingCall(player);
                return null;
            }

            PhoneCall? call = GetCallOf(player);
            if (call == null)
            {
                return "You aren't in a call.";
            }

            bool isDeclining = call.IsRinging() && call.GetCallerPlayer() != player.Entity;
            if (isDeclining)
            {
                _phoneRoleplayService.SendDeclineAction(player);
                SendPhoneLine(GetOtherSide(call, player), "The call is declined.");
            }
            else
            {
                _phoneRoleplayService.SendHangUpAction(player);
                SendPhoneLine(call.IsConnected() ? GetOtherSide(call, player) : null, "The line goes dead.");
            }
            EndCall(call, isDeclining ? PhoneCallLogType.Incoming : PhoneCallLogType.Missed, hungUpBy: player);
            return null;
        }

        public PhoneCall? GetCallOf(Player player)
        {
            return GetCalls().FirstOrDefault(call => call.GetCallerPlayer() == player.Entity || call.GetCalledPlayer() == player.Entity)
                ?? GetRingingCallFor(player);
        }

        public bool IsPhoneInCall(Phone phone)
        {
            return GetCalls().Any(call => call.GetCallerPhoneId() == phone.GetId() || call.GetCalledPhoneId() == phone.GetId());
        }

        public void PassOnSpeech(Player speaker, string text)
        {
            PhoneCall? call = GetCallOf(speaker);
            if (call == null || !call.IsConnected())
            {
                return;
            }

            bool isCaller = call.GetCallerPlayer() == speaker.Entity;
            if (!isCaller)
            {
                SendSpeech(GetCaller(call), call.GetCallerPhoneId(), GetCalledNumber(call), text);
                return;
            }

            IServerPhoneNumber? serverNumber = call.GetServerNumber();
            if (serverNumber != null)
            {
                serverNumber.OnCallerSpoke(CreateLine(call), text);
                return;
            }
            SendSpeech(GetCalledPlayer(call), call.GetCalledPhoneId(), GetCallerNumber(call), text);
        }

        public void ProcessCalls()
        {
            foreach (PhoneCallFailing failingCall in _entityManager.GetComponents<PhoneCallFailing>().Where(failingCall => failingCall.IsDue()).ToList())
            {
                EndFailingCall(failingCall);
            }

            foreach (PhoneCall call in GetCalls())
            {
                if (!IsStillPossible(call))
                {
                    SendPhoneLine(GetCaller(call), "The call is cut off.");
                    SendPhoneLine(GetCalledPlayer(call), "The call is cut off.");
                    EndCall(call, PhoneCallLogType.Missed);
                }
                else if (call.IsRinging())
                {
                    ProcessRinging(call);
                }
            }
        }

        public void EndCallsOf(Player player)
        {
            foreach (PhoneCall call in GetCalls().Where(call => call.GetCallerPlayer() == player.Entity || call.GetCalledPlayer() == player.Entity))
            {
                SendPhoneLine(GetOtherSide(call, player), "The call is cut off.");
                EndCall(call, PhoneCallLogType.Missed, leavingPlayer: player);
            }
        }

        public void SayToCaller(PhoneCall call, string speakerName, string text)
        {
            Player? caller = GetCaller(call);
            caller?.SendClientMessage(PhoneColor, $"(Phone) {speakerName}: {text}");
        }

        public void EndCall(PhoneCall call)
        {
            SendPhoneLine(GetCaller(call), "The line goes dead.");
            EndCall(call, PhoneCallLogType.Missed);
        }

        private void DialServerNumber(Player caller, Phone phone, uint number, IServerPhoneNumber? serverNumber)
        {
            if (serverNumber == null)
            {
                _phoneEditorService.AddCallLog(phone, number, PhoneCallLogType.Outgoing, DateTime.UtcNow, 0);
                BeginFailingCall(caller, "The number you have dialed is not in service.");
                return;
            }

            PhoneCall call = CreateCall(caller, phone, number, null, serverNumber);
            call.Connect(default);
            serverNumber.OnCallConnected(CreateLine(call));
        }

        private void DialPhone(Player caller, Phone phone, uint number)
        {
            Phone? calledPhone = _phoneService.GetPhoneByNumber(number);
            string? unreachableText = GetUnreachableText(calledPhone);
            if (unreachableText != null)
            {
                _phoneEditorService.AddCallLog(phone, number, PhoneCallLogType.Outgoing, DateTime.UtcNow, 0);
                BeginFailingCall(caller, unreachableText);
                return;
            }

            CreateCall(caller, phone, number, calledPhone!.GetId(), null);
            SendPhoneLine(caller, "It's ringing. Use /hangup to stop.");
        }

        /// <summary>
        /// The caller holds the phone for a moment before hearing the reason, like a real call. It also lets the
        /// phone animation start on the client before it's stopped again; stopping it at once leaves it stuck.
        /// </summary>
        private void BeginFailingCall(Player caller, string reason)
        {
            caller.AddComponent<PhoneCallFailing>(DateTime.UtcNow + FailureDelay, reason);
        }

        private void EndFailingCall(PhoneCallFailing failingCall)
        {
            Player? caller = failingCall.GetComponent<Player>();
            string reason = failingCall.GetReason();
            failingCall.Destroy();
            if (caller == null)
            {
                return;
            }

            SendPhoneLine(caller, reason);
            PutPhoneDown(caller, null, null);
        }

        private void StopFailingCall(Player player)
        {
            player.DestroyComponents<PhoneCallFailing>();
            _phoneRoleplayService.SendHangUpAction(player);
            StopPhoneAnimation(player);
        }

        /// <summary>
        /// What the caller hears instead of ringing, or null when the phone can ring.
        /// </summary>
        private string? GetUnreachableText(Phone? calledPhone)
        {
            if (calledPhone == null)
                return "The number you have dialed is not in service.";
            if (!calledPhone.IsOn())
                return "The mobile phone you are calling is switched off.";
            if (IsPhoneInCall(calledPhone))
                return "The line is busy.";
            return null;
        }

        private string? GetDialError(Player caller, Phone phone, uint number)
        {
            if (!phone.IsOn())
                return "Your phone is turned off.";
            if (GetCallOf(caller) != null || caller.GetComponent<PhoneCallFailing>() != null)
                return "You're in a call already. Use /hangup first.";
            if (IsPhoneInCall(phone))
                return "That phone is in a call already.";
            if (number == phone.GetNumber())
                return "You can't call your own number.";
            return null;
        }

        /// <summary>
        /// Rings again every few seconds, and gives up after half a minute. A phone nobody carries here rings unheard.
        /// </summary>
        private void ProcessRinging(PhoneCall call)
        {
            DateTime now = DateTime.UtcNow;
            Phone? calledPhone = GetCalledPhone(call);
            if (calledPhone == null || now - call.GetStartedAt() >= RingTimeout)
            {
                SendPhoneLine(GetCaller(call), "Nobody answers.");
                EndCall(call, PhoneCallLogType.Missed);
                return;
            }

            if (now - call.GetLastRingAt() < RingInterval)
            {
                return;
            }

            bool isFirstRing = call.GetLastRingAt() == DateTime.MinValue;
            call.SetLastRingAt(now);
            uint callerNumber = GetCallerNumber(call);
            _phoneAlertService.Ring(calledPhone, callerNumber);
            if (isFirstRing)
            {
                PublishRinging(calledPhone, callerNumber);
            }
        }

        private void PublishRinging(Phone calledPhone, uint callerNumber)
        {
            Player? carrier = _phoneService.GetCarrier(calledPhone);
            if (carrier == null)
            {
                return;
            }

            // Not awaited: the dispatcher logs each handler's errors itself.
            _ = _serverEventAggregator.PublishAsync(new OnPhoneRingingEventArgs { Carrier = carrier, PhoneId = calledPhone.GetId(), CallerNumber = callerNumber });
        }

        /// <summary>
        /// A call ends when the caller lost their phone or turned it off, and when the one who answered did.
        /// </summary>
        private bool IsStillPossible(PhoneCall call)
        {
            if (!IsCarryingWorkingPhone(GetCaller(call), _phoneService.GetPhone(call.GetCallerPhoneId())))
            {
                return false;
            }
            if (call.IsRinging())
            {
                return GetCalledPhone(call)?.IsOn() == true;
            }
            return call.IsToServerNumber() || IsCarryingWorkingPhone(GetCalledPlayer(call), GetCalledPhone(call));
        }

        private bool IsCarryingWorkingPhone(Player? player, Phone? phone)
        {
            return player != null && phone != null && phone.IsOn() && _phoneService.IsCarrying(player, phone);
        }

        /// <summary>
        /// Writes both call logs, lets a server number know, has both sides put their phone down and removes the call.
        /// <paramref name="unansweredType"/> is what the called phone logs when nobody picked up: Missed, or Incoming
        /// when it was declined.
        /// </summary>
        private void EndCall(PhoneCall call, PhoneCallLogType unansweredType, Player? leavingPlayer = null, Player? hungUpBy = null)
        {
            if (!call.IsComponentAlive)
            {
                return;
            }

            WriteCallLogs(call, unansweredType);
            call.GetServerNumber()?.OnCallEnded(CreateLine(call));
            PutPhoneDown(GetCaller(call), leavingPlayer, hungUpBy);
            PutPhoneDown(GetCalledPlayer(call), leavingPlayer, hungUpBy);
            call.DestroyEntity();
        }

        /// <summary>
        /// Stops the phone animation and has the player lower the phone with a /me. The one who hung up already put
        /// it away with their own /me, and a leaving player is gone, so changing them could crash the server.
        /// </summary>
        private void PutPhoneDown(Player? player, Player? leavingPlayer, Player? hungUpBy)
        {
            if (player == null || player == leavingPlayer)
            {
                return;
            }

            StopPhoneAnimation(player);
            if (player != hungUpBy)
            {
                _phoneRoleplayService.SendLowerPhoneAction(player);
            }
        }

        /// <summary>
        /// The other player in the call, or null for a server number or a phone that hasn't been answered.
        /// </summary>
        private Player? GetOtherSide(PhoneCall call, Player player)
        {
            return call.GetCallerPlayer() == player.Entity ? GetCalledPlayer(call) : GetCaller(call);
        }

        private void WriteCallLogs(PhoneCall call, PhoneCallLogType unansweredType)
        {
            Phone? callerPhone = _phoneService.GetPhone(call.GetCallerPhoneId());
            if (callerPhone != null)
            {
                _phoneEditorService.AddCallLog(callerPhone, call.GetCalledNumber(), PhoneCallLogType.Outgoing, call.GetStartedAt(), call.GetDurationSeconds());
            }

            Phone? calledPhone = GetCalledPhone(call);
            if (calledPhone != null)
            {
                PhoneCallLogType type = call.IsConnected() ? PhoneCallLogType.Incoming : unansweredType;
                _phoneEditorService.AddCallLog(calledPhone, GetCallerNumber(call), type, call.GetStartedAt(), call.GetDurationSeconds());
            }
        }

        /// <summary>
        /// Always sends the stop: the special action the server reads lags behind what was just set, so checking it
        /// first can skip the stop and leave the phone stuck at the player's ear.
        /// </summary>
        private static void StopPhoneAnimation(Player? player)
        {
            if (player != null)
            {
                player.SpecialAction = SpecialAction.StopUseCellphone;
            }
        }

        /// <summary>
        /// Shows what someone said on the phone, as the listener's own phone labels the speaker's number.
        /// </summary>
        private void SendSpeech(Player? listener, ulong? listenerPhoneId, uint speakerNumber, string text)
        {
            Phone? listenerPhone = listenerPhoneId.HasValue ? _phoneService.GetPhone(listenerPhoneId.Value) : null;
            if (listener == null || listenerPhone == null)
            {
                return;
            }
            listener.SendClientMessage(PhoneColor, $"(Phone) {_phoneService.GetNumberLabel(listenerPhone, speakerNumber)}: {text}");
        }

        private static void SendPhoneLine(Player? player, string text)
        {
            player?.SendClientMessage(PhoneColor, $"(Phone) {text}");
        }

        private PhoneCall CreateCall(Player caller, Phone phone, uint number, ulong? calledPhoneId, IServerPhoneNumber? serverNumber)
        {
            return _entityManager.AddComponent<PhoneCall>(EntityId.NewEntityId(),
                new object?[] { phone.GetId(), caller.Entity, number, calledPhoneId, serverNumber });
        }

        private IServerPhoneLine CreateLine(PhoneCall call)
        {
            return new ServerPhoneLine(call, this, _phoneService, _entityManager);
        }

        private PhoneCall? GetRingingCallFor(Player player)
        {
            return GetCalls().FirstOrDefault(call => call.IsRinging() && GetCalledPhone(call) is Phone calledPhone && _phoneService.IsCarrying(player, calledPhone));
        }

        private List<PhoneCall> GetCalls()
        {
            return _entityManager.GetComponents<PhoneCall>().Where(call => call.IsComponentAlive).ToList();
        }

        private Player? GetCaller(PhoneCall call)
        {
            return GetAlivePlayer(call.GetCallerPlayer());
        }

        private Player? GetCalledPlayer(PhoneCall call)
        {
            return call.GetCalledPlayer() == default ? null : GetAlivePlayer(call.GetCalledPlayer());
        }

        private Player? GetAlivePlayer(EntityId playerId)
        {
            Player? player = _entityManager.GetComponent<Player>(playerId);
            return player != null && player.IsComponentAlive ? player : null;
        }

        private Phone? GetCalledPhone(PhoneCall call)
        {
            ulong? calledPhoneId = call.GetCalledPhoneId();
            return calledPhoneId.HasValue ? _phoneService.GetPhone(calledPhoneId.Value) : null;
        }

        private uint GetCallerNumber(PhoneCall call)
        {
            return _phoneService.GetPhone(call.GetCallerPhoneId())?.GetNumber() ?? 0;
        }

        private static uint GetCalledNumber(PhoneCall call)
        {
            return call.GetCalledNumber();
        }
    }
}
