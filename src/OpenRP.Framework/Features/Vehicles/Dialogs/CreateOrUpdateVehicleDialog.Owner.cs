using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Vehicles.Dialogs
{
    /// <summary>
    /// The "Owner" row: the character who owns the vehicle, by character id or full name. "None" clears it.
    /// Characters are looked up in the same database context as the editor's other data.
    /// </summary>
    public partial class CreateOrUpdateVehicleDialog
    {
        private const string NoneText = "None";

        private void AskForOwner()
        {
            OpenInputDialog("Edit Owner", Data.OwnerCharacterId?.ToString() ?? NoneText, SetOwnerFromInput, ShowEditor,
                "Enter the owner's character id or full name (First Last), or None to clear the owner.");
        }

        private void SetOwnerFromInput(string input)
        {
            string text = input.Trim();
            if (text.Length == 0)
            {
                ShowEditor();
                return;
            }

            if (text.Equals(NoneText, StringComparison.OrdinalIgnoreCase))
            {
                Data.OwnerCharacterId = null;
                ShowEditor();
                return;
            }

            (ulong? characterId, string? error) = FindCharacterId(text);
            if (error != null)
            {
                OpenMessageDialog("Invalid Owner", error, AskForOwner, ShowEditor);
                return;
            }

            Data.OwnerCharacterId = characterId;
            ShowEditor();
        }

        private (ulong? CharacterId, string? Error) FindCharacterId(string text)
        {
            if (!ulong.TryParse(text, out ulong characterId))
            {
                return FindCharacterIdByName(text);
            }

            if (!_context.Characters.AsNoTracking().Any(character => character.Id == characterId))
            {
                return (null, $"There's no character with the id {characterId}.");
            }
            return (characterId, null);
        }

        private (ulong? CharacterId, string? Error) FindCharacterIdByName(string name)
        {
            string[] parts = name.Replace('_', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                return (null, "Enter the full name, first and last, for example John Smith, or a character id.");
            }

            string firstName = parts[0];
            string lastName = parts[^1];
            List<ulong> characterIds = _context.Characters.AsNoTracking()
                .Where(character => character.FirstName == firstName && character.LastName == lastName)
                .Select(character => character.Id)
                .Take(2)
                .ToList();

            if (characterIds.Count == 0)
            {
                return (null, $"Nobody is called {firstName} {lastName}.");
            }
            if (characterIds.Count > 1)
            {
                return (null, $"Several people are called {firstName} {lastName}. Enter the character id instead.");
            }
            return (characterIds[0], null);
        }

        private string GetOwnerLabel()
        {
            ulong? ownerId = Data.OwnerCharacterId;
            if (ownerId == null)
            {
                return NoneText;
            }

            string? name = _context.Characters.AsNoTracking()
                .Where(character => character.Id == ownerId.Value)
                .Select(character => character.FirstName + " " + character.LastName)
                .FirstOrDefault();
            return name == null ? $"Character #{ownerId.Value} (not found)" : $"{name} (#{ownerId.Value})";
        }
    }
}
