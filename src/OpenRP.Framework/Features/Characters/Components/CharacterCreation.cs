using OpenRP.Framework.Database.Models;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Characters.Components
{
    public class CharacterCreation : Component
    {
        public CharacterModel CreatingCharacter { get; set; }

        public CharacterCreation()
        {
            CreatingCharacter = new CharacterModel();
        }
    }
}
