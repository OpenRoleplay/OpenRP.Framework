using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    public class InventoryModel : BaseModel
    {
        public string Name { get; set; }
        /// <summary>
        /// Max weight in grams
        /// </summary>
        public uint? MaxWeight { get; set; }

        // Navigational Properties
        public List<InventoryItemModel> Items { get; set; }
    }
}