using OpenRP.Framework.Shared;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.ChickenCoops.Components
{
    /// <summary>
    /// A coop where chickens breed and lay eggs over time. Its text label is a child entity.
    /// </summary>
    public class ChickenCoop : Component
    {
        private const float NearbyRange = 3.0f;
        private static readonly TimeSpan BreedInterval = TimeSpan.FromHours(1);
        private static readonly TimeSpan LayInterval = TimeSpan.FromMinutes(15);

        private readonly Vector3 _position;
        private int _chickens = 1;
        private int _eggs = 1;
        private DateTime _nextChickenBredAt = DateTime.UtcNow.Add(BreedInterval);
        private DateTime _nextEggsLaidAt = DateTime.UtcNow.Add(LayInterval);

        public ChickenCoop(Vector3 position)
        {
            _position = position;
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

        public int GetChickenCount()
        {
            return _chickens;
        }

        public int GetEggCount()
        {
            return _eggs;
        }

        public bool HasEggs()
        {
            return _eggs > 0;
        }

        public bool IsPlayerNearby(Player player)
        {
            return player.IsInRangeOfPoint(NearbyRange, _position);
        }

        public void RemoveOneEgg()
        {
            if (!HasEggs())
            {
                return;
            }

            _eggs--;
            UpdateLabel();
        }

        /// <summary>
        /// Adds a chicken when it's time to. True when a chicken was bred.
        /// </summary>
        public bool BreedChickenIfDue()
        {
            if (DateTime.UtcNow < _nextChickenBredAt)
            {
                return false;
            }

            _chickens++;
            _nextChickenBredAt = DateTime.UtcNow.Add(BreedInterval);
            UpdateLabel();
            return true;
        }

        /// <summary>
        /// Lets every chicken have a chance to lay an egg when it's time to. Returns the number of eggs laid.
        /// </summary>
        public int LayEggsIfDue(Random random)
        {
            if (DateTime.UtcNow < _nextEggsLaidAt || _chickens == 0)
            {
                return 0;
            }

            int eggsLaid = random.Next(_chickens) + 1;
            _eggs += eggsLaid;
            _nextEggsLaidAt = DateTime.UtcNow.Add(LayInterval);
            UpdateLabel();
            return eggsLaid;
        }

        public string GetLabelText()
        {
            string chickenWord = _chickens == 1 ? "chicken" : "chickens";
            string eggWord = _eggs == 1 ? "egg" : "eggs";
            string verbToBe = _chickens == 1 ? "is" : "are";

            return $"** There {verbToBe} {_chickens} {chickenWord} with {_eggs} {eggWord} in the chicken coop. **\n\n" +
                $"{ChatColor.LightGray}(( Use /collectegg to collect an egg. ))";
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
