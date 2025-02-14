using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Commands.Entities
{
    public class CommandGroupNode
    {
        public string Name { get; set; }
        public List<CommandGroupNode> Subgroups { get; } = new();
        public List<ServerCommandInfo> Commands { get; } = new();
    }
}
