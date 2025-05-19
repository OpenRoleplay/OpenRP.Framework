using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Maps.Components
{
    public class Map : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private bool _hasChanges;
        private bool _isDeleted;
        private MapModel _cachedMapModel;

        public Map(MapModel mapModel)
        {
            _cachedMapModel = mapModel;
        }

        // IChangeable
        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        // IDeletable
        public bool IsDeleted()
        {
            return _isDeleted;
        }

        public void ProcessDeletion(bool processDeletion = true)
        {
            _isDeleted = processDeletion;
        }

        // IBaseDataComponent
        public ulong GetId()
        {
            return _cachedMapModel.Id;
        }

        // Map Component
    }
}
