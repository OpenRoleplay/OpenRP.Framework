using OpenRP.Framework.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class PermissionModel : BaseModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
