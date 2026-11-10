using OpenRP.Framework.Features.EmergencyCalls.Entities;
using SampSharp.Entities;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.EmergencyCalls.Components
{
    /// <summary>
    /// A 911 call in progress, on the call's own entity, so it disappears with the call.
    /// </summary>
    public class EmergencyCall : Component
    {
        private readonly List<EmergencyCallLine> _lines = new List<EmergencyCallLine>();
        private readonly string _callerName;
        private readonly string _callerNumber;
        private readonly string _area;
        private int _replyCount;
        private int _dispatchCount;
        private bool _isReplying;
        private bool _hasUnansweredLines;

        public EmergencyCall(string callerName, string callerNumber, string area)
        {
            _callerName = callerName;
            _callerNumber = callerNumber;
            _area = area;
        }

        /// <summary>
        /// The caller's character name. The AI dispatcher doesn't get it: it has to ask, like a real dispatcher.
        /// </summary>
        public string GetCallerName()
        {
            return _callerName;
        }

        /// <summary>
        /// The number the call comes from, as caller ID shows it.
        /// </summary>
        public string GetCallerNumber()
        {
            return _callerNumber;
        }

        /// <summary>
        /// The area the call comes in from, such as "Red County".
        /// </summary>
        public string GetArea()
        {
            return _area;
        }

        public IReadOnlyList<EmergencyCallLine> GetLines()
        {
            return _lines;
        }

        public void AddCallerLine(string text)
        {
            _lines.Add(new EmergencyCallLine(false, text));
            _hasUnansweredLines = true;
        }

        public void AddDispatcherLine(string text)
        {
            _lines.Add(new EmergencyCallLine(true, text));
        }

        public bool HasUnansweredLines()
        {
            return _hasUnansweredLines;
        }

        public bool IsReplying()
        {
            return _isReplying;
        }

        /// <summary>
        /// Starts a reply to everything the caller said so far.
        /// </summary>
        public void BeginReply()
        {
            _isReplying = true;
            _hasUnansweredLines = false;
            _replyCount++;
        }

        public void EndReply()
        {
            _isReplying = false;
        }

        public int GetReplyCount()
        {
            return _replyCount;
        }

        public int GetDispatchCount()
        {
            return _dispatchCount;
        }

        public void AddDispatch()
        {
            _dispatchCount++;
        }
    }
}
