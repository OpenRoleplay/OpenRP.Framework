using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    public class SkillModel : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}