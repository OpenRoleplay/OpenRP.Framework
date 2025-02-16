using SampSharp.Entities.SAMP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Commands.Attributes
{
    /// <summary>
    /// An attribute which indicates the method is invokable as a player command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ServerCommandAttribute : PlayerCommandAttribute, ICommandMethodInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerCommandAttribute"/> class.
        /// </summary>
        public ServerCommandAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerCommandAttribute"/> class.
        /// </summary>
        public ServerCommandAttribute(string[] PermissionGroups, string Description)
        {
            this.PermissionGroups = PermissionGroups;
            this.Description = Description;
            this.CommandGroups = new string[1] { "Misc" };
        }

        public string[] CommandGroups { get; set; }
        public string[] PermissionGroups { get; set; }
        public string Description { get; set; }
    }
}
