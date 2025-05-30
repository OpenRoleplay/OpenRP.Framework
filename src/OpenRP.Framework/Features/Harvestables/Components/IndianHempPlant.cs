using OpenRP.Framework.Features.BiomeGenerator.Entities;
using OpenRP.Framework.Shared;
using OpenRP.Streamer;
using SampSharp.ColAndreas.Entities.Services;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Harvestables.Entities;

namespace OpenRP.Framework.Features.Harvestables.Components
{
    public class IndianHempPlant : Component, IHarvestableObject
    {
        private BiomeObject _generatedObject;
        private int _remainingLeaves;

        public IndianHempPlant()
        {
            _remainingLeaves = 10;
        }

        public string GetTextLabelString()
        {
            return $"{ChatColor.Highlight}Indian Hemp (Apocynum Cannabinum)\n{ChatColor.White}{_remainingLeaves} leaves remaining\n\nUse {ChatColor.Highlight}/harvest indian hemp";
        }

        public DynamicTextLabel GetDynamicTextLabel()
        {
            return GetComponentInChildren<DynamicTextLabel>();
        }

        public void UpdateDynamicTextLabel()
        {
            DynamicTextLabel dynamicTextLabel = GetDynamicTextLabel();
            dynamicTextLabel.Text = GetTextLabelString();
        }

        public DynamicObject GetDynamicObject()
        {
            return GetComponentInChildren<DynamicObject>();
        }

        public bool IsPlayerNearby(Player player)
        {
            Vector3 textLabelPosition = GetDynamicTextLabel().Position;
            if (player.IsInRangeOfPoint(3.0f, textLabelPosition))
            {
                return true;
            }
            return false;
        }

        public void BeginHarvest()
        {

        }

        public void EndHarvest()
        {
            _remainingLeaves -= 1;
            UpdateDynamicTextLabel();
        }
    }
}
