using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Services
{
    public class DataMemoryService : IDataMemoryService
    {
        private BaseDataContext _dataContext;
        public DataMemoryService(BaseDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // Data
        private List<ItemModel> Items { get; set; } = null;
        private List<VehicleModelModel> VehicleModels { get; set; } = null;
        private List<VehicleModelSpawnTypeModelModel> VehicleSpawnTypeModels { get; set; } = null;
        private List<VehicleModelDefaultColorModel> VehicleModelDefaultColors { get; set; } = null;
        private List<CurrencyModel> Currencies { get; set; } = null;
        private List<CurrencyUnitModel> CurrencyUnits { get; set; } = null;
        private List<FishLootModel> FishSpecies { get; set; } = null;

        public List<ItemModel> GetItems()
        {
            if (Items == null)
            {
                Items = _dataContext.Items
                    .ToList();
            }
            return Items;
        }
        public List<FishLootModel> GetFishSpecies()
        {
            if (FishSpecies == null)
            {
                FishSpecies = _dataContext.FishSpecies
                    .ToList();
            }
            return FishSpecies;
        }


        public List<VehicleModelModel> GetVehicleModels()
        {
            if (VehicleModels == null)
            {
                VehicleModels = _dataContext.VehicleModels
                    .ToList();
            }
            return VehicleModels;
        }

        public List<VehicleModelSpawnTypeModelModel> GetSpawnTypeModels()
        {
            if (VehicleSpawnTypeModels == null)
            {
                VehicleSpawnTypeModels = _dataContext.VehicleModelSpawnTypeModels
                    .ToList();
            }
            return VehicleSpawnTypeModels;
        }

        public List<VehicleModelDefaultColorModel> GetVehicleModelDefaultColors()
        {
            if (VehicleModelDefaultColors == null)
            {
                VehicleModelDefaultColors = _dataContext.VehicleModelDefaultColors
                    .Include(i => i.VehicleModel)
                    .ToList();
            }
            return VehicleModelDefaultColors;
        }

        public List<CurrencyModel> GetCurrencies()
        {
            if (Currencies == null)
            {
                Currencies = _dataContext.Currencies
                    .Include(c => c.CurrencyUnits)
                    .ToList();
            }
            return Currencies;
        }

        public List<CurrencyUnitModel> GetCurrencyUnits()
        {
            if (CurrencyUnits == null)
            {
                CurrencyUnits = GetCurrencies()
                    .SelectMany(c => c.CurrencyUnits)
                    .ToList();
            }
            return CurrencyUnits;
        }
    }
}
