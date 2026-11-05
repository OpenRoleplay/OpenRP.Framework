using OpenRP.Framework.Features.Skills.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Skills.Services.Dialogs
{
    public interface ISkillDialogService
    {
        /// <summary>
        /// Shows the skills of the character the player is playing as.
        /// </summary>
        void OpenSkills(Player player);

        /// <summary>
        /// Shows one skill in detail, with a button back to the list.
        /// </summary>
        void OpenSkillDetails(Player player, CharacterSkill skill);
    }
}
