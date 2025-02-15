using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Dialogs
{
    public class BetterMessageDialog : MessageDialog
    {
        private int _NewLinesAtLength;
        public BetterMessageDialog(string button1, string button2 = null) : base(string.Empty, string.Empty, button1, button2)
        {
            _NewLinesAtLength = 100;
        }

        public void SetTitle(TitleType type, params string[] strings)
        {
            Caption = DialogHelper.GetBetterTitle(type, strings);
        }

        public void SetNewLinesAtLength(int newLinesAtLength)
        {
            _NewLinesAtLength = newLinesAtLength;
        }

        public void SetContent(string content)
        {
            // Set the formatted content with white color
            Content = ChatColor.White + content.InsertNewlinesAtLength(_NewLinesAtLength);
        }
    }
}
