using OpenRP.Framework.Features.Vehicles.Enums;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Vehicles.Helpers
{
    /// <summary>
    /// The rules for turn signals: which key press means what, which side a state lights, and the messages.
    /// </summary>
    public static class IndicatorRules
    {
        // How long after a key-driven change the driver's look keys are ignored, so a held or bounced key doesn't flicker the signals.
        public const int KeyCooldownInMilliseconds = 500;

        /// <summary>
        /// The state a key press asks for, or null when no look key was pressed in this change. Both look keys pressed in the
        /// same change is the hazard switch; a single key press asks for its side.
        /// </summary>
        public static IndicatorState? GetPressedState(Keys newKeys, Keys oldKeys)
        {
            Keys justPressed = newKeys & ~oldKeys;
            bool leftJustPressed = IsHeld(justPressed, Keys.LookLeft);
            bool rightJustPressed = IsHeld(justPressed, Keys.LookRight);
            if (leftJustPressed && rightJustPressed)
            {
                return IndicatorState.Hazards;
            }
            if (leftJustPressed)
            {
                return IndicatorState.Left;
            }
            return rightJustPressed ? IndicatorState.Right : null;
        }

        /// <summary>
        /// The state after a look key press. Hazards go off on any press. Otherwise the same side switches off, and another side switches to it.
        /// </summary>
        public static IndicatorState GetStateAfterKeyPress(IndicatorState currentState, IndicatorState pressedState)
        {
            if (currentState == IndicatorState.Hazards)
            {
                return IndicatorState.Off;
            }
            return currentState == pressedState ? IndicatorState.Off : pressedState;
        }

        /// <summary>
        /// True when the key cooldown that started at startedAtTick is over at nowTick. Both are in milliseconds.
        /// </summary>
        public static bool IsCooldownOver(long startedAtTick, long nowTick)
        {
            return nowTick - startedAtTick >= KeyCooldownInMilliseconds;
        }

        /// <summary>
        /// The state for a word of the /blinker command, or null when the word isn't one of them.
        /// </summary>
        public static IndicatorState? ParseState(string? word)
        {
            switch (word?.ToLowerInvariant())
            {
                case "left":
                    return IndicatorState.Left;
                case "right":
                    return IndicatorState.Right;
                case "hazards":
                    return IndicatorState.Hazards;
                case "off":
                    return IndicatorState.Off;
                default:
                    return null;
            }
        }

        /// <summary>
        /// True when the left-hand lights of the vehicle flash in this state.
        /// </summary>
        public static bool IsLeftSideLit(IndicatorState state)
        {
            return state == IndicatorState.Left || state == IndicatorState.Hazards;
        }

        /// <summary>
        /// True when the right-hand lights of the vehicle flash in this state.
        /// </summary>
        public static bool IsRightSideLit(IndicatorState state)
        {
            return state == IndicatorState.Right || state == IndicatorState.Hazards;
        }

        /// <summary>
        /// The /me line for switching to this state.
        /// </summary>
        public static string GetSwitchMessage(IndicatorState state)
        {
            switch (state)
            {
                case IndicatorState.Left:
                    return "flicks the turn signal lever left.";
                case IndicatorState.Right:
                    return "flicks the turn signal lever right.";
                case IndicatorState.Hazards:
                    return "presses the hazard switch.";
                default:
                    return "switches off the turn signal.";
            }
        }

        /// <summary>
        /// The error for asking for a state the vehicle already has.
        /// </summary>
        public static string GetAlreadySetMessage(IndicatorState state)
        {
            switch (state)
            {
                case IndicatorState.Left:
                    return "The left turn signal is already on.";
                case IndicatorState.Right:
                    return "The right turn signal is already on.";
                case IndicatorState.Hazards:
                    return "The hazards are already on.";
                default:
                    return "The turn signals are already off.";
            }
        }

        private static bool IsHeld(Keys keys, Keys key)
        {
            return (keys & key) != 0;
        }
    }
}
