using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Services
{
    public interface IDataMemoryService
    {
        List<ItemModel> GetItems();
        List<FishLootModel> GetFishSpecies();
        List<VehicleModelModel> GetVehicleModels();
        List<VehicleModelSpawnTypeModelModel> GetSpawnTypeModels();
        List<VehicleModelDefaultColorModel> GetVehicleModelDefaultColors();
        List<CurrencyModel> GetCurrencies();
        List<CurrencyUnitModel> GetCurrencyUnits();
    }
}
