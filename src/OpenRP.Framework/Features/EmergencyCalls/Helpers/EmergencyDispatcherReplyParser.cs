using OpenRP.Framework.Features.EmergencyCalls.Entities;
using OpenRP.Framework.Features.EmergencyCalls.Enums;
using System;
using System.Text.RegularExpressions;

namespace OpenRP.Framework.Features.EmergencyCalls.Helpers
{
    /// <summary>
    /// Reads the AI dispatcher's reply: "SAY: ..." lines, an optional "DISPATCH: POLICE|FIRE|EMS ..." line and an
    /// optional "HANGUP" line. Anything else is ignored, so stray text never reaches the game.
    /// </summary>
    public static class EmergencyDispatcherReplyParser
    {
        private const string SayPrefix = "SAY:";
        private const string DispatchPrefix = "DISPATCH:";
        private const string HangUpLine = "HANGUP";

        // A command that starts after other text on the same line. Uppercase only, so speech like "say" or "hang up"
        // isn't split.
        private static readonly Regex CommandAfterText = new Regex(@"(?<=\S)[ \t]+(?=(SAY:|DISPATCH:|HANGUP\b))", RegexOptions.Compiled);

        public static EmergencyDispatcherReply Parse(string replyText)
        {
            EmergencyDispatcherReply reply = new EmergencyDispatcherReply();
            foreach (string rawLine in PutCommandsOnOwnLines(replyText).Split('\n'))
            {
                ReadLine(reply, rawLine.Trim());
            }
            return reply;
        }

        /// <summary>
        /// The model sometimes writes "SAY: ... DISPATCH: POLICE ..." on one line. Each command gets its own line, so
        /// the broadcast isn't read as speech.
        /// </summary>
        private static string PutCommandsOnOwnLines(string replyText)
        {
            return CommandAfterText.Replace(replyText, "\n");
        }

        private static void ReadLine(EmergencyDispatcherReply reply, string line)
        {
            if (line.StartsWith(SayPrefix, StringComparison.OrdinalIgnoreCase))
            {
                AddSayLine(reply, line[SayPrefix.Length..].Trim());
            }
            else if (line.StartsWith(DispatchPrefix, StringComparison.OrdinalIgnoreCase))
            {
                ReadDispatch(reply, line[DispatchPrefix.Length..].Trim());
            }
            else if (line.Equals(HangUpLine, StringComparison.OrdinalIgnoreCase))
            {
                reply.HangsUp = true;
            }
        }

        private static void AddSayLine(EmergencyDispatcherReply reply, string text)
        {
            if (text.Length > 0)
            {
                reply.SayLines.Add(text);
            }
        }

        /// <summary>
        /// "POLICE All units, ..." becomes the service and the broadcast text.
        /// </summary>
        private static void ReadDispatch(EmergencyDispatcherReply reply, string dispatch)
        {
            int firstSpace = dispatch.IndexOf(' ');
            if (firstSpace <= 0)
            {
                return;
            }

            EmergencyServiceType? service = GetService(dispatch[..firstSpace]);
            string text = dispatch[(firstSpace + 1)..].Trim();
            if (service.HasValue && text.Length > 0)
            {
                reply.DispatchService = service;
                reply.DispatchText = text;
            }
        }

        private static EmergencyServiceType? GetService(string serviceName)
        {
            return serviceName.ToUpperInvariant() switch
            {
                "POLICE" => EmergencyServiceType.Police,
                "FIRE" => EmergencyServiceType.Fire,
                "EMS" or "AMBULANCE" => EmergencyServiceType.Ambulance,
                _ => null
            };
        }
    }
}
