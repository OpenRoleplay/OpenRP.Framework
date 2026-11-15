using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Entities;
using SampSharp.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Components
{
    /// <summary>
    /// An object of a map: a model at a position, with its materials as child entities. It's a root entity of its own
    /// that points to its map, so a map with hundreds of thousands of objects has no huge parent. It's spawned as a
    /// streamer object while its map is loaded. Only the map store changes its state.
    /// </summary>
    public class MapObject : Component
    {
        private readonly MapObjectModel _mapObjectModel;
        private readonly EntityId _mapEntity;

        public MapObject(MapObjectModel mapObjectModel, EntityId mapEntity)
        {
            _mapObjectModel = mapObjectModel;
            _mapEntity = mapEntity;
        }

        public ulong GetId()
        {
            return _mapObjectModel.Id;
        }

        public ulong GetMapId()
        {
            return _mapObjectModel.MapId;
        }

        public int GetModelId()
        {
            return _mapObjectModel.ModelId;
        }

        internal void SetModelId(int modelId)
        {
            _mapObjectModel.ModelId = modelId;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(_mapObjectModel.X, _mapObjectModel.Y, _mapObjectModel.Z);
        }

        internal void SetPosition(Vector3 position)
        {
            _mapObjectModel.X = position.X;
            _mapObjectModel.Y = position.Y;
            _mapObjectModel.Z = position.Z;
        }

        public Vector3 GetRotation()
        {
            return new Vector3(_mapObjectModel.RotX, _mapObjectModel.RotY, _mapObjectModel.RotZ);
        }

        internal void SetRotation(Vector3 rotation)
        {
            _mapObjectModel.RotX = rotation.X;
            _mapObjectModel.RotY = rotation.Y;
            _mapObjectModel.RotZ = rotation.Z;
        }

        /// <summary>
        /// The virtual world the object is in, or -1 for every virtual world.
        /// </summary>
        public int GetVirtualWorld()
        {
            return _mapObjectModel.VirtualWorld;
        }

        internal void SetVirtualWorld(int virtualWorld)
        {
            _mapObjectModel.VirtualWorld = virtualWorld;
        }

        /// <summary>
        /// The interior the object is in, or -1 for every interior.
        /// </summary>
        public int GetInterior()
        {
            return _mapObjectModel.Interior;
        }

        internal void SetInterior(int interior)
        {
            _mapObjectModel.Interior = interior;
        }

        public string? GetDescription()
        {
            return _mapObjectModel.Description;
        }

        internal void SetDescription(string? description)
        {
            _mapObjectModel.Description = description;
        }

        /// <summary>
        /// The stream distance the object was given, or null when it uses the default.
        /// </summary>
        public float? GetStreamDistanceSetting()
        {
            return _mapObjectModel.StreamDistance;
        }

        /// <summary>
        /// The distance in metres the object is streamed in at: its own setting, or the default.
        /// </summary>
        public float GetStreamDistance()
        {
            return GetStreamDistanceSetting() ?? MapLimits.DefaultStreamDistance;
        }

        internal void SetStreamDistanceSetting(float? streamDistance)
        {
            _mapObjectModel.StreamDistance = streamDistance;
        }

        /// <summary>
        /// The map this object belongs to, or null when that map is gone.
        /// </summary>
        public Map? GetMap()
        {
            return Manager.GetComponent<Map>(_mapEntity);
        }

        /// <summary>
        /// The materials of this object.
        /// </summary>
        public List<MapObjectMaterial> GetMaterials()
        {
            return GetComponentsInChildren<MapObjectMaterial>().ToList();
        }

        /// <summary>
        /// The material in this slot, or null when the slot is empty.
        /// </summary>
        public MapObjectMaterial? GetMaterial(int materialIndex)
        {
            return GetComponentsInChildren<MapObjectMaterial>().FirstOrDefault(material => material.GetMaterialIndex() == materialIndex);
        }
    }
}
