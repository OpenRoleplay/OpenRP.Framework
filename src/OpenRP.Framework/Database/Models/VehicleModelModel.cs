using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    public class VehicleModelModel : BaseModel
    {
        public int ModelId { get; set; }
        public string Name { get; set; }
    }
}