using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRP.Framework.Features.Properties.Helpers
{
    /// <summary>
    /// Builds the PropertyDefaultData and PropertyDoorDefaultData classes, in the same form as the other DefaultData
    /// classes, so properties built in-game can be shipped as seed data. The door links go in PropertyDoorLinks
    /// instead, because linked doors form cycles that EF can't insert as seed data.
    /// </summary>
    public static class PropertyDefaultDataFiles
    {
        public const string PropertyClassName = "PropertyDefaultData";
        public const string PropertyDoorClassName = "PropertyDoorDefaultData";
        public const string PropertyDoorLinksClassName = "PropertyDoorLinks";
        private const string SetLinksMethodName = "SetLinks";

        private const string RowIndentation = "                ";
        private const string FieldIndentation = "                    ";
        private const string StatementIndentation = "            ";
        private const string ArgumentIndentation = "                ";

        public static string CreatePropertyDefaultData(IEnumerable<PropertyModel> properties, string namespaceName, DateTime generatedAt)
        {
            IEnumerable<string> rows = properties.OrderBy(property => property.Id).Select(CreatePropertyRow);
            return CreateDefaultDataClass(namespaceName, PropertyClassName, nameof(PropertyModel), rows, CreateHeader(generatedAt));
        }

        public static string CreatePropertyDoorDefaultData(IEnumerable<PropertyDoorModel> doors, string namespaceName, DateTime generatedAt)
        {
            IEnumerable<string> rows = doors.OrderBy(door => door.Id).Select(CreatePropertyDoorRow);
            string header = CreateHeader(generatedAt) + $"// The doors start unlinked; {PropertyDoorLinksClassName}.cs sets their links.{Environment.NewLine}";
            return CreateDefaultDataClass(namespaceName, PropertyDoorClassName, nameof(PropertyDoorModel), rows, header);
        }

        public static string CreatePropertyDoorLinks(IEnumerable<PropertyDoorModel> doors, string namespaceName, DateTime generatedAt)
        {
            IEnumerable<string> lines = doors
                .Where(door => door.LinkedToPropertyDoorId.HasValue)
                .OrderBy(door => door.Id)
                .Select(CreateLinkLine);

            StringBuilder builder = new StringBuilder();
            builder.Append(CreateHeader(generatedAt));
            builder.AppendLine("// Seed data can't hold the door links (linked doors form cycles that EF can't insert), so they are set");
            builder.AppendLine($"// afterwards. Copy the lines of {SetLinksMethodName} into the Up() of the migration that seeds {PropertyDoorClassName},");
            builder.AppendLine("// after its InsertData calls. Don't call this class from a migration: an applied migration must never change,");
            builder.AppendLine("// and this file is regenerated.");
            builder.AppendLine("using Microsoft.EntityFrameworkCore.Migrations;");
            builder.AppendLine();
            builder.AppendLine($"namespace {namespaceName}");
            builder.AppendLine("{");
            builder.AppendLine($"    public static class {PropertyDoorLinksClassName}");
            builder.AppendLine("    {");
            builder.AppendLine($"        public static void {SetLinksMethodName}(MigrationBuilder migrationBuilder)");
            builder.AppendLine("        {");
            foreach (string line in lines)
            {
                builder.AppendLine(line);
            }
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            return builder.ToString();
        }

        private static string CreatePropertyRow(PropertyModel property)
        {
            return CreateRow(nameof(PropertyModel), new List<(string, string)>
            {
                (nameof(PropertyModel.Id), CSharpLiterals.GetULong(property.Id)),
                (nameof(PropertyModel.Name), CSharpLiterals.GetString(property.Name)),
            });
        }

        private static string CreatePropertyDoorRow(PropertyDoorModel door)
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
            });
        }

        private static string CreateRow(string modelName, List<(string Name, string Value)> fields)
        {
            string assignments = string.Join($",{Environment.NewLine}", fields.Select(field => $"{FieldIndentation}{field.Name} = {field.Value}"));
            return $"{RowIndentation}new {modelName}(){Environment.NewLine}{RowIndentation}{{{Environment.NewLine}{assignments}{Environment.NewLine}{RowIndentation}}}";
        }

        private static string CreateLinkLine(PropertyDoorModel door)
        {
            string[] arguments =
            {
                "table: \"PropertyDoors\"",
                "keyColumn: \"Id\"",
                $"keyValue: {CSharpLiterals.GetULong(door.Id, withSuffix: true)}",
                $"column: \"{nameof(PropertyDoorModel.LinkedToPropertyDoorId)}\"",
                $"value: {CSharpLiterals.GetULong(door.LinkedToPropertyDoorId, withSuffix: true)}",
            };
            string argumentList = string.Join($",{Environment.NewLine}", arguments.Select(argument => $"{ArgumentIndentation}{argument}"));
            return $"{StatementIndentation}migrationBuilder.UpdateData({Environment.NewLine}{argumentList});";
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
