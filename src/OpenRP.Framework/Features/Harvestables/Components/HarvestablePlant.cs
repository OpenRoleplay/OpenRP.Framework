using OpenRP.Framework.Features.Harvestables.Entities;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Harvestables.Components
{
    /// <summary>
    /// A plant that can be harvested a limited number of times. Each plant type derives from this and can add its own
    /// state and behaviour; its object and text label are child entities.
    /// </summary>
    public abstract class HarvestablePlant : Component, IHarvestableObject
    {
        private const float HarvestRange = 3.0f;

        private readonly string _resourceName;
        private readonly Vector3 _labelPosition;
        private int _remainingHarvests;

        protected HarvestablePlant(string resourceName, int maxHarvests, Vector3 labelPosition)
        {
            _resourceName = resourceName;
            _remainingHarvests = maxHarvests;
            _labelPosition = labelPosition;
        }

        /// <summary>
        /// The text above the plant, for example its name and how many harvests are left.
        /// </summary>
        public abstract string GetLabelText();

        public string GetResourceName()
        {
            return _resourceName;
        }

        public Vector3 GetLabelPosition()
        {
            return _labelPosition;
        }

        public int GetRemainingHarvests()
        {
            return _remainingHarvests;
        }

        public bool HasHarvestsLeft()
        {
            return _remainingHarvests > 0;
        }

        public bool IsPlayerNearby(Player player)
        {
            return player.IsInRangeOfPoint(HarvestRange, _labelPosition);
        }

        public void RemoveOneHarvest()
        {
            if (!HasHarvestsLeft())
            {
                return;
            }

            _remainingHarvests--;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            DynamicTextLabel? label = this.GetComponentInChildren<DynamicTextLabel>();
            if (label != null)
            {
                label.Text = GetLabelText();
            }
        }
    }
}
