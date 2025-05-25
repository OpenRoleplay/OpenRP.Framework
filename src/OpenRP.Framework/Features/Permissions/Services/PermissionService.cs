using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Commands.Helpers;
using OpenRP.Framework.Features.Permissions.Components;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Permissions.Services
{
    public class PermissionService : IPermissionService
    {
        private IEntityManager _entityManager;
        private BaseDataContext _dataContext;

        public PermissionService(IEntityManager entityManager, BaseDataContext dataContext)
        {
            _entityManager = entityManager;
            _dataContext = dataContext;
        }

        public CharacterPermissions ReloadCharacterPermissions(Character character)
        {
            // Remove Old Component (if any)
            character.DestroyComponents<CharacterPermissions>();

            // Get permissions
            List<string> permissionList = GetCharacterPermissions(character.GetId());

            // Add Component
            CharacterPermissions characterPermissions = character.AddComponent<CharacterPermissions>(permissionList);

            return characterPermissions;
        }

        public List<string> GetCharacterPermissions(ulong characterId)
        {
            List<string> permissions = GetCharacterPermissionsModels(characterId).Select(i => i?.Name).ToList();

            return permissions;
        }

        public List<PermissionModel> GetCharacterPermissionsModels(ulong characterId)
        {
            List<PermissionModel> permissions = new List<PermissionModel>();

            permissions.AddRange(GetCharacterPermissionGroups(characterId));

            // Merge duplicates
            permissions = permissions.GroupBy(i => i.Name, (key, value) => value.OrderBy(e => e.Name).First()).ToList();

            return permissions;
        }

        public List<PermissionModel> GetCharacterPermissionGroups(ulong characterId)
        {
            GiveCharacterPermissionGroupByName(characterId, "Default");

            List<CharacterPermissionGroupModel> characterPermissionGroups = _dataContext.CharacterPermissionGroups
                .Include(perm => perm.PermissionGroup)
                    .ThenInclude(perm => perm.Permissions)
                        .ThenInclude(perm => perm.Permission)
                .Where(i => i.CharacterId == characterId)
                .ToList();

            List<PermissionModel> permissionModels = characterPermissionGroups.Select(i => i.PermissionGroup).SelectMany(i => i.Permissions).Select(i => i.Permission).ToList();

            return permissionModels;
        }

        public bool GiveCharacterPermissionGroupByName(ulong characterId, string permissionGroup)
        {
            PermissionGroupModel permissionGroupModel = _dataContext.PermissionGroups.FirstOrDefault(i => i.Name == permissionGroup);
            if (permissionGroupModel != null)
            {
                if (!_dataContext.CharacterPermissionGroups.Any(i => i.PermissionGroupId == permissionGroupModel.Id && i.CharacterId == characterId))
                {
                    _dataContext.CharacterPermissionGroups.Add(new CharacterPermissionGroupModel()
                    {
                        CharacterId = characterId,
                        PermissionGroupId = permissionGroupModel.Id
                    });
                    _dataContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public void CreateDatabaseCommandPermissionsIfNotExists()
        {
            Console.WriteLine("\nBegin Gamemode Automatic Command Permission Management");
            // Get all types in the attached assemblies
            List<MethodInfo> commandMethods = ServerCommandCache.CachedServerCommands.Select(i => i.Method).ToList();

            foreach (MethodInfo method in commandMethods)
            {
                ServerCommandAttribute attribute = method.GetCustomAttribute<ServerCommandAttribute>();

                if (attribute != null)
                {
                    string commandName = attribute.Name ?? method.Name; // Use attribute name or method name as fallback
                    string permissionName = $"cmd.{commandName.ToLower()}";

                    var strategy = _dataContext.Database.CreateExecutionStrategy();

                    strategy.Execute(() =>
                    {
                        using (var transaction = _dataContext.Database.BeginTransaction())
                        {
                            try
                            {
                                // Retrieve or create the PermissionModel
                                var permission = _dataContext.Permissions
                                    .FirstOrDefault(p => p.Name == permissionName);

                                if (permission == null)
                                {
                                    permission = new PermissionModel
                                    {
                                        Name = permissionName
                                        ,
                                        Description = $"This permission grants the ability to use the /{commandName.ToLower()} command."
                                    };
                                    _dataContext.Permissions.Add(permission);
                                    Console.WriteLine($"Created permission for command: {commandName} (new permission = {permissionName}).");
                                    _dataContext.SaveChanges(); // Save to generate the ID if it's auto-generated
                                }

                                // Update description if it has none.
                                if (String.IsNullOrEmpty(permission.Description))
                                {
                                    permission.Description = $"This permission grants the ability to use the /{commandName.ToLower()} command.";
                                    Console.WriteLine($"Updated permission for command: {commandName} (new description = \"{permission.Description}\").");
                                    _dataContext.SaveChanges();
                                }

                                // Fetch all desired PermissionGroupModels based on attribute.PermissionGroups
                                var desiredGroupNames = attribute.PermissionGroups.Select(pg => pg.Trim()).ToList();
                                var desiredPermissionGroups = _dataContext.PermissionGroups
                                    .Where(pg => desiredGroupNames.Contains(pg.Name))
                                    .ToList();

                                // Log or handle missing permission groups
                                var foundGroupNames = desiredPermissionGroups.Select(pg => pg.Name).ToHashSet();
                                var missingGroups = desiredGroupNames.Except(foundGroupNames);
                                if (missingGroups.Any())
                                {
                                    Console.WriteLine($"Warning: The following permission groups were not found: {string.Join(", ", missingGroups)}");
                                }

                                // Retrieve existing PermissionGroupPermissionModels for this permission
                                var existingPgPermissions = _dataContext.PermissionGroupPermissions
                                    .Where(pgpm => pgpm.PermissionId == permission.Id && pgpm.GamemodeManaged)
                                    .ToList();

                                var existingGroupIds = existingPgPermissions.Select(pgpm => pgpm.PermissionGroupId).ToHashSet();
                                var desiredGroupIds = desiredPermissionGroups.Select(pg => pg.Id).ToHashSet();

                                // Determine which groups to add and which to remove
                                var groupsToAdd = desiredPermissionGroups
                                    .Where(pg => !existingGroupIds.Contains(pg.Id))
                                    .ToList();

                                var groupsToRemove = existingPgPermissions
                                    .Where(pgpm => !desiredGroupIds.Contains(pgpm.PermissionGroupId))
                                    .ToList();

                                // Add new PermissionGroupPermissionModels
                                foreach (var group in groupsToAdd)
                                {
                                    var pgpm = new PermissionGroupPermissionModel
                                    {
                                        GamemodeManaged = true,
                                        PermissionId = permission.Id,
                                        PermissionGroupId = group.Id
                                    };
                                    _dataContext.PermissionGroupPermissions.Add(pgpm);
                                    Console.WriteLine($"Permission {permissionName} has been added to permission group {group.Name}.");
                                }

                                // Remove PermissionGroupPermissionModels that are no longer desired
                                foreach (var pgpm in groupsToRemove)
                                {
                                    _dataContext.PermissionGroupPermissions.Remove(pgpm);
                                    Console.WriteLine($"Permission {permissionName} has been removed from permission group {pgpm.PermissionGroup.Name}.");
                                }

                                // Save all changes within the transaction
                                _dataContext.SaveChanges();
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                // Rollback the transaction if any error occurs
                                transaction.Rollback();
                                Console.WriteLine($"An error occurred: {ex.Message}");
                            }
                        }
                    });
                }
            }
            Console.WriteLine("End Gamemode Automatic Command Permission Management\n");
        }

        public void RemoveOldCommandPermissions()
        {
            Console.WriteLine("\nBegin Gamemode Old Command Permission Cleanup");

            // Get all current command permissions in the gamemode
            HashSet<string> currentCommandPermissions = ServerCommandCache.CachedServerCommands.Select(i => i.Method)
            .Select(m =>
            {
                var attribute = m.GetCustomAttribute<ServerCommandAttribute>();
                string commandName = attribute.Name ?? m.Name;
                return $"cmd.{commandName.ToLower()}";
            })
            .ToHashSet();

            // Get all permissions in the database that start with "cmd."
            var existingCommandPermissions = _dataContext.Permissions
                .Where(p => p.Name.StartsWith("cmd."))
                .ToList();

            var permissionsToRemove = existingCommandPermissions
                .Where(p => !currentCommandPermissions.Contains(p.Name))
                .ToList();

            if (permissionsToRemove.Any())
            {
                foreach (var permission in permissionsToRemove)
                {
                    Console.WriteLine($"Removing old permission: {permission.Name}");
                    _dataContext.Permissions.Remove(permission);
                }

                // Save changes
                _dataContext.SaveChanges();
            }
            else
            {
                Console.WriteLine("No old command permissions to remove.");
            }

            Console.WriteLine("End Gamemode Old Command Permission Cleanup\n");
        }

    }
}
