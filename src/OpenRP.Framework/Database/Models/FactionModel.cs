using OpenRP.Framework.Database.Entities;
using OpenRP.Framework.Features.Factions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class FactionModel : BaseModel
    {
        public string Name { get; set; }
        public ulong? ParentFactionId { get; set; }
        public FactionType Type { get; set; }

        // Navigational Properties
        public FactionModel ParentFaction { get; set; }
        public ICollection<FactionModel> ChildFactions { get; set; }

        public FactionModel()
        {
            ChildFactions = new List<FactionModel>();
        }
    }
}
