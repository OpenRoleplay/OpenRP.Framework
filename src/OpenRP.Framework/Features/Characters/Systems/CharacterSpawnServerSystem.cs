using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Characters.Systems
{
    /// <summary>
    /// Puts the player into the world as the character they selected.
    /// </summary>
    public class CharacterSpawnServerSystem : IServerSystem
    {
        // Palomino Creek. Characters don't remember their last position yet.
        private static readonly Vector3 DefaultSpawnPosition = new Vector3(2273.5562f, 82.3747f, 26.4844f);
        private const float DefaultSpawnRotation = 358f;
        private const int DefaultSpawnVirtualWorld = 0;
        private const int NoTeam = 0;

        [ServerEvent]
        public void OnCharacterSelected(OnCharacterSelectedEventArgs args)
        {
            if (args.Player == null || args.Character == null)
            {
                return;
            }

            SendLoggedInAsMessage(args.Player, args.Character);
            SetPlayerToCharacterIdentity(args.Player, args.Character);
            SpawnPlayerAtDefaultSpawn(args.Player, args.Character);
        }

        private static void SendLoggedInAsMessage(Player player, Character character)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Logged in as {ChatColor.CornflowerBlue}{character.GetName()}{ChatColor.White}!");
        }

        private static void SetPlayerToCharacterIdentity(Player player, Character character)
        {
            player.Name = character.GetPlayerName();
            player.Color = Color.White;
        }

        private static void SpawnPlayerAtDefaultSpawn(Player player, Character character)
        {
            player.ToggleSpectating(false);
            player.ToggleControllable(true);
            player.SetSpawnInfo(NoTeam, character.GetSkin(), DefaultSpawnPosition, DefaultSpawnRotation);
            player.VirtualWorld = DefaultSpawnVirtualWorld;
            player.Spawn();
            player.Skin = character.GetSkin();
        }
    }
}
