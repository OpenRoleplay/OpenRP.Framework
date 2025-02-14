using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Commands.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Commands.Helpers
{
    public static class ServerCommandCache
    {
        public static readonly List<ServerCommandInfo> CachedServerCommands = LoadCommandInfo();

        private static List<ServerCommandInfo> LoadCommandInfo()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                .Where(m => m.GetCustomAttributes(typeof(ServerCommandAttribute), false).Any())
                .Select(m =>
                {
                    var attr = m.GetCustomAttribute<ServerCommandAttribute>();
                    string name = attr.Name ?? m.Name;
                    return new ServerCommandInfo
                    {
                        Name = name,
                        Description = string.IsNullOrEmpty(attr.Description) ? $"This command has no description yet." : attr.Description,
                        GroupPath = attr.CommandGroups,
                        Method = m,
                        Attribute = attr
                    };
                })
                .ToList();
        }
    }
}
