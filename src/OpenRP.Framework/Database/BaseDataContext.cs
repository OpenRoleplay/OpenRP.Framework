using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Entities;
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

        public override int SaveChanges()
        {
            UpdateTimestamps();
            AttachNewInventories();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            AttachNewInventories();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            // Only update entities that inherit from BaseEntity
            var entries = ChangeTracker.Entries<BaseModel>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedOn = DateTime.UtcNow;
                    entry.Entity.UpdatedOn = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedOn = DateTime.UtcNow;
                }
            }
        }

        private void AttachNewInventories()
        {
            // Loop through all newly added CharacterModel entries.
            foreach (var entry in ChangeTracker.Entries<CharacterModel>().Where(e => e.State == EntityState.Added))
            {
                if (entry.Entity.Inventory == null)
                {
                    // Create a new InventoryModel and associate it.
                    var newInventory = new InventoryModel()
                    {
                        Name = $"{entry.Entity.FirstName} {entry.Entity.LastName}`s Inventory",
                        MaxWeight = 10000
                    };
                    entry.Entity.Inventory = newInventory;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure all entities that inherit from BaseModel
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(BaseModel).IsAssignableFrom(t.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasKey(nameof(BaseModel.Id));

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseModel.Id))
                    .ValueGeneratedOnAdd();
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
