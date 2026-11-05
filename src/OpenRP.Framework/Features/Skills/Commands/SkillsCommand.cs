using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Skills.Services.Dialogs;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Skills.Commands
{
    public class SkillsCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Show your character's skills and how far you are in each of them.",
            CommandGroups = new string[] { "Character" })]
        public void Skills(Player player, ISkillDialogService skillDialogService)
        {
            skillDialogService.OpenSkills(player);
        }
    }
}
