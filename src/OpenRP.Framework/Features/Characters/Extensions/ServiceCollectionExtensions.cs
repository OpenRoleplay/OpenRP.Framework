using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Characters.Services;
using OpenRP.Framework.Features.Characters.Services.Dialogs;

namespace OpenRP.Framework.Features.Characters.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCharacters(this IServiceCollection self)
        {
            // Both are transient because CharacterService owns a database context.
            return self
                .AddTransient<ICharacterService, CharacterService>()
                .AddTransient<ICharacterDialogService, CharacterDialogService>();
        }
    }
}
