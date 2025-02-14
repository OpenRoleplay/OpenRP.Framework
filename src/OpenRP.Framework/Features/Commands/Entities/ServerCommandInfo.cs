using OpenRP.Framework.Features.Commands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Commands.Entities
{
    public class ServerCommandInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] GroupPath { get; set; }
        public MethodInfo Method { get; set; }
        public ServerCommandAttribute Attribute { get; set; }
    }
}
