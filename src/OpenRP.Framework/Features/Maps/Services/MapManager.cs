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
    public class MapManager : BaseManager<MapModel, Map>, IMapManager
    {
        private readonly IMapService _mapService;

        public MapManager(BaseDataContext dataContext, IEntityManager entityManager, IMapService mapService, ILogger<MapManager> logger)
            : base(dataContext, entityManager, logger)
        {
            _mapService = mapService;
        }

        protected override DbSet<MapModel> GetDbSet() => _dataContext.Maps;

        protected override Guid EntityTypeKey => MapEntities.MapType;

        protected override MapModel GetModelFromComponent(Map component)
        {
            return component.GetRawMapModel();
        }

        /// <summary>
        /// A map loaded or unloaded on another server is spawned or despawned here, so the game shows it as saved.
        /// </summary>
        protected override void OnComponentLoaded(Map component)
        {
            _mapService.SyncChangedMap(component);
        }
    }
}
