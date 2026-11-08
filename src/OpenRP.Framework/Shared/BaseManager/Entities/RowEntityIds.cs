using SampSharp.Entities;
using System;
using System.Collections.Concurrent;

namespace OpenRP.Framework.Shared.BaseManager.Entities
{
    /// <summary>
    /// The entity id of a database row: the same row always gets the same entity id while the server runs.
    /// </summary>
    /// <remarks>
    /// SampSharp 1.0 entity ids are random Guids, so a row id can't be turned into an entity id directly anymore.
    /// This keeps the old "entity id = row id" lookup working: each feature keeps its own type key (one Guid per
    /// table), and the first lookup of a row hands out a new entity id that every later lookup reuses.
    /// The ids are random and only valid while the server runs: save and share row ids, never entity ids.
    /// </remarks>
    public static class RowEntityIds
    {
        private static readonly ConcurrentDictionary<(Guid TypeKey, long RowId), EntityId> _entityIds = new();

        public static EntityId Get(Guid typeKey, long rowId)
        {
            return _entityIds.GetOrAdd((typeKey, rowId), _ => EntityId.NewEntityId());
        }

        /// <summary>
        /// Makes an existing entity the entity of a row, for a component whose row was just inserted.
        /// </summary>
        public static void Register(Guid typeKey, long rowId, EntityId entityId)
        {
            _entityIds[(typeKey, rowId)] = entityId;
        }

        /// <summary>
        /// Forgets the entity id of a deleted row.
        /// </summary>
        public static void Remove(Guid typeKey, long rowId)
        {
            _entityIds.TryRemove((typeKey, rowId), out _);
        }
    }
}
