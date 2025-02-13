using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared
{
    /// <summary>
    /// Contains predefined chat color codes used in Open Roleplay.
    /// </summary>
    public static class ChatColor
    {
        public const string White = "{FFFFFF}";
        public const string Yellow = "{FFFF00}";
        public const string DarkGreen = "{007500}";
        public const string Calypso = "{276b80}";
        public const string Red = "{FF0000}";
        public const string CornflowerBlue = "{6495ED}";
        public const string ChromeYellow = "{FFA500}";

        // Open Roleplay Colors
        public const string Main = CornflowerBlue; // Reusing the value from CornflowerBlue
        public const string Highlight = "{B2CAF6}";
    }
}
