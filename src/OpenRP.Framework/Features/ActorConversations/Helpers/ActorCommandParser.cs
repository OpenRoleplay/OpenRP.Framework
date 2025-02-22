using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.ActorConversations.Helpers
{
    public class ActorCommandParser
    {
        private string _inputCommands;
        public ActorCommandParser(string inputCommands)
        {
            _inputCommands = inputCommands;
        }

        private List<string> SplitCommands(string input)
        {
            List<string> result = new List<string>();

            // Step 1: Split the input on newlines, trimming entries and removing empty entries
            string[] lines = input.Split("\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                // Step 2: Split each line on slashes using a lookahead to include the slash in the split
                // The regex pattern '(?=/)' splits the string at positions before a slash
                string[] commands = Regex.Split(line, @"(?=/)");

                foreach (var cmd in commands)
                {
                    string trimmedCmd = cmd.Trim();

                    // Ensure that the command is not empty after trimming
                    if (!string.IsNullOrEmpty(trimmedCmd))
                    {
                        result.Add(trimmedCmd);
                    }
                }
            }

            return result;
        }

        public List<string> Parse()
        {
            return SplitCommands(_inputCommands);
        }
    }
}
