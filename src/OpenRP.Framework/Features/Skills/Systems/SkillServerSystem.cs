using OpenRP.Framework.Features.Skills.Services;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;

namespace OpenRP.Framework.Features.Skills.Systems
{
    public class SkillServerSystem : IServerSystem
    {
        [ServerEvent]
        public void OnCharacterSelected(OnCharacterSelectedEventArgs args, ISkillService skillService)
        {
            if (args.Player == null || args.Character == null)
            {
                return;
            }

            skillService.LoadCharacterSkills(args.Player, args.Character);
        }
    }
}
