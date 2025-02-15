using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Animations.Services;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Services;
using SampSharp.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServerSystemEvents(this IServiceCollection self)
        {
            return self
                .AddSingleton<IServerEventAggregator, ServerEventAggregator>()
                .AddServerSystemsInAssembly();
        }

        public static IServiceCollection AddServerSystemsInAssembly(this IServiceCollection services, Assembly assembly)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            var types = new AssemblyScanner().IncludeAssembly(assembly)
                .Implements<IServerSystem>()
                .ScanTypes();

            foreach (var type in types)
                AddServerSystem(services, type);

            return services;
        }

        public static IServiceCollection AddServerSystemsInAssembly(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            return AddServerSystemsInAssembly(services, Assembly.GetCallingAssembly());
        }

        public static IServiceCollection AddServerSystem(this IServiceCollection services, Type type)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (type == null) throw new ArgumentNullException(nameof(type));

            return services.AddSingleton(type);
        }
    }
}
