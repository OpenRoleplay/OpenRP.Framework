namespace OpenRP.Framework.Database.Models
{
    public class VehicleModel
    {
        public ulong Id { get; set; }
        public ulong? ModelId { get; set; }
        public ulong? ModelSpawnTypeId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Rotation { get; set; }
        public int Color1 { get; set; }
        public int Color2 { get; set; }
        public bool IsEngineOn { get; set; }
        public bool AreLightsOn { get; set; }
        public bool IsAlarmOn { get; set; }
        public bool AreDoorsLocked { get; set; }
        public bool IsBonnetOpen { get; set; }
        public bool IsBootOpen { get; set; }
        public bool IsDriverDoorOpen { get; set; }
        public bool IsPassengerDoorOpen { get; set; }
        public bool IsBackLeftDoorOpen { get; set; }
        public bool IsBackRightDoorOpen { get; set; }
        public bool IsDriverWindowClosed { get; set; }
        public bool IsPassengerWindowClosed { get; set; }
        public bool IsBackLeftWindowClosed { get; set; }
        public bool IsBackRightWindowClosed { get; set; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
        public bool ServerVehicle { get; set; }
        public ulong? OwnerCharacterId { get; set; }
        public int? LoadedAs { get; set; }

        // Navigational Properties
        public CharacterModel? OwnerCharacter { get; set; }
        public VehicleModelSpawnTypeModel? ModelSpawnType { get; set; }
        public VehicleModelModel? Model { get; set; }

        // Constructor
        public VehicleModel()
        {
            ModelId = null;
            ModelSpawnTypeId = null;
            Color1 = 1;
            Color2 = 1;
            IsEngineOn = false;
            AreLightsOn = false;
            IsAlarmOn = false;
            AreDoorsLocked = false;
            IsBonnetOpen = false;
            IsBootOpen = false;
            IsDriverDoorOpen = false;
            IsPassengerDoorOpen = false;
            IsBackLeftDoorOpen = false;
            IsBackRightDoorOpen = false;
            IsDriverWindowClosed = false;
            IsPassengerWindowClosed = false;
            IsBackLeftWindowClosed = false;
            IsBackRightWindowClosed = false;
            CurrentHealth = 1000;
            MaxHealth = 1000;
            ServerVehicle = false;
            OwnerCharacter = null;
            OwnerCharacterId = null;
            LoadedAs = null;
        }
    }
}