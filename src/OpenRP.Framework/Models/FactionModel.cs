using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Models
{
    public class FactionModel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public ulong? ParentFactionId { get; set; }

        // Navigational Properties
        public FactionModel ParentFaction { get; set; }
        public ICollection<FactionModel> ChildFactions { get; set; }

        public FactionModel()
        {
            ChildFactions = new List<FactionModel>();
        }
    }
}
