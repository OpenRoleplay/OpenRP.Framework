using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database;
using OpenRP.Framework.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace OpenRP.Framework.Features.Characters.Components
{
    public class Character : Component
    {
        private Player _player;
        private CharacterModel _characterModel;

        public Character(CharacterModel characterModel)
        {
            _characterModel = characterModel;
        }

        public ulong GetDatabaseId() => GetCharacterModel().Id;

        public Player GetPlayer()
        {
            if (_player == null && this.Entity.IsOfType(SampEntities.PlayerType))
            {
                _player = this.GetComponent<Player>();
            }
            return _player;
        }

        /// <summary>
        /// Method to get the Character Model. Don't use, will be deprecated in the future.
        /// </summary>
        public CharacterModel GetCharacterModel()
        {
            return _characterModel;
        }

        public string GetCharacterName()
        {
            return String.Format("{0} {1}", _characterModel.FirstName, _characterModel.LastName);
        }
    }

}
