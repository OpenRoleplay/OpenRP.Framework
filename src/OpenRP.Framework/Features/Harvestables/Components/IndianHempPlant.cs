using OpenRP.Framework.Shared;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Harvestables.Components
{
    public class IndianHempPlant : HarvestablePlant
    {
        private const int MaxLeaves = 10;

        public IndianHempPlant(string resourceName, Vector3 labelPosition)
            : base(resourceName, MaxLeaves, labelPosition)
        {
        }

        public override string GetLabelText()
        {
            if (!HasHarvestsLeft())
            {
                return $"{ChatColor.Highlight}Indian Hemp (Apocynum Cannabinum)\n{ChatColor.White}No leaves left";
            }
            return $"{ChatColor.Highlight}Indian Hemp (Apocynum Cannabinum)\n{ChatColor.White}{GetRemainingHarvests()} leaves remaining\n\nUse {ChatColor.Highlight}/harvest {GetResourceName()}";
        }
    }
}
