namespace OpenRP.Framework.Features.Properties.Services
{
    public interface IPropertyDefaultDataExporter
    {
        /// <summary>
        /// Writes the loaded properties and doors to scriptfiles/DefaultData as PropertyDefaultData and
        /// PropertyDoorDefaultData classes, and the door links as PropertyDoorLinks, replacing the previous files.
        /// </summary>
        void ExportDefaultData();
    }
}
