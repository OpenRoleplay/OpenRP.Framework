namespace OpenRP.Framework.Features.Vehicles.Entities
{
    /// <summary>
    /// The permissions of the vehicles feature. The admin permission is given to the Admin group at startup.
    /// </summary>
    public static class VehiclePermissions
    {
        /// <summary>
        /// Lets the character lock, unlock and park any vehicle, with or without a key. Admins also use it to count as admins.
        /// </summary>
        public const string Admin = "vehicles.admin";

        public const string AdminDescription = "Lock, unlock and park any vehicle, whether or not you have its key.";
    }
}
