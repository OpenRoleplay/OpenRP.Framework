using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Dialogs
{
    public class BetterListDialog : ListDialog
    {
        private int _currentRowIndex;
        public BetterListDialog(string button1, string button2) : base(string.Empty, button1, button2)
        {
            _currentRowIndex = -1;
        }

        public void SetTitle(TitleType type, params string[] strings)
        {
            Caption = DialogHelper.GetBetterTitle(type, strings);
        }

        public int AddHeaders(string headerText)
        {
            ListDialogRow tablistDialogRow = new ListDialogRow(ChatColor.CornflowerBlue + headerText);

            if (_currentRowIndex != -1)
            {
                Rows.Add(String.Empty);
                _currentRowIndex++;
            }

            Rows.Add(tablistDialogRow);
            _currentRowIndex++;

            return _currentRowIndex;
        }

        /// <summary>
        /// Overrides the Add method to prefix each column with ChatColor.White.
        /// </summary>
        /// <param name="columns">The column strings to add.</param>
        public int AddRow(string listItem)
        {
            Rows.Add(new ListDialogRow(ChatColor.White + listItem));
            _currentRowIndex++;

            return _currentRowIndex;
        }
    }
}
