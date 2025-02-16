using OpenRP.Framework.Features.Characters.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public interface ICharacterVehicleManager
    {
        bool HasVehicleKey(Character character, ulong vehicleId);
    }
}
