using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Properties.Components;
using OpenRP.Framework.Features.Properties.Helpers;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenRP.Framework.Features.Properties.Services
{
    public class PropertyDefaultDataExporter : IPropertyDefaultDataExporter
    {
        // The server runs from its own folder, which holds scriptfiles.
        private static readonly string DefaultDataFolder = Path.Combine("scriptfiles", "DefaultData");
        private const string FallbackHostName = "OpenRP.GameMode";
        private const string OldLinksClassName = "PropertyDoorLinks";

        private readonly IEntityManager _entityManager;
        private readonly ILogger<PropertyDefaultDataExporter> _logger;

        public PropertyDefaultDataExporter(IEntityManager entityManager, ILogger<PropertyDefaultDataExporter> logger)
        {
            _entityManager = entityManager;
            _logger = logger;
        }

        public void ExportDefaultData()
        {
            try
            {
                List<PropertyModel> properties = GetPropertyModels();
                List<PropertyDoorModel> doors = GetPropertyDoorModels();
                string namespaceName = GetDefaultDataNamespace();
                DateTime generatedAt = DateTime.UtcNow;

                Directory.CreateDirectory(DefaultDataFolder);
                WriteDefaultDataFile(PropertyDefaultDataFiles.PropertyClassName, PropertyDefaultDataFiles.CreatePropertyDefaultData(properties, namespaceName, generatedAt));
                WriteDefaultDataFile(PropertyDefaultDataFiles.PropertyDoorClassName, PropertyDefaultDataFiles.CreatePropertyDoorDefaultData(doors, namespaceName, generatedAt));
                DeleteOldLinksFile();

                int linkCount = PropertyDefaultDataFiles.GetExportedLinks(doors).Count(link => link.Value.HasValue);
                _logger.LogInformation("Exported {PropertyCount} properties, {DoorCount} property doors and {LinkCount} door links as default data to {Folder}.", properties.Count, doors.Count, linkCount, Path.GetFullPath(DefaultDataFolder));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export the properties and property doors as default data.");
            }
        }

        private List<PropertyModel> GetPropertyModels()
        {
            return _entityManager.GetComponents<Property>()
                .Where(property => !property.IsDeleted())
                .Select(property => property.GetRawPropertyModel())
                .ToList();
        }

        private List<PropertyDoorModel> GetPropertyDoorModels()
        {
            return _entityManager.GetComponents<PropertyDoor>()
                .Where(door => !door.IsDeleted())
                .Select(door => door.GetRawPropertyDoorModel())
                .ToList();
        }

        /// <summary>
        /// The host's own namespace, so the files can be copied into the gamemode as they are.
        /// </summary>
        private static string GetDefaultDataNamespace()
        {
            string hostName = Assembly.GetEntryAssembly()?.GetName().Name ?? FallbackHostName;
            return $"{hostName}.Features.Properties.Models.DefaultData";
        }

        /// <summary>
        /// Older versions wrote the links to a separate file to paste into a migration; they're in the doors file now.
        /// </summary>
        private static void DeleteOldLinksFile()
        {
            File.Delete(Path.Combine(DefaultDataFolder, $"{OldLinksClassName}.cs"));
        }

        private static void WriteDefaultDataFile(string className, string content)
        {
            File.WriteAllText(Path.Combine(DefaultDataFolder, $"{className}.cs"), content);
        }
    }
}
