using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class PermissionGroupModel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }

        // Navigational Properties
        public List<PermissionGroupPermissionModel> Permissions { get; set; }
    }
}
