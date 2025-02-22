using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    public class InventoryModel : BaseEntity
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Max weight in grams
        /// </summary>
        public uint? MaxWeight { get; set; }

        // Navigational Properties
        public List<InventoryItemModel> Items { get; set; }
    }
}