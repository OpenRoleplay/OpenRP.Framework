using System;
using System.Globalization;

namespace OpenRP.Framework.Features.Discord.Helpers
{
    /// <summary>
    /// The names of the channels that show server stats.
    /// </summary>
    public static class DiscordChannelNames
    {
        private const int TimeRoundingInMinutes = 15;

        public static string GetPlayerCountName(int playerCount, int maxPlayers)
        {
            return $"Players Online: {playerCount}/{maxPlayers}";
        }

        /// <summary>
        /// The in-game date and time, rounded down to the quarter hour so the channel only changes four times per
        /// in-game hour. Invariant culture, so the date keeps its slashes on any server culture.
        /// </summary>
        public static string GetServerDateTimeName(DateTime date, TimeSpan time)
        {
            int roundedMinutes = time.Minutes / TimeRoundingInMinutes * TimeRoundingInMinutes;
            return string.Format(CultureInfo.InvariantCulture, "Server Time: {0:dd/MM/yyyy} {1:D2}:{2:D2}", date, time.Hours, roundedMinutes);
        }
    }
}
