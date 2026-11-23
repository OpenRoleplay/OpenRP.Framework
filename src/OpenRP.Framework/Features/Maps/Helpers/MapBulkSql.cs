using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.Framework.Features.Maps.Enums;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Helpers
{
    /// <summary>
    /// The SQL of the bulk replace: multi-row INSERTs, and the queries that read back the new ids. Table and column names
    /// come from the EF model and are quoted for the database, so they match the migrations. Every value is a parameter.
    /// </summary>
    public sealed class MapBulkSql
    {
        /// <summary>
        /// Rows per INSERT. The widest row has 17 columns, so 1,000 rows use 17,000 placeholders, well under MySQL's limit
        /// of 65,535 per statement.
        /// </summary>
        public const int RowsPerStatement = 1000;

        private const int MaxPlaceholdersPerStatement = 65535;

        // The order of these is the order in which AddObjectRows adds the values.
        private static readonly string[] ObjectProperties =
        {
            nameof(MapObjectModel.MapId), nameof(MapObjectModel.ModelId), nameof(MapObjectModel.X), nameof(MapObjectModel.Y),
            nameof(MapObjectModel.Z), nameof(MapObjectModel.RotX), nameof(MapObjectModel.RotY), nameof(MapObjectModel.RotZ),
            nameof(MapObjectModel.VirtualWorld), nameof(MapObjectModel.Interior), nameof(MapObjectModel.Description),
            nameof(MapObjectModel.StreamDistance), nameof(MapObjectModel.CreatedOn), nameof(MapObjectModel.UpdatedOn)
        };

        // The order of these is the order in which AddMaterialRows adds the values.
        private static readonly string[] MaterialProperties =
        {
            nameof(MapObjectMaterialModel.MapObjectId), nameof(MapObjectMaterialModel.MaterialIndex), nameof(MapObjectMaterialModel.Type),
            nameof(MapObjectMaterialModel.ModelId), nameof(MapObjectMaterialModel.TxdName), nameof(MapObjectMaterialModel.TextureName),
            nameof(MapObjectMaterialModel.Color), nameof(MapObjectMaterialModel.Text), nameof(MapObjectMaterialModel.MaterialSize),
            nameof(MapObjectMaterialModel.FontFace), nameof(MapObjectMaterialModel.FontSize), nameof(MapObjectMaterialModel.Bold),
            nameof(MapObjectMaterialModel.FontColor), nameof(MapObjectMaterialModel.BackColor), nameof(MapObjectMaterialModel.Alignment),
            nameof(MapObjectMaterialModel.CreatedOn), nameof(MapObjectMaterialModel.UpdatedOn)
        };

        private readonly string _objectTable;
        private readonly string[] _objectColumns;
        private readonly string _materialTable;
        private readonly string[] _materialColumns;
        private readonly string _selectObjectIdsSql;
        private readonly string _selectMaterialIdsSql;

        public MapBulkSql(BaseDataContext db)
        {
            ISqlGenerationHelper sqlHelper = db.GetService<ISqlGenerationHelper>();
            IEntityType objectType = GetEntityType(db, typeof(MapObjectModel));
            IEntityType materialType = GetEntityType(db, typeof(MapObjectMaterialModel));

            _objectTable = QuoteTable(objectType, sqlHelper);
            _objectColumns = QuoteColumns(objectType, ObjectProperties, sqlHelper);
            _materialTable = QuoteTable(materialType, sqlHelper);
            _materialColumns = QuoteColumns(materialType, MaterialProperties, sqlHelper);

            string objectId = QuoteColumn(objectType, nameof(MapObjectModel.Id), sqlHelper);
            string objectMapId = QuoteColumn(objectType, nameof(MapObjectModel.MapId), sqlHelper);
            string materialId = QuoteColumn(materialType, nameof(MapObjectMaterialModel.Id), sqlHelper);
            string materialObjectId = QuoteColumn(materialType, nameof(MapObjectMaterialModel.MapObjectId), sqlHelper);
            string materialIndex = QuoteColumn(materialType, nameof(MapObjectMaterialModel.MaterialIndex), sqlHelper);

            _selectObjectIdsSql = $"SELECT {objectId} FROM {_objectTable} WHERE {objectMapId} = @mapId ORDER BY {objectId}";
            _selectMaterialIdsSql = $"SELECT m.{materialId}, m.{materialObjectId}, m.{materialIndex} FROM {_materialTable} AS m "
                + $"INNER JOIN {_objectTable} AS o ON o.{objectId} = m.{materialObjectId} "
                + $"WHERE o.{objectMapId} = @mapId ORDER BY m.{materialId}";
        }

        /// <summary>
        /// The INSERT for this many objects. The values are added with <see cref="AddObjectRows"/>.
        /// </summary>
        public string InsertObjectsSql(int rowCount)
        {
            return BuildInsert(_objectTable, _objectColumns, rowCount);
        }

        /// <summary>
        /// The INSERT for this many materials. The values are added with <see cref="AddMaterialRows"/>.
        /// </summary>
        public string InsertMaterialsSql(int rowCount)
        {
            return BuildInsert(_materialTable, _materialColumns, rowCount);
        }

        /// <summary>
        /// The ids of the map's objects, in insert order. Takes the map id as <c>@mapId</c>, see <see cref="AddMapIdParameter"/>.
        /// </summary>
        public string SelectObjectIdsSql => _selectObjectIdsSql;

        /// <summary>
        /// The id, object id and material index of the map's materials, in insert order. Takes <c>@mapId</c> too.
        /// </summary>
        public string SelectMaterialIdsSql => _selectMaterialIdsSql;

        public static void AddMapIdParameter(DbCommand command, ulong mapId)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@mapId";
            parameter.Value = mapId;
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Adds the values of the objects from start up to start + count, in the order of the object columns.
        /// </summary>
        public static void AddObjectRows(DbCommand command, ulong mapId, IReadOnlyList<MapObjectSpec> specs, int start, int count, DateTime now)
        {
            for (int index = start; index < start + count; index++)
            {
                MapObjectSpec spec = specs[index];
                AddParameter(command, mapId);
                AddParameter(command, spec.ModelId);
                AddParameter(command, spec.Position.X);
                AddParameter(command, spec.Position.Y);
                AddParameter(command, spec.Position.Z);
                AddParameter(command, spec.Rotation.X);
                AddParameter(command, spec.Rotation.Y);
                AddParameter(command, spec.Rotation.Z);
                AddParameter(command, spec.VirtualWorld);
                AddParameter(command, spec.Interior);
                AddParameter(command, spec.Description);
                AddParameter(command, spec.StreamDistance);
                AddParameter(command, now);
                AddParameter(command, now);
            }
        }

        /// <summary>
        /// Adds the values of the materials from start up to start + count, in the order of the material columns. A texture
        /// has no text, so the text columns get their neutral values.
        /// </summary>
        public static void AddMaterialRows(DbCommand command, IReadOnlyList<(ulong ObjectId, MapObjectTextureSpec Texture)> rows,
            int start, int count, DateTime now)
        {
            for (int index = start; index < start + count; index++)
            {
                (ulong objectId, MapObjectTextureSpec texture) = rows[index];
                AddParameter(command, objectId);
                AddParameter(command, texture.MaterialIndex);
                AddParameter(command, (int)MapObjectMaterialType.Texture);
                AddParameter(command, texture.ModelId);
                AddParameter(command, texture.TxdName);
                AddParameter(command, texture.TextureName);
                AddParameter(command, texture.ArgbColor);
                AddParameter(command, null);
                AddParameter(command, 0);
                AddParameter(command, null);
                AddParameter(command, 0);
                AddParameter(command, false);
                AddParameter(command, 0u);
                AddParameter(command, 0u);
                AddParameter(command, 0);
                AddParameter(command, now);
                AddParameter(command, now);
            }
        }

        private static string BuildInsert(string table, string[] columns, int rowCount)
        {
            int placeholderCount = rowCount * columns.Length;
            if (placeholderCount > MaxPlaceholdersPerStatement)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), $"{placeholderCount} placeholders are more than one statement can take.");
            }

            List<string> rows = new List<string>(rowCount);
            for (int row = 0; row < rowCount; row++)
            {
                IEnumerable<string> placeholders = Enumerable.Range(row * columns.Length, columns.Length).Select(ParameterName);
                rows.Add("(" + string.Join(", ", placeholders) + ")");
            }
            return $"INSERT INTO {table} ({string.Join(", ", columns)}) VALUES {string.Join(", ", rows)}";
        }

        /// <summary>
        /// The parameters are numbered in the order they're added, which is the order the INSERT names them in.
        /// </summary>
        private static void AddParameter(DbCommand command, object? value)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = ParameterName(command.Parameters.Count);
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        private static string ParameterName(int index)
        {
            return "@p" + index.ToString(CultureInfo.InvariantCulture);
        }

        private static IEntityType GetEntityType(BaseDataContext db, Type clrType)
        {
            return db.Model.FindEntityType(clrType) ?? throw new InvalidOperationException($"{clrType.Name} isn't in the database model.");
        }

        private static string QuoteTable(IEntityType entityType, ISqlGenerationHelper sqlHelper)
        {
            string tableName = entityType.GetTableName() ?? throw new InvalidOperationException($"{entityType.Name} has no table.");
            return sqlHelper.DelimitIdentifier(tableName, entityType.GetSchema());
        }

        private static string[] QuoteColumns(IEntityType entityType, IEnumerable<string> propertyNames, ISqlGenerationHelper sqlHelper)
        {
            return propertyNames.Select(propertyName => QuoteColumn(entityType, propertyName, sqlHelper)).ToArray();
        }

        private static string QuoteColumn(IEntityType entityType, string propertyName, ISqlGenerationHelper sqlHelper)
        {
            IProperty property = entityType.FindProperty(propertyName)
                ?? throw new InvalidOperationException($"{entityType.Name} has no property {propertyName}.");
            string tableName = entityType.GetTableName() ?? throw new InvalidOperationException($"{entityType.Name} has no table.");
            string columnName = property.GetColumnName(StoreObjectIdentifier.Table(tableName, entityType.GetSchema()))
                ?? throw new InvalidOperationException($"{entityType.Name}.{propertyName} has no column.");
            return sqlHelper.DelimitIdentifier(columnName);
        }
    }
}
