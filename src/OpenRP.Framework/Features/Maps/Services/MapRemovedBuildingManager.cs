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
    public class MapRemovedBuildingManager : BaseManager<MapRemovedBuildingModel, MapRemovedBuilding>, IMapRemovedBuildingManager
    {
        private readonly IMapService _mapService;

        public MapRemovedBuildingManager(BaseDataContext dataContext, IEntityManager entityManager, IMapService mapService, ILogger<MapRemovedBuildingManager> logger)
            : base(dataContext, entityManager, logger)
        {
            _mapService = mapService;
        }

        protected override DbSet<MapRemovedBuildingModel> GetDbSet() => _dataContext.MapRemovedBuildings;

        protected override Guid EntityTypeKey => MapEntities.MapRemovedBuildingType;

        protected override EntityId? GetParentEntityId(MapRemovedBuildingModel model)
        {
            return MapEntities.GetMapId((int)model.MapId);
        }

        protected override MapRemovedBuildingModel GetModelFromComponent(MapRemovedBuilding component)
        {
            return component.GetRawMapRemovedBuildingModel();
        }

        /// <summary>
        /// Removals loaded or changed from the database are applied to the online players when their map is loaded, so a
        /// removal made on another server shows here too. The startup load leaves that to the players who connect.
        /// </summary>
        protected override void OnComponentLoaded(MapRemovedBuilding component)
        {
            _mapService.ApplyChangedRemovedBuilding(component);
        }
    }
}
