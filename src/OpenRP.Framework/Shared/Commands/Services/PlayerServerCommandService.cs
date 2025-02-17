using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Commands.Helpers;
using OpenRP.Framework.Features.Permissions.Components;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Helpers;
using SampSharp.Entities.SAMP.Commands;
using SampSharp.Entities.SAMP;
using SampSharp.Entities.Utilities;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Shared.Commands.Services
{
    public class PlayerServerCommandService : CommandServiceBase, IPlayerCommandService
    {
        private readonly IEntityManager _entityManager;

        /// <inheritdoc />
        public PlayerServerCommandService(IEntityManager entityManager) : base(entityManager, 1)
        {
            _entityManager = entityManager;
        }

        /// <inheritdoc />
        protected override bool TryCollectParameters(ParameterInfo[] parameters, int prefixParameters, out CommandParameterInfo[] result)
        {
            if (!base.TryCollectParameters(parameters, prefixParameters, out result))
                return false;

            // Ensure player is first parameter
            var type = parameters[0]
                .ParameterType;
            return type == typeof(EntityId) || typeof(Component).IsAssignableFrom(type);
        }

        /// <inheritdoc />
        public bool Invoke(IServiceProvider services, EntityId player, string inputText)
        {
            if (!player.IsOfType(SampEntities.PlayerType))
                throw new InvalidEntityArgumentException(nameof(player), SampEntities.PlayerType);

            // Check if logged in and playing as Character
            Player playerComp = _entityManager.GetComponent<Player>(player);
            Character character = playerComp.GetPlayerCurrentlyPlayingAsCharacter();
            if (character == null)
            {
                playerComp.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be playing as a character in order to do this command!");
                return true;
            }

            CharacterPermissions characterPermissions = character.GetComponent<CharacterPermissions>();
            if (characterPermissions == null)
            {
                playerComp.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "Something went wrong when checking your permissions, please make a bug report!");
                return true;
            }

            string command = inputText.Split(' ')[0].TrimStart('/').ToLower();
            if (!ServerCommandCache.CachedServerCommands.Any(i => i.Name.ToLower() == command))
            {
                playerComp.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "This command does not exist!");
                return true;
            }

            if (!characterPermissions.DoesPlayerHaveCommandPermission(inputText))
            {
                playerComp.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You do not have enough permissions to use this command!");
                return true;
            }

            var result = Invoke(services, new object[] { player }, inputText);

            if (result.Response != InvokeResponse.InvalidArguments)
                return result.Response == InvokeResponse.Success;

            _entityManager.GetComponent<Player>(player)
                ?.SendClientMessage(result.UsageMessage);
            return true;
        }

        /// <inheritdoc />
        protected override bool ValidateInputText(ref string inputText)
        {
            if (!base.ValidateInputText(ref inputText))
                return false;

            // Player commands must start with a slash.
            if (!inputText.StartsWith("/") || inputText.Length <= 1)
                return false;

            inputText = inputText.Substring(1);

            return true;
        }

        /// <inheritdoc />
        protected override IEnumerable<(MethodInfo method, ICommandMethodInfo commandInfo)> ScanMethods(AssemblyScanner scanner)
        {
            return scanner.ScanMethods<ServerCommandAttribute>()
                .Select(r => (r.method, r.attribute as ICommandMethodInfo));
        }

        private static string CommandText(CommandInfo command)
        {
            if (command.Parameters.Length == 0)
            {
                return TempChatHelper.ReturnPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, $"/{command.Name}");
            }

            return TempChatHelper.ReturnPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, $"/{command.Name} " + string.Join(" ", command.Parameters.Select(arg => arg.IsRequired
                ? $"[{arg.Name.Replace("_", " ")}]"
                : $"<{arg.Name.Replace("_", " ")}>")));
        }

        /// <inheritdoc />
        protected override string GetUsageMessage(CommandInfo[] commands)
        {
            return commands.Length == 1
                ? CommandText(commands[0])
                : $"{string.Join(ChatColor.ChromeYellow + " | " + ChatColor.White, commands.Select(CommandText))}";
        }
    }
}
