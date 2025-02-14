using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using OpenRP.Framework.Shared.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Dialogs
{
    public class BetterInputDialog : InputDialog
    {
        private int _NewLinesAtLength;
        public BetterInputDialog(string button1, string button2 = null) : base()
        {
            this.Button1 = button1;
            this.Button2 = button2;

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
