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
    public class MapObjectManager : BaseManager<MapObjectModel, MapObject>, IMapObjectManager
    {
        private readonly IMapService _mapService;

        public MapObjectManager(BaseDataContext dataContext, IEntityManager entityManager, IMapService mapService, ILogger<MapObjectManager> logger)
            : base(dataContext, entityManager, logger)
        {
            _mapService = mapService;
        }

        protected override DbSet<MapObjectModel> GetDbSet() => _dataContext.MapObjects;

        protected override Guid EntityTypeKey => MapEntities.MapObjectType;

        protected override EntityId? GetParentEntityId(MapObjectModel model)
        {
            return MapEntities.GetMapId((int)model.MapId);
        }

        protected override MapObjectModel GetModelFromComponent(MapObject component)
        {
            return component.GetRawMapObjectModel();
        }

        /// <summary>
        /// Objects loaded or changed from the database are spawned on a loaded map, so the game shows them. The startup
        /// load leaves that to the map's spawn, which spawns each object once.
        /// </summary>
        protected override void OnComponentLoaded(MapObject component)
        {
            _mapService.RespawnChangedObject(component);
        }
    }
}
