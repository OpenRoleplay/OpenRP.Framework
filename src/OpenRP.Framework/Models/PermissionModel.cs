using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Models
{
    public class PermissionModel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
