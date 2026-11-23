using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.Framework.Features.Maps.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Maps.Services
{
    /// <summary>
    /// Reads and writes map rows. Each call gets a fresh database context, so nothing stays tracked between calls.
    /// </summary>
    public class MapDatabase : IMapDatabase
    {
        // Deleting a big map's materials and objects takes far longer than a normal query.
        private const int LongCommandTimeoutInSeconds = 600;

        private readonly IServiceScopeFactory _scopeFactory;

        public MapDatabase(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public MapRows LoadAllRows()
        {
            return Use(db =>
            {
                // A big map holds hundreds of thousands of objects, which takes far longer than a normal query.
                db.Database.SetCommandTimeout(LongCommandTimeoutInSeconds);
                return new MapRows(
                    db.Maps.AsNoTracking().ToList(),
                    db.MapObjects.AsNoTracking().ToList(),
                    db.MapObjectMaterials.AsNoTracking().ToList(),
                    db.MapRemovedBuildings.AsNoTracking().ToList());
            });
        }

        public MapModel InsertMap(string name)
        {
            return Use(db =>
            {
                MapModel map = new MapModel { Name = name };
                db.Maps.Add(map);
                db.SaveChanges();
                return map;
            });
        }

        public bool SaveMapName(ulong mapId, string name)
        {
            return Use(db => db.Maps.Where(map => map.Id == mapId)
                .ExecuteUpdate(setters => setters
                    .SetProperty(map => map.Name, name)
                    .SetProperty(map => map.UpdatedOn, DateTime.UtcNow)) > 0);
        }

        public bool SaveMapIsLoaded(ulong mapId, bool isLoaded)
        {
            return Use(db => db.Maps.Where(map => map.Id == mapId)
                .ExecuteUpdate(setters => setters
                    .SetProperty(map => map.IsLoaded, isLoaded)
                    .SetProperty(map => map.UpdatedOn, DateTime.UtcNow)) > 0);
        }

        public async Task DeleteMapAsync(ulong mapId)
        {
            // The awaits below don't resume on the main thread, so the database work stays off it.
            using IServiceScope scope = _scopeFactory.CreateScope();
            BaseDataContext db = scope.ServiceProvider.GetRequiredService<BaseDataContext>();
            db.Database.SetCommandTimeout(LongCommandTimeoutInSeconds);
            await db.Database.CreateExecutionStrategy()
                .ExecuteAsync(() => DeleteMapRowsAsync(db, mapId))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Children go first, so no row is left pointing at a deleted map. Each attempt of the execution strategy starts
        /// its own transaction.
        /// </summary>
        private static async Task DeleteMapRowsAsync(BaseDataContext db, ulong mapId)
        {
            await using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync().ConfigureAwait(false);
            await db.MapObjectMaterials
                .Where(material => db.MapObjects.Any(mapObject => mapObject.Id == material.MapObjectId && mapObject.MapId == mapId))
                .ExecuteDeleteAsync().ConfigureAwait(false);
            await db.MapObjects.Where(mapObject => mapObject.MapId == mapId).ExecuteDeleteAsync().ConfigureAwait(false);
            await db.MapRemovedBuildings.Where(removal => removal.MapId == mapId).ExecuteDeleteAsync().ConfigureAwait(false);
            await db.Maps.Where(map => map.Id == mapId).ExecuteDeleteAsync().ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        public MapObjectModel InsertObject(MapObjectModel model)
        {
            return Use(db =>
            {
                db.MapObjects.Add(model);
                db.SaveChanges();
                return model;
            });
        }

        public bool SaveObjectTransform(ulong mapObjectId, Vector3 position, Vector3 rotation)
        {
            return Use(db => db.MapObjects.Where(mapObject => mapObject.Id == mapObjectId)
                .ExecuteUpdate(setters => setters
                    .SetProperty(mapObject => mapObject.X, position.X)
                    .SetProperty(mapObject => mapObject.Y, position.Y)
                    .SetProperty(mapObject => mapObject.Z, position.Z)
                    .SetProperty(mapObject => mapObject.RotX, rotation.X)
                    .SetProperty(mapObject => mapObject.RotY, rotation.Y)
                    .SetProperty(mapObject => mapObject.RotZ, rotation.Z)
                    .SetProperty(mapObject => mapObject.UpdatedOn, DateTime.UtcNow)) > 0);
        }

        public bool SaveObjectModel(ulong mapObjectId, int modelId)
        {
            return Use(db => db.MapObjects.Where(mapObject => mapObject.Id == mapObjectId)
                .ExecuteUpdate(setters => setters
                    .SetProperty(mapObject => mapObject.ModelId, modelId)
                    .SetProperty(mapObject => mapObject.UpdatedOn, DateTime.UtcNow)) > 0);
        }

        public bool SaveObjectWorld(ulong mapObjectId, int virtualWorld, int interior)
        {
            return Use(db => db.MapObjects.Where(mapObject => mapObject.Id == mapObjectId)
                .ExecuteUpdate(setters => setters
                    .SetProperty(mapObject => mapObject.VirtualWorld, virtualWorld)
                    .SetProperty(mapObject => mapObject.Interior, interior)
                    .SetProperty(mapObject => mapObject.UpdatedOn, DateTime.UtcNow)) > 0);
        }

        public bool SaveObjectDescription(ulong mapObjectId, string? description)
        {
            return Use(db => db.MapObjects.Where(mapObject => mapObject.Id == mapObjectId)
                .ExecuteUpdate(setters => setters
                    .SetProperty(mapObject => mapObject.Description, description)
                    .SetProperty(mapObject => mapObject.UpdatedOn, DateTime.UtcNow)) > 0);
        }

        public bool SaveObjectStreamDistance(ulong mapObjectId, float? streamDistance)
        {
            return Use(db => db.MapObjects.Where(mapObject => mapObject.Id == mapObjectId)
                .ExecuteUpdate(setters => setters
                    .SetProperty(mapObject => mapObject.StreamDistance, streamDistance)
                    .SetProperty(mapObject => mapObject.UpdatedOn, DateTime.UtcNow)) > 0);
        }

        public bool DeleteObject(ulong mapObjectId)
        {
            return Use(db => db.MapObjects.Where(mapObject => mapObject.Id == mapObjectId).ExecuteDelete() > 0);
        }

        public MapObjectMaterialModel InsertMaterial(MapObjectMaterialModel material)
        {
            return Use(db =>
            {
                db.MapObjectMaterials.Add(material);
                db.SaveChanges();
                return material;
            });
        }

        public bool SaveMaterialContent(ulong materialId, MapObjectMaterialModel content)
        {
            return Use(db => db.MapObjectMaterials.Where(material => material.Id == materialId)
                .ExecuteUpdate(setters => setters
                    .SetProperty(material => material.Type, content.Type)
                    .SetProperty(material => material.ModelId, content.ModelId)
                    .SetProperty(material => material.TxdName, content.TxdName)
                    .SetProperty(material => material.TextureName, content.TextureName)
                    .SetProperty(material => material.Color, content.Color)
                    .SetProperty(material => material.Text, content.Text)
                    .SetProperty(material => material.MaterialSize, content.MaterialSize)
                    .SetProperty(material => material.FontFace, content.FontFace)
                    .SetProperty(material => material.FontSize, content.FontSize)
                    .SetProperty(material => material.Bold, content.Bold)
                    .SetProperty(material => material.FontColor, content.FontColor)
                    .SetProperty(material => material.BackColor, content.BackColor)
                    .SetProperty(material => material.Alignment, content.Alignment)
                    .SetProperty(material => material.UpdatedOn, DateTime.UtcNow)) > 0);
        }

        public bool DeleteMaterial(ulong materialId)
        {
            return Use(db => db.MapObjectMaterials.Where(material => material.Id == materialId).ExecuteDelete() > 0);
        }

        public MapRemovedBuildingModel InsertRemovedBuilding(MapRemovedBuildingModel removal)
        {
            return Use(db =>
            {
                db.MapRemovedBuildings.Add(removal);
                db.SaveChanges();
                return removal;
            });
        }

        public bool DeleteRemovedBuilding(ulong removalId)
        {
            return Use(db => db.MapRemovedBuildings.Where(removal => removal.Id == removalId).ExecuteDelete() > 0);
        }

        /// <summary>
        /// Swaps the map's objects for the given ones, in one transaction that is rolled back when any step fails. Each attempt
        /// of the execution strategy starts its own transaction, so a retry never sees the rows of a failed attempt.
        /// </summary>
        public MapBulkRowIds ReplaceObjectRows(ulong mapId, IReadOnlyList<MapObjectSpec> specs, IProgress<MapBulkProgress>? progress)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            BaseDataContext db = scope.ServiceProvider.GetRequiredService<BaseDataContext>();
            db.Database.SetCommandTimeout(LongCommandTimeoutInSeconds);
            return db.Database.CreateExecutionStrategy().Execute(() => WriteReplacement(db, mapId, specs, progress));
        }

        private static MapBulkRowIds WriteReplacement(BaseDataContext db, ulong mapId, IReadOnlyList<MapObjectSpec> specs,
            IProgress<MapBulkProgress>? progress)
        {
            using IDbContextTransaction transaction = db.Database.BeginTransaction();
            DeleteMapRows(db, mapId);
            MapBulkSql sql = new MapBulkSql(db);
            DateTime now = DateTime.UtcNow;
            InsertObjects(db, transaction, sql, mapId, specs, now, progress);
            List<ulong> objectIds = ReadObjectIds(db, transaction, sql, mapId, specs.Count);
            List<(ulong ObjectId, MapObjectTextureSpec Texture)> materialRows = CreateMaterialRows(specs, objectIds);
            InsertMaterials(db, transaction, sql, materialRows, now, progress);
            List<ulong> materialIds = ReadMaterialIds(db, transaction, sql, mapId, materialRows);
            SetMapLoaded(db, mapId, now);
            transaction.Commit();
            return new MapBulkRowIds(objectIds, materialIds);
        }

        /// <summary>
        /// Materials go first, while their objects still exist to find them. The materials' cascade would do the same.
        /// </summary>
        private static void DeleteMapRows(BaseDataContext db, ulong mapId)
        {
            db.MapObjectMaterials
                .Where(material => db.MapObjects.Any(mapObject => mapObject.Id == material.MapObjectId && mapObject.MapId == mapId))
                .ExecuteDelete();
            db.MapObjects.Where(mapObject => mapObject.MapId == mapId).ExecuteDelete();
        }

        private static void InsertObjects(BaseDataContext db, IDbContextTransaction transaction, MapBulkSql sql, ulong mapId,
            IReadOnlyList<MapObjectSpec> specs, DateTime now, IProgress<MapBulkProgress>? progress)
        {
            for (int start = 0; start < specs.Count; start += MapBulkSql.RowsPerStatement)
            {
                int count = Math.Min(MapBulkSql.RowsPerStatement, specs.Count - start);
                using DbCommand command = CreateCommand(db, transaction, sql.InsertObjectsSql(count));
                MapBulkSql.AddObjectRows(command, mapId, specs, start, count, now);
                command.ExecuteNonQuery();
                progress?.Report(new MapBulkProgress(MapBulkProgress.SavingObjects, start + count, specs.Count));
            }
        }

        /// <summary>
        /// The new object ids, in insert order. An object count that differs means the rows aren't the ones inserted.
        /// </summary>
        private static List<ulong> ReadObjectIds(BaseDataContext db, IDbContextTransaction transaction, MapBulkSql sql, ulong mapId,
            int expectedCount)
        {
            List<ulong> ids = new List<ulong>(expectedCount);
            using DbCommand command = CreateCommand(db, transaction, sql.SelectObjectIdsSql);
            MapBulkSql.AddMapIdParameter(command, mapId);
            using DbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ids.Add(Convert.ToUInt64(reader.GetValue(0), CultureInfo.InvariantCulture));
            }
            if (ids.Count != expectedCount)
            {
                throw new InvalidOperationException($"Expected {expectedCount} objects after the insert, found {ids.Count}.");
            }
            return ids;
        }

        /// <summary>
        /// Every material with its object's id, in the order of the objects and their textures.
        /// </summary>
        private static List<(ulong ObjectId, MapObjectTextureSpec Texture)> CreateMaterialRows(IReadOnlyList<MapObjectSpec> specs,
            IReadOnlyList<ulong> objectIds)
        {
            List<(ulong ObjectId, MapObjectTextureSpec Texture)> rows = new List<(ulong ObjectId, MapObjectTextureSpec Texture)>();
            for (int index = 0; index < specs.Count; index++)
            {
                foreach (MapObjectTextureSpec texture in specs[index].Textures ?? Array.Empty<MapObjectTextureSpec>())
                {
                    rows.Add((objectIds[index], texture));
                }
            }
            return rows;
        }

        private static void InsertMaterials(BaseDataContext db, IDbContextTransaction transaction, MapBulkSql sql,
            IReadOnlyList<(ulong ObjectId, MapObjectTextureSpec Texture)> rows, DateTime now, IProgress<MapBulkProgress>? progress)
        {
            for (int start = 0; start < rows.Count; start += MapBulkSql.RowsPerStatement)
            {
                int count = Math.Min(MapBulkSql.RowsPerStatement, rows.Count - start);
                using DbCommand command = CreateCommand(db, transaction, sql.InsertMaterialsSql(count));
                MapBulkSql.AddMaterialRows(command, rows, start, count, now);
                command.ExecuteNonQuery();
                progress?.Report(new MapBulkProgress(MapBulkProgress.SavingTextures, start + count, rows.Count));
            }
        }

        /// <summary>
        /// Reads the material ids back, and checks each one against the row it was inserted for. A difference means the
        /// database didn't keep the insert order, so the transaction is rolled back rather than guessing.
        /// </summary>
        private static List<ulong> ReadMaterialIds(BaseDataContext db, IDbContextTransaction transaction, MapBulkSql sql, ulong mapId,
            IReadOnlyList<(ulong ObjectId, MapObjectTextureSpec Texture)> rows)
        {
            List<ulong> ids = new List<ulong>(rows.Count);
            using DbCommand command = CreateCommand(db, transaction, sql.SelectMaterialIdsSql);
            MapBulkSql.AddMapIdParameter(command, mapId);
            using DbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                CheckMaterialRow(reader, rows, ids.Count);
                ids.Add(Convert.ToUInt64(reader.GetValue(0), CultureInfo.InvariantCulture));
            }
            if (ids.Count != rows.Count)
            {
                throw new InvalidOperationException($"Expected {rows.Count} materials after the insert, found {ids.Count}.");
            }
            return ids;
        }

        private static void CheckMaterialRow(DbDataReader reader, IReadOnlyList<(ulong ObjectId, MapObjectTextureSpec Texture)> rows,
            int index)
        {
            if (index >= rows.Count)
            {
                throw new InvalidOperationException($"The database holds more than the {rows.Count} materials that were inserted.");
            }
            ulong objectId = Convert.ToUInt64(reader.GetValue(1), CultureInfo.InvariantCulture);
            int materialIndex = Convert.ToInt32(reader.GetValue(2), CultureInfo.InvariantCulture);
            (ulong expectedObjectId, MapObjectTextureSpec texture) = rows[index];
            if (objectId != expectedObjectId || materialIndex != texture.MaterialIndex)
            {
                throw new InvalidOperationException($"Material {index + 1} came back out of insert order.");
            }
        }

        private static void SetMapLoaded(BaseDataContext db, ulong mapId, DateTime now)
        {
            int updated = db.Maps.Where(map => map.Id == mapId)
                .ExecuteUpdate(setters => setters
                    .SetProperty(map => map.IsLoaded, true)
                    .SetProperty(map => map.UpdatedOn, now));
            if (updated == 0)
            {
                throw new MapRowMissingException("map");
            }
        }

        private static DbCommand CreateCommand(BaseDataContext db, IDbContextTransaction transaction, string commandText)
        {
            DbCommand command = db.Database.GetDbConnection().CreateCommand();
            command.Transaction = transaction.GetDbTransaction();
            command.CommandTimeout = LongCommandTimeoutInSeconds;
            command.CommandText = commandText;
            return command;
        }

        private T Use<T>(Func<BaseDataContext, T> work)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            return work(scope.ServiceProvider.GetRequiredService<BaseDataContext>());
        }

        private void Use(Action<BaseDataContext> work)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            work(scope.ServiceProvider.GetRequiredService<BaseDataContext>());
        }
    }
}
