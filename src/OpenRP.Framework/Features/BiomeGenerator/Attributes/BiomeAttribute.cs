using OpenRP.Framework.Features.BiomeGenerator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class BiomeAttribute : Attribute
    {
        public string Name { get; }
        public byte SpawnFrequency { get; }
        public BiomeType BiomeType { get; }

        public BiomeAttribute(byte spawnFrequency, string name, BiomeType biomeType)
        {
            Name = name;
            SpawnFrequency = spawnFrequency;
            BiomeType = biomeType;
        }
    }
}
