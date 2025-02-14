using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database
{
    public class BaseDataContext : DbContext
    {
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<CharacterModel> Characters { get; set; }
        public DbSet<CharacterPermissionGroupModel> CharacterPermissionGroups { get; set; }
        public DbSet<InventoryModel> Inventories { get; set; }
        public DbSet<InventoryItemModel> InventoryItems { get; set; }
        public DbSet<DroppedInventoryItemModel> DroppedInventoryItems { get; set; }
        public DbSet<ItemModel> Items { get; set; }
        public DbSet<ActorModel> Actors { get; set; }
        public DbSet<ActorPromptModel> ActorPrompts { get; set; }
        public DbSet<ActorRelationshipModel> ActorRelationships { get; set; }
        public DbSet<CurrencyModel> Currencies { get; set; }
        public DbSet<CurrencyUnitModel> CurrencyUnits { get; set; }
        public DbSet<PermissionModel> Permissions { get; set; }
        public DbSet<PermissionGroupModel> PermissionGroups { get; set; }
        public DbSet<PermissionGroupPermissionModel> PermissionGroupPermissions { get; set; }
        public DbSet<VehicleModel> Vehicles { get; set; }
        public DbSet<SkillModel> Skills { get; set; }
        public DbSet<CharacterSkillModel> CharacterSkills { get; set; }
        public DbSet<CharacterPreferencesModel> CharacterPreferences { get; set; }
        public DbSet<PropertyModel> Properties { get; set; }
        public DbSet<PropertyDoorModel> PropertyDoors { get; set; }
        public DbSet<MainMenuSceneModel> MainMenuScenes { get; set; }
        public DbSet<FactionModel> Factions { get; set; }
        public DbSet<VehicleModelModel> VehicleModels { get; set; }
        public DbSet<VehicleModelDefaultColorModel> VehicleModelDefaultColors { get; set; }
        public DbSet<VehicleModelSpawnTypeModel> VehicleModelSpawnTypes { get; set; }
        public DbSet<VehicleModelSpawnTypeModelModel> VehicleModelSpawnTypeModels { get; set; }
        public DbSet<FishLootModel> FishSpecies { get; set; }
        public DbSet<AccountSettingsModel> AccountSettings { get; set; }

        // Parameterless Constructor
        public BaseDataContext() : base() { }

        // Constructor accepting options
        public BaseDataContext(DbContextOptions options) : base(options) { }
    }
}
