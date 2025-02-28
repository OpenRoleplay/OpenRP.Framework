using OpenRP.Framework.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class PermissionGroupPermissionModel : BaseModel
    {
        public ulong PermissionGroupId { get; set; }
        public ulong PermissionId { get; set; }
        public bool GamemodeManaged { get; set; }


        // Navigational Properties
        public PermissionGroupModel PermissionGroup { get; set; }
        public PermissionModel Permission { get; set; }
    }
}
