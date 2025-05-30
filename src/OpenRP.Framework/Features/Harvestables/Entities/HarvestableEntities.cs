using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Harvestables.Entities
{
    public static class HarvestableEntities
    {
        // Id Registry
        private static int IndianHempId = int.MinValue;

        // Entity Types
        [EntityType]
        public static readonly Guid IndianHempType = new Guid("F7AC9D99-47F4-4C10-939D-C0A2183F2417");

        // Generate Methods
        public static EntityId GenerateIndianHempId()
        {
            EntityId newEntityId = new EntityId(IndianHempType, IndianHempId);

            // Atomically decrement the counter to ensure thread-safety.
            int newId = Interlocked.Increment(ref IndianHempId);

            return newEntityId;
        }
    }
}
