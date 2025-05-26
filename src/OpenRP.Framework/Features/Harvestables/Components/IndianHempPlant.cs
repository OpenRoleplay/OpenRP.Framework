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
    public class IndianHempPlant : Component, IHarvestableEntity
    {
        private BiomeObject _generatedObject;
        private int _remainingLeaves;

        public IndianHempPlant(BiomeObject generatedObject)
        {
            _generatedObject = generatedObject;
            _remainingLeaves = 10;

            CreateDynamicObject();
            //CreateDynamicTextLabel();
        }

        public string GetTextLabelString()
        {
            return $"{ChatColor.Highlight}Indian Hemp (Apocynum Cannabinum)\n{ChatColor.White}{_remainingLeaves} leaves remaining\n\nUse {ChatColor.Highlight}/harvest hemp";
        }

        public Vector3 GetTextLabelPosition()
        {
            Vector3 textLabelPosition = new Vector3(_generatedObject.GamePosition.XY, _generatedObject.GamePosition.Z + 0.25f);
            return textLabelPosition;
        }

        public void CreateDynamicTextLabel()
        {
            Vector3 textLabelPosition = GetTextLabelPosition();
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

        public void CreateDynamicObject()
        {
            BiomeManager.CreateGeneratorObject(_generatedObject, Entity, _colAndreasService, _streamerService);
        }

        public DynamicObject GetDynamicObject()
        {
            return GetComponentInChildren<DynamicObject>();
        }

        public bool IsPlayerNearby(Player player)
        {
            Vector3 textLabelPosition = GetTextLabelPosition();
            if (player.IsInRangeOfPoint(3.0f, textLabelPosition))
            {
                return true;
            }
            return false;
        }

        public void Harvest()
        {
            _remainingLeaves -= 1;
            UpdateDynamicTextLabel();
        }
    }
}
