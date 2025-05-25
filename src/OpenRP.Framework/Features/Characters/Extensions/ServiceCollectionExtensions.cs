﻿using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Animations.Services;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Characters.Services;
using OpenRP.Framework.Features.Characters.Services.Dialogs;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Characters.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCharacters(this IServiceCollection self)
        {
            return self
                .AddTransient<ITempCharacterService, TempCharacterService>()
                .AddSingleton<ICharacterDialogService, CharacterDialogService>();
        }
    }
}
