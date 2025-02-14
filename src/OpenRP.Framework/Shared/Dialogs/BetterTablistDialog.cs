using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Dialogs
{
    public class BetterTablistDialog : TablistDialog
    {
        private int _currentRowIndex;
        public BetterTablistDialog(string button1, string button2, int ColumnCount = 1) : base(string.Empty, button1, button2, CreateDummyColumns(ColumnCount))
        {
            _currentRowIndex = -1;
        }

        private static string[] CreateDummyColumns(int columnCount = 1, bool unset = true)
        {
            string[] dummyColumns = new string[columnCount];

            for (int i = 0; i < columnCount; i++)
            {
                if (unset)
                {
                    dummyColumns[i] = "Unset";
                }
                else
                {
                    dummyColumns[i] = $"{ChatColor.White}";
                }
            }
            return dummyColumns;
        }

        public void SetTitle(TitleType type, params string[] strings)
        {
            Caption = DialogHelper.GetBetterTitle(type, strings);
        }

        public int? AddHeaders(params string[] strings)
        {
            if (strings.Length != ColumnCount)
            {
                string[] tmpStrings = CreateDummyColumns(ColumnCount);

                for (int i = 0; i < tmpStrings.Length; i++)
                {
                    tmpStrings[i] = String.Empty;
                    if (i < strings.Length)
                    {
                        tmpStrings[i] = strings[i];
                    }
                }

                strings = tmpStrings;
            }

            string[] coloredColumns = strings.Select(column => $"{ChatColor.CornflowerBlue}{column}").ToArray();
            TablistDialogRow tablistDialogRow = new TablistDialogRow(coloredColumns.ToArray());

            if (Header == null || Header.Columns.Any(i => i.Equals("Unset")))
            {
                Header = tablistDialogRow;
            }
            else
            {
                if (_currentRowIndex != -1)
                {
                    Rows.Add(CreateDummyColumns(ColumnCount, false));
                    _currentRowIndex++;
                }

                Rows.Add(tablistDialogRow);
                _currentRowIndex++;

                return _currentRowIndex;
            }
            return null;
        }

        /// <summary>
        /// Overrides the Add method to prefix each column with ChatColor.White.
        /// </summary>
        /// <param name="columns">The column strings to add.</param>
        public int AddRow(params string[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns), "Columns cannot be null.");
            }

            // Prefix each column with ChatColor.White
            string[] coloredColumns = columns.Select(column => $"{ChatColor.White}{column}").ToArray();

            Rows.Add(new TablistDialogRow(columns));
            _currentRowIndex++;

            return _currentRowIndex;
        }
    }
}
