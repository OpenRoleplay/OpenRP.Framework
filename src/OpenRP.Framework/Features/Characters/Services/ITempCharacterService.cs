using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Characters.Services
{
    public interface ITempCharacterService
    {
        Character ReloadCharacter(Player player, ulong characterId);
        bool SetCharacterAccent(Character character, string accent);
        InventoryModel GetCharacterInventory(Character character);
    }
}
