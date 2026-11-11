using Microsoft.Extensions.Logging;
using OpenRP.Framework.Features.EmergencyCalls.Components;
using OpenRP.Framework.Features.EmergencyCalls.Entities;
using OpenRP.Framework.Features.EmergencyCalls.Enums;
using OpenRP.Framework.Features.EmergencyCalls.Services.Dialogs;
using OpenRP.Framework.Features.Phones.Entities;
using OpenRP.Framework.Features.Phones.Helpers;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.AI.Helpers;
using OpenRP.Framework.Shared.Locations.Helpers;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.EmergencyCalls.Services
{
    /// <summary>
    /// 911. An AI dispatcher talks with the caller and relays the call to the units; without an API key, the caller
    /// fills in a short form instead.
    /// </summary>
    public class EmergencyNumber : IServerPhoneNumber
    {
        private const uint Number = 911;
        private const string Name = "Emergency";
        private const string SpeakerName = "911 Operator";
        private const string Greeting = "911, what is your emergency?";
        // Each reply is a paid request; a call that goes on this long is wrapped up.
        private const int MaxReplies = 8;
        private const int MaxDispatchesPerCall = 3;
        private static readonly TimeSpan DispatchCooldown = TimeSpan.FromMinutes(1);

        private readonly IEntityManager _entityManager;
        private readonly IEmergencyDispatcherAi _emergencyDispatcherAi;
        private readonly IDispatchService _dispatchService;
        private readonly IEmergencyCallDialogService _emergencyCallDialogService;
        private readonly ILogger<EmergencyNumber> _logger;

        public EmergencyNumber(IEntityManager entityManager, IEmergencyDispatcherAi emergencyDispatcherAi, IDispatchService dispatchService,
            IEmergencyCallDialogService emergencyCallDialogService, ILogger<EmergencyNumber> logger)
        {
            _entityManager = entityManager;
            _emergencyDispatcherAi = emergencyDispatcherAi;
            _dispatchService = dispatchService;
            _emergencyCallDialogService = emergencyCallDialogService;
            _logger = logger;
        }

        public uint GetNumber()
        {
            return Number;
        }

        public string GetName()
        {
            return Name;
        }

        public void OnCallConnected(IServerPhoneLine line)
        {
            Player? caller = line.GetCaller();
            if (caller == null || !caller.IsPlayerPlayingAsCharacter())
            {
                line.HangUp();
                return;
            }

            EmergencyCall call = CreateEmergencyCall(line, caller);
            Say(line, call, Greeting);
            if (!_emergencyDispatcherAi.IsConfigured())
            {
                _emergencyCallDialogService.OpenEmergencyForm(caller, line, (service, what, where) => DispatchForm(line, call, service, what, where));
            }
        }

        public void OnCallerSpoke(IServerPhoneLine line, string text)
        {
            EmergencyCall? call = GetEmergencyCall(line);
            if (call == null || !_emergencyDispatcherAi.IsConfigured())
            {
                return;
            }

            // The reply waits until the caller pauses, so a follow-up line gets the same answer (ReplyToQuietCallers).
            call.AddCallerLine(text);
        }

        /// <summary>
        /// Starts a reply on every call whose caller said something new and then paused. Called twice a second.
        /// </summary>
        public void ReplyToQuietCallers()
        {
            foreach (EmergencyCall call in _entityManager.GetComponents<EmergencyCall>())
            {
                if (call.GetLine().IsActive() && call.IsWaitingForReply(AiReplyTiming.QuietTimeBeforeReply))
                {
                    _ = ReplyAsync(call.GetLine(), call);
                }
            }
        }

        public void OnCallEnded(IServerPhoneLine line)
        {
            // The EmergencyCall component lives on the call's entity, so it goes with the call.
        }

        private EmergencyCall CreateEmergencyCall(IServerPhoneLine line, Player caller)
        {
            string callerName = caller.GetPlayerCurrentlyPlayingAsCharacter().GetName();
            string callerNumber = PhoneNumbers.Format(line.GetCallerNumber());
            string area = ZoneNames.GetZoneName(caller.Position);
            return _entityManager.AddComponent<EmergencyCall>(line.GetCallId(), new object[] { callerName, callerNumber, area, line });
        }

        private EmergencyCall? GetEmergencyCall(IServerPhoneLine line)
        {
            return line.IsActive() ? _entityManager.GetComponent<EmergencyCall>(line.GetCallId()) : null;
        }

        /// <summary>
        /// Answers everything the caller said so far. Lines said while it's written wait for the next pause. The call
        /// can end during the await, so it's checked again after it.
        /// </summary>
        private async Task ReplyAsync(IServerPhoneLine line, EmergencyCall call)
        {
            if (call.GetReplyCount() >= MaxReplies)
            {
                EndLongCall(line, call);
                return;
            }

            call.BeginReply();
            EmergencyDispatcherReply? reply = await TryGenerateReplyAsync(call);
            if (!call.IsComponentAlive || !line.IsActive())
            {
                return;
            }
            call.EndReply();

            if (reply == null)
            {
                Say(line, call, "Sorry, you're breaking up. Please call again.");
                line.HangUp();
                return;
            }
            ApplyReply(line, call, reply);
        }

        private async Task<EmergencyDispatcherReply?> TryGenerateReplyAsync(EmergencyCall call)
        {
            try
            {
                return await _emergencyDispatcherAi.GenerateReplyAsync(call);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The AI dispatcher failed to reply to a 911 call from {CallerNumber}.", call.GetCallerNumber());
                return null;
            }
        }

        private void ApplyReply(IServerPhoneLine line, EmergencyCall call, EmergencyDispatcherReply reply)
        {
            foreach (string sayLine in reply.SayLines)
            {
                Say(line, call, sayLine);
            }
            if (reply.HasDispatch())
            {
                TryDispatch(line, call, reply.DispatchService!.Value, reply.DispatchText!);
            }
            if (reply.HangsUp)
            {
                line.HangUp();
            }
        }

        private void EndLongCall(IServerPhoneLine line, EmergencyCall call)
        {
            Say(line, call, "I have to clear the line now. Call back if anything changes.");
            line.HangUp();
        }

        /// <summary>
        /// The form's answers become a broadcast with everything known about the call.
        /// </summary>
        private void DispatchForm(IServerPhoneLine line, EmergencyCall call, EmergencyServiceType service, string what, string where)
        {
            if (!call.IsComponentAlive || !line.IsActive())
            {
                return;
            }

            string text = $"All units, 911 call from the {call.GetArea()} area: {what.TrimEnd('.')}. Location given: {where.TrimEnd('.')}. Caller is {call.GetCallerName()}, callback {call.GetCallerNumber()}.";
            TryDispatch(line, call, service, text);
            Say(line, call, "Help is on the way. Stay somewhere safe.");
            line.HangUp();
        }

        private void TryDispatch(IServerPhoneLine line, EmergencyCall call, EmergencyServiceType service, string text)
        {
            Player? caller = line.GetCaller();
            if (caller == null || !CanDispatch(caller, line, call))
            {
                _logger.LogInformation("A 911 dispatch from {CallerNumber} was held back by the dispatch limits.", call.GetCallerNumber());
                return;
            }

            _dispatchService.Broadcast(service, text);
            call.AddDispatch();
            call.AddDispatcherLine($"DISPATCH: {service.ToString().ToUpperInvariant()} {text}");
            caller.DestroyComponents<EmergencyDispatchCooldown>();
            caller.AddComponent<EmergencyDispatchCooldown>(new object[] { line.GetCallId(), DateTime.UtcNow });
        }

        private static bool CanDispatch(Player caller, IServerPhoneLine line, EmergencyCall call)
        {
            EmergencyDispatchCooldown? cooldown = caller.GetComponent<EmergencyDispatchCooldown>();
            return call.GetDispatchCount() < MaxDispatchesPerCall
                && (cooldown == null || !cooldown.IsBlocking(line.GetCallId(), DispatchCooldown));
        }

        private static void Say(IServerPhoneLine line, EmergencyCall call, string text)
        {
            line.Say(SpeakerName, text);
            call.AddDispatcherLine($"SAY: {text}");
        }
    }
}
