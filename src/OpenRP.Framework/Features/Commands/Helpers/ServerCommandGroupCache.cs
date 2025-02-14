using OpenRP.Framework.Features.Commands.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Commands.Helpers
{
    public static class ServerCommandGroupCache
    {
        public static readonly CommandGroupNode CachedServerCommandGroups = LoadServerCommandGroups();

        private static CommandGroupNode LoadServerCommandGroups()
        {
            var commands = ServerCommandCache.CachedServerCommands;
            var root = new CommandGroupNode { Name = "Root" };

            foreach (var command in commands)
            {
                var currentGroup = root; // Reset to root for each command
                var groups = command.Attribute.CommandGroups ?? Array.Empty<string>();

                foreach (var groupName in groups)
                {
                    var existingGroup = currentGroup.Subgroups
                        .FirstOrDefault(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));

                    if (existingGroup == null)
                    {
                        existingGroup = new CommandGroupNode { Name = groupName };
                        currentGroup.Subgroups.Add(existingGroup);
                    }

                    currentGroup = existingGroup;
                }

                currentGroup.Commands.Add(command);
            }

            return root; // Return the actual root node
        }
    }
}
