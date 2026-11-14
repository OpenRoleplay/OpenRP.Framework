using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Maps.Services
{
    public class MapObjectMaterialManager : BaseManager<MapObjectMaterialModel, MapObjectMaterial>, IMapObjectMaterialManager
    {
        private readonly IMapService _mapService;

        public MapObjectMaterialManager(BaseDataContext dataContext, IEntityManager entityManager, IMapService mapService, ILogger<MapObjectMaterialManager> logger)
            : base(dataContext, entityManager, logger)
        {
            _mapService = mapService;
        }

        protected override DbSet<MapObjectMaterialModel> GetDbSet() => _dataContext.MapObjectMaterials;

        protected override Guid EntityTypeKey => MapEntities.MapObjectMaterialEntityType;

        protected override EntityId? GetParentEntityId(MapObjectMaterialModel model)
        {
            return MapEntities.GetMapObjectId((int)model.MapObjectId);
        }

        protected override MapObjectMaterialModel GetModelFromComponent(MapObjectMaterial component)
        {
            return component.GetRawMapObjectMaterialModel();
        }

        /// <summary>
        /// A material loaded or changed from the database respawns its object, so the game shows it as saved.
        /// </summary>
        protected override void OnComponentLoaded(MapObjectMaterial component)
        {
            MapObject? mapObject = component.GetObject();
            if (mapObject != null)
            {
                _mapService.RespawnChangedObject(mapObject);
            }
        }
    }
}
