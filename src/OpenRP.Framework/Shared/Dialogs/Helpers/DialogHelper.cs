using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Dialogs.Helpers
{
    public class DialogHelper
    {
        // Title Constants
        private const string Prefix = ChatColor.White + "Open Roleplay" + ChatColor.CornflowerBlue + " | " + ChatColor.White;
        private const string ParentSeparator = ChatColor.CornflowerBlue + " | " + ChatColor.White;
        private const string ChildSeparator = ChatColor.CornflowerBlue + " -> " + ChatColor.White;

        // Button Constants
        public const string Cancel = "Cancel";
        public const string Quit = "Quit";
        public const string Previous = "Previous";
        public const string Next = "Next";
        public const string Retry = "Retry";
        public const string Yes = "Yes";
        public const string No = "No";

        public static string GetTitle(params string[] strings)
        {
            return string.Format("{0}{1}", Prefix, string.Join(ParentSeparator, strings));
        }

        public static string GetSubTitle(params string[] strings)
        {
            return string.Format("{0}{1}", Prefix, string.Join(ChildSeparator, strings));
        }

        public static string GetBetterTitle(TitleType type, params string[] strings)
        {
            string separator = type switch
            {
                TitleType.Parents => ParentSeparator,
                TitleType.Children => ChildSeparator,
                _ => string.Empty,
            };

            return $"{Prefix}{String.Join(separator, strings)}";
        }

        public static string GetBooleanAsOnOrOff(bool input)
        {
            if (input)
            {
                return $"{Color.DarkGreen}On";
            }
            return $"{Color.DarkRed}Off";
        }
    }
}
