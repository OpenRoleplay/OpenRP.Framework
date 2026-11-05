using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Skills.Services;
using OpenRP.Framework.Features.Skills.Services.Dialogs;

namespace OpenRP.Framework.Features.Skills.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSkills(this IServiceCollection self)
        {
            // SkillService is transient because it uses a database context.
            return self
                .AddTransient<ISkillService, SkillService>()
                .AddSingleton<ISkillDialogService, SkillDialogService>();
        }
    }
}
