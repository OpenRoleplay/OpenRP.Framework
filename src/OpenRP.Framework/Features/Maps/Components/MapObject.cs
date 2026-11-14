using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Components
{
    /// <summary>
    /// An object of a map: a model at a position, with its materials as child entities. It's spawned as a streamer
    /// object while its map is loaded.
    /// </summary>
    public class MapObject : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly MapObjectModel _mapObjectModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public MapObject(MapObjectModel mapObjectModel)
        {
            _mapObjectModel = mapObjectModel;
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        public bool IsDeleted()
        {
            return _isDeleted;
        }

        public void ProcessDeletion(bool processDeletion = true)
        {
            _isDeleted = processDeletion;
        }

        public MapObjectModel GetRawMapObjectModel()
        {
            return _mapObjectModel;
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

        public void SetModelId(int modelId)
        {
            _mapObjectModel.ModelId = modelId;
            ProcessChanges();
        }

        public Vector3 GetPosition()
        {
            return new Vector3(_mapObjectModel.X, _mapObjectModel.Y, _mapObjectModel.Z);
        }

        public void SetPosition(Vector3 position)
        {
            _mapObjectModel.X = position.X;
            _mapObjectModel.Y = position.Y;
            _mapObjectModel.Z = position.Z;
            ProcessChanges();
        }

        public Vector3 GetRotation()
        {
            return new Vector3(_mapObjectModel.RotX, _mapObjectModel.RotY, _mapObjectModel.RotZ);
        }

        public void SetRotation(Vector3 rotation)
        {
            _mapObjectModel.RotX = rotation.X;
            _mapObjectModel.RotY = rotation.Y;
            _mapObjectModel.RotZ = rotation.Z;
            ProcessChanges();
        }

        /// <summary>
        /// The virtual world the object is in, or -1 for every virtual world.
        /// </summary>
        public int GetVirtualWorld()
        {
            return _mapObjectModel.VirtualWorld;
        }

        public void SetVirtualWorld(int virtualWorld)
        {
            _mapObjectModel.VirtualWorld = virtualWorld;
            ProcessChanges();
        }

        /// <summary>
        /// The interior the object is in, or -1 for every interior.
        /// </summary>
        public int GetInterior()
        {
            return _mapObjectModel.Interior;
        }

        public void SetInterior(int interior)
        {
            _mapObjectModel.Interior = interior;
            ProcessChanges();
        }

        public string? GetDescription()
        {
            return _mapObjectModel.Description;
        }

        public void SetDescription(string? description)
        {
            _mapObjectModel.Description = description;
            ProcessChanges();
        }

        /// <summary>
        /// The map this object belongs to, or null when it has none.
        /// </summary>
        public Map? GetMap()
        {
            return GetComponentInParent<Map>();
        }

        /// <summary>
        /// The materials of this object that aren't deleted.
        /// </summary>
        public List<MapObjectMaterial> GetMaterials()
        {
            return GetComponentsInChildren<MapObjectMaterial>().Where(material => !material.IsDeleted()).ToList();
        }

        /// <summary>
        /// The material row of this slot, even when it's marked deleted: until the next save, that row still holds
        /// the slot in the database, so a new material for the slot has to reuse it.
        /// </summary>
        public MapObjectMaterial? GetMaterialRow(int materialIndex)
        {
            return GetComponentsInChildren<MapObjectMaterial>().FirstOrDefault(material => material.GetMaterialIndex() == materialIndex);
        }
    }
}
