namespace OpenRP.Framework.Features.Fishing.Entities
{
    /// <summary>
    /// The tunable numbers of fishing, in one place.
    /// </summary>
    public static class FishingSettings
    {
        // The Skills row the host seeds for fishing.
        public const string SkillName = "Fishing";

        // Items (see ItemModelDefaultData).
        public const int FishingRodItemId = 52;
        public const int FishingBaitItemId = 53;
        public const int FishingNetItemId = 54;

        // Rod: where the player can fish from.
        public const float FacingWaterDistance = 3f;
        // Long enough to reach the water from a high pier.
        public const float FacingWaterRayLength = 15f;
        public const float BoatWaterRayLength = 15f;
        public const float MaxMoveDistanceOnLand = 2f;
        // A boat drifts on the waves, so it gets more room than someone standing on a pier.
        public const float MaxMoveDistanceOnBoat = 8f;

        // Rod: timing.
        public const int MinBiteWaitInSeconds = 20;
        public const int MaxBiteWaitInSeconds = 45;
        public const float HotspotBiteWaitMultiplier = 0.5f;
        public const int BiteWindowInSeconds = 3;
        public const int FisherUpdateIntervalInMilliseconds = 500;

        // Rod: how the rod looks in the player's hand.
        public const int RodObjectModelId = 18632;
        public const int RodAttachedObjectSlot = 9;
        public const string FishingAnimationLibrary = "SAMP";
        public const string FishingAnimationName = "FishingIdle";

        // Size gates: the fishing level a fish of each size needs.
        public const uint SmallFishRequiredLevel = 0;
        public const uint MediumFishRequiredLevel = 5;
        public const uint LargeFishRequiredLevel = 20;
        public const uint HugeFishRequiredLevel = 50;

        // Net-only fish: the fishing level each species needs.
        public const uint SwordfishRequiredLevel = 60;
        public const uint GiantSturgeonRequiredLevel = 70;
        public const uint AlligatorGarRequiredLevel = 70;
        public const uint BlueMarlinRequiredLevel = 80;

        // Treasures: the fishing level each one needs.
        public const uint OldLockboxRequiredLevel = 10;
        public const uint SilverRingRequiredLevel = 30;
        public const uint GoldPocketWatchRequiredLevel = 60;
        public const uint PirateDoubloonRequiredLevel = 90;

        // How likely each size is from land. On a boat every size keeps its full odds.
        public const float MediumFishLandOdds = 0.60f;
        public const float LargeFishLandOdds = 0.25f;
        public const float HugeFishLandOdds = 0.05f;

        // Experience per catch.
        public const uint JunkExperience = 2;
        public const uint TreasureExperience = 40;
        public const uint SmallFishExperience = 10;
        public const uint MediumFishExperience = 25;
        public const uint LargeFishExperience = 60;
        public const uint HugeFishExperience = 150;

        // Hotspots.
        public const int HotspotCount = 30;
        public const int HotspotObjectModelId = 18741;
        public const float HotspotRange = 25f;
        public const int HotspotMoveIntervalInMilliseconds = 45 * 60 * 1000;
        // Stays inside the detailed map, where ColAndreas knows the real water.
        public const float HotspotMapEdge = 2900f;
        public const float HotspotRayStartHeight = 1000f;
        public const float HotspotRayEndHeight = -10f;
        public const int HotspotMaxPlacementAttempts = 500;

        // Reefer net.
        public const uint NetStorageMaxWeightInGrams = 500000;
        public const int NetSyncIntervalInMilliseconds = 60000;
        public const int MaxNetStorageDialogRows = 50;

        // Trawling.
        public const int TrawlUpdateIntervalInMilliseconds = 1000;
        public const float TrawlDurationInSeconds = 15f * 60f;
        public const float TrawlMinSpeedInKmh = 5f;
        public const float TrawlMaxSpeedInKmh = 20f;
        public const float HotspotTrawlProgressMultiplier = 1.5f;
        // Turns GTA's velocity into km/h.
        public const float VelocityToKmh = 180f;
        public const float VehicleWaterRayStartHeight = 2f;
        public const float VehicleWaterRayLength = 12f;
        public const int TrawlWarningIntervalInSeconds = 30;
        public const int TrawlProgressMessageIntervalInSeconds = 180;

        // Net results: the chance of each, out of 100. The rest is one huge fish.
        public const int ManySmallFishChance = 60;
        public const int FewBiggerFishChance = 30;
        public const int MinManySmallFish = 8;
        public const int MaxManySmallFish = 20;
        public const int MinFewBiggerFish = 2;
        public const int MaxFewBiggerFish = 5;
    }
}
