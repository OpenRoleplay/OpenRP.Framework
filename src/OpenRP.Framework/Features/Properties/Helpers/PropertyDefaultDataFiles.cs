using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRP.Framework.Features.Properties.Helpers
{
    /// <summary>
    /// Builds the PropertyDefaultData and PropertyDoorDefaultData classes, in the same form as the other DefaultData
    /// classes, so properties built in-game can be shipped as seed data, door links included.
    /// </summary>
    public static class PropertyDefaultDataFiles
    {
        public const string PropertyClassName = "PropertyDefaultData";
        public const string PropertyDoorClassName = "PropertyDoorDefaultData";

        private const string RowIndentation = "                ";
        private const string FieldIndentation = "                    ";

        public static string CreatePropertyDefaultData(IEnumerable<PropertyModel> properties, string namespaceName, DateTime generatedAt)
        {
            IEnumerable<string> rows = properties.OrderBy(property => property.Id).Select(CreatePropertyRow);
            return CreateDefaultDataClass(namespaceName, PropertyClassName, nameof(PropertyModel), rows, CreateHeader(generatedAt));
        }

        public static string CreatePropertyDoorDefaultData(IEnumerable<PropertyDoorModel> doors, string namespaceName, DateTime generatedAt)
        {
            Dictionary<ulong, ulong?> exportedLinks = GetExportedLinks(doors.ToList());
            IEnumerable<string> rows = doors.OrderBy(door => door.Id).Select(door => CreatePropertyDoorRow(door, exportedLinks[door.Id]));
            string header = CreateHeader(generatedAt)
                + $"// Each pair of doors stores one link and the other door leads back, so the links fit in seed data:{Environment.NewLine}"
                + $"// EF can't insert seed rows that point at each other.{Environment.NewLine}";
            return CreateDefaultDataClass(namespaceName, PropertyDoorClassName, nameof(PropertyDoorModel), rows, header);
        }

        /// <summary>
        /// Each door's link as exported. Two doors that link to each other (stored before links went one way) keep
        /// only the lower id's link; the other door leads back to it, so they work the same.
        /// </summary>
        public static Dictionary<ulong, ulong?> GetExportedLinks(List<PropertyDoorModel> doors)
        {
            Dictionary<ulong, ulong?> linksByDoorId = doors.ToDictionary(door => door.Id, door => door.LinkedToPropertyDoorId);
            return doors.ToDictionary(door => door.Id, door => IsLinkedBackFromLowerId(door, linksByDoorId) ? null : door.LinkedToPropertyDoorId);
        }

        private static bool IsLinkedBackFromLowerId(PropertyDoorModel door, Dictionary<ulong, ulong?> linksByDoorId)
        {
            ulong? linkedDoorId = door.LinkedToPropertyDoorId;
            return linkedDoorId.HasValue
                && linkedDoorId.Value < door.Id
                && linksByDoorId.TryGetValue(linkedDoorId.Value, out ulong? linkBack)
                && linkBack == door.Id;
        }

        private static string CreatePropertyRow(PropertyModel property)
        {
            return CreateRow(nameof(PropertyModel), new List<(string, string)>
            {
                (nameof(PropertyModel.Id), CSharpLiterals.GetULong(property.Id)),
                (nameof(PropertyModel.Name), CSharpLiterals.GetString(property.Name)),
                (nameof(PropertyModel.JurisdictionId), CSharpLiterals.GetULong(property.JurisdictionId)),
            });
        }

        private static string CreatePropertyDoorRow(PropertyDoorModel door, ulong? exportedLinkedDoorId)
        {
            return CreateRow(nameof(PropertyDoorModel), new List<(string, string)>
            {
                (nameof(PropertyDoorModel.Id), CSharpLiterals.GetULong(door.Id)),
                (nameof(PropertyDoorModel.Name), CSharpLiterals.GetString(door.Name)),
                (nameof(PropertyDoorModel.PropertyId), CSharpLiterals.GetULong(door.PropertyId)),
                (nameof(PropertyDoorModel.X), CSharpLiterals.GetFloat(door.X)),
                (nameof(PropertyDoorModel.Y), CSharpLiterals.GetFloat(door.Y)),
                (nameof(PropertyDoorModel.Z), CSharpLiterals.GetFloat(door.Z)),
                (nameof(PropertyDoorModel.Angle), CSharpLiterals.GetFloat(door.Angle)),
                (nameof(PropertyDoorModel.Interior), CSharpLiterals.GetInt(door.Interior)),
                (nameof(PropertyDoorModel.InsidePropertyId), CSharpLiterals.GetULong(door.InsidePropertyId)),
                (nameof(PropertyDoorModel.LinkedToPropertyDoorId), CSharpLiterals.GetULong(exportedLinkedDoorId)),
            });
        }

        private static string CreateRow(string modelName, List<(string Name, string Value)> fields)
        {
            string assignments = string.Join($",{Environment.NewLine}", fields.Select(field => $"{FieldIndentation}{field.Name} = {field.Value}"));
            return $"{RowIndentation}new {modelName}(){Environment.NewLine}{RowIndentation}{{{Environment.NewLine}{assignments}{Environment.NewLine}{RowIndentation}}}";
        }

        private static string CreateHeader(DateTime generatedAt)
        {
            return $"// Generated by Open Roleplay on {generatedAt:yyyy-MM-dd HH:mm:ss} UTC from the database.{Environment.NewLine}"
                + $"// The server overwrites this file on every start; copy it into the gamemode to keep it.{Environment.NewLine}";
        }

        private static string CreateDefaultDataClass(string namespaceName, string className, string modelName, IEnumerable<string> rows, string header)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(header);
            builder.AppendLine("using Microsoft.EntityFrameworkCore;");
            builder.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
            builder.AppendLine("using OpenRP.Framework.Database.Extensions;");
            builder.AppendLine("using OpenRP.Framework.Database.Models;");
            builder.AppendLine();
            builder.AppendLine($"namespace {namespaceName}");
            builder.AppendLine("{");
            builder.AppendLine($"    public class {className} : IEntityTypeConfiguration<{modelName}>");
            builder.AppendLine("    {");
            builder.AppendLine($"        public void Configure(EntityTypeBuilder<{modelName}> builder)");
            builder.AppendLine("        {");
            builder.AppendLine("            builder.HasDefaultData(");
            builder.AppendLine(string.Join($",{Environment.NewLine}", rows));
            builder.AppendLine("            );");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}
