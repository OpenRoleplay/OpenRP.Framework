using OpenRP.Framework.Features.FlyMode.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.FlyMode.Components
{
    /// <summary>
    /// Marks a player who just landed. Holds the state to put back when they spawn, and is removed after that.
    /// </summary>
    public class FlyModeRestore : Component
    {
        private readonly FlyModeSavedState _savedState;

        public FlyModeRestore(FlyModeSavedState savedState)
        {
            _savedState = savedState;
        }

        public FlyModeSavedState GetSavedState()
        {
            return _savedState;
        }
    }
}
