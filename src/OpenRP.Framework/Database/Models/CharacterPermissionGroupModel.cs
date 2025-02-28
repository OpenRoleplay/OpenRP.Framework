using OpenRP.Framework.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class CharacterPermissionGroupModel : BaseModel
    {
        public ulong CharacterId { get; set; }
        public ulong PermissionGroupId { get; set; }

        // Navigational Properties
        public CharacterModel Character { get; set; }
        public PermissionGroupModel PermissionGroup { get; set; }
    }
}
