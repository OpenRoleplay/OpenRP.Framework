using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Animations.Services;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Animations.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAnimations(this IServiceCollection self)
        {
            return self
                .AddSingleton<IAnimationManager, AnimationManager>();
        }
    }
}
