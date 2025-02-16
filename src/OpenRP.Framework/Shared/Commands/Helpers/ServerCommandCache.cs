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
        // List of assemblies to scan (default includes the library's own assembly)
        private static readonly List<Assembly> _assembliesToScan = new List<Assembly> { Assembly.GetExecutingAssembly() };

        // Lazy-load the commands to allow assemblies to be registered first
        private static readonly Lazy<List<ServerCommandInfo>> _cachedServerCommands =
            new Lazy<List<ServerCommandInfo>>(LoadCommandInfo);

        public static List<ServerCommandInfo> CachedServerCommands => _cachedServerCommands.Value;

        // Allow the application to register its assemblies
        public static void RegisterApplicationAssembly(Assembly applicationAssembly)
        {
            _assembliesToScan.Add(applicationAssembly);
        }

        private static List<ServerCommandInfo> LoadCommandInfo()
        {
            return _assembliesToScan
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                .Where(m => m.GetCustomAttributes(typeof(ServerCommandAttribute), false).Any())
                .Select(m =>
                {
                    var attr = m.GetCustomAttribute<ServerCommandAttribute>();
                    string name = attr.Name ?? m.Name;
                    return new ServerCommandInfo
                    {
                        Name = name,
                        Description = !string.IsNullOrEmpty(attr.Description)
                            ? attr.Description
                            : "This command has no description yet.",
                        GroupPath = attr.CommandGroups,
                        Method = m,
                        Attribute = attr
                    };
                })
                .ToList();
        }
    }
}
