using OpenRP.Framework.Features.Accounts.Components;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities.SAMP;
using System.Globalization;

namespace OpenRP.Framework.Features.Characters.Services.Dialogs
{
    /// <summary>
    /// Character creation: first name -> middle name (optional) -> last name -> date of birth.
    /// "Previous" always goes back one step; going back from the first step returns to character selection.
    /// </summary>
    public partial class CharacterDialogService
    {
        private const string CharacterCreationTitle = "Character Creation";
        private const int FirstNameMaxLength = 35;
        private const int MiddleNameMaxLength = 30;
        private const int LastNameMaxLength = 35;
        private const string DateOfBirthDisplayFormat = "dd/MM/yyyy";
        private static readonly string[] DateOfBirthInputFormats = { "dd/MM/yyyy", "d/M/yyyy" };

        private void BeginCharacterCreation(Player player, Action onGoBack)
        {
            player.DestroyComponents<CharacterCreation>();
            player.AddComponent<CharacterCreation>();

            OpenCreateCharacterFirstName(player, onGoBack);
        }

        private void CancelCharacterCreation(Player player, Action onGoBack)
        {
            player.DestroyComponents<CharacterCreation>();

            OpenCharacterSelection(player, onGoBack);
        }

        private void OpenCreateCharacterFirstName(Player player, Action onGoBack)
        {
            string content = $"Pick a first name for your character. The first name of your character can be up to {FirstNameMaxLength} characters long.";

            void OnFirstNameEntered(string? input)
            {
                string? error = GetRequiredNameError(input, "first name", FirstNameMaxLength);
                if (error != null)
                {
                    ShowCharacterCreationError(player, "First name", error, () => OpenCreateCharacterFirstName(player, onGoBack));
                    return;
                }

                GetCharacterCreation(player).CreatingCharacter.FirstName = input!.Trim();
                OpenCreateCharacterMiddleNameQuestion(player, onGoBack);
            }

            ShowCharacterCreationInput(player, "First name", content, OnFirstNameEntered, () => CancelCharacterCreation(player, onGoBack));
        }

        private void OpenCreateCharacterMiddleNameQuestion(Player player, Action onGoBack)
        {
            BetterMessageDialog questionDialog = new BetterMessageDialog(DialogHelper.Yes, DialogHelper.No);
            questionDialog.SetTitle(TitleType.Children, CharacterCreationTitle, "Middle name");
            questionDialog.SetContent("Do you want to give your character a middle name?");

            void MiddleNameQuestionDialogHandler(MessageDialogResponse response)
            {
                if (response.Response == DialogResponse.LeftButton)
                {
                    OpenCreateCharacterMiddleName(player, onGoBack);
                    return;
                }

                GetCharacterCreation(player).CreatingCharacter.MiddleName = null;
                OpenCreateCharacterLastName(player, onGoBack);
            }

            _dialogService.Show(player, questionDialog, MiddleNameQuestionDialogHandler);
        }

        private void OpenCreateCharacterMiddleName(Player player, Action onGoBack)
        {
            string content = $"Pick a middle name for your character. The middle name of your character can be up to {MiddleNameMaxLength} characters long. You can also skip this step by not filling a middle name in.";

            void OnMiddleNameEntered(string? input)
            {
                string? error = GetOptionalNameError(input, "middle name", MiddleNameMaxLength);
                if (error != null)
                {
                    ShowCharacterCreationError(player, "Middle name", error, () => OpenCreateCharacterMiddleName(player, onGoBack));
                    return;
                }

                GetCharacterCreation(player).CreatingCharacter.MiddleName = String.IsNullOrWhiteSpace(input) ? null : input.Trim();
                OpenCreateCharacterLastName(player, onGoBack);
            }

            ShowCharacterCreationInput(player, "Middle name", content, OnMiddleNameEntered, () => OpenCreateCharacterMiddleNameQuestion(player, onGoBack));
        }

        private void OpenCreateCharacterLastName(Player player, Action onGoBack)
        {
            string content = $"Pick a last name for your character. The last name of your character can be up to {LastNameMaxLength} characters long.";

            void OnLastNameEntered(string? input)
            {
                string? error = GetRequiredNameError(input, "last name", LastNameMaxLength);
                if (error != null)
                {
                    ShowCharacterCreationError(player, "Last name", error, () => OpenCreateCharacterLastName(player, onGoBack));
                    return;
                }

                GetCharacterCreation(player).CreatingCharacter.LastName = input!.Trim();
                OpenCreateCharacterDateOfBirth(player, onGoBack);
            }

            ShowCharacterCreationInput(player, "Last name", content, OnLastNameEntered, () => OpenCreateCharacterMiddleNameQuestion(player, onGoBack));
        }

        private void OpenCreateCharacterDateOfBirth(Player player, Action onGoBack)
        {
            string content = $"Pick a date of birth for your character. The appropriate format to use is {DateOfBirthDisplayFormat.ToUpperInvariant()}.";

            void OnDateOfBirthEntered(string? input)
            {
                if (!TryParseDateOfBirth(input, out DateTime dateOfBirth))
                {
                    ShowCharacterCreationError(player, "Date of Birth", "Your chosen format for the Date of Birth is incorrect, please try again.", () => OpenCreateCharacterDateOfBirth(player, onGoBack));
                    return;
                }

                OpenConfirmCharacterDateOfBirth(player, dateOfBirth, onGoBack);
            }

            ShowCharacterCreationInput(player, "Date of Birth", content, OnDateOfBirthEntered, () => OpenCreateCharacterLastName(player, onGoBack));
        }

        private void OpenConfirmCharacterDateOfBirth(Player player, DateTime dateOfBirth, Action onGoBack)
        {
            int age = GetAgeInYears(dateOfBirth, DateTime.Today);

            BetterMessageDialog confirmDialog = new BetterMessageDialog(DialogHelper.Yes, DialogHelper.No);
            confirmDialog.SetTitle(TitleType.Children, CharacterCreationTitle, "Date of Birth");
            confirmDialog.SetContent($"Your chosen Date of Birth is {dateOfBirth.ToString(DateOfBirthDisplayFormat, CultureInfo.InvariantCulture)}, meaning that your character would be {age} years old. Is that correct?");

            void ConfirmDateOfBirthDialogHandler(MessageDialogResponse response)
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    OpenCreateCharacterDateOfBirth(player, onGoBack);
                    return;
                }

                GetCharacterCreation(player).CreatingCharacter.DateOfBirth = dateOfBirth;
                FinishCharacterCreation(player, onGoBack);
            }

            _dialogService.Show(player, confirmDialog, ConfirmDateOfBirthDialogHandler);
        }

        private void FinishCharacterCreation(Player player, Action onGoBack)
        {
            Account account = player.GetComponent<Account>();
            bool isCreated = _characterService.CreateCharacter(account, GetCharacterCreation(player).CreatingCharacter);
            player.DestroyComponents<CharacterCreation>();

            if (!isCreated)
            {
                ShowCharacterCreationError(player, "Error", "Your character could not be created. Please try again later.", () => OpenCharacterSelection(player, onGoBack));
                return;
            }

            OpenCharacterSelection(player, onGoBack);
        }

        private void ShowCharacterCreationInput(Player player, string step, string content, Action<string?> onNext, Action onPrevious)
        {
            BetterInputDialog inputDialog = new BetterInputDialog(DialogHelper.Next, DialogHelper.Previous);
            inputDialog.SetTitle(TitleType.Children, CharacterCreationTitle, step);
            inputDialog.SetContent(content);

            void CharacterCreationInputDialogHandler(InputDialogResponse response)
            {
                if (response.Response == DialogResponse.LeftButton)
                {
                    onNext(response.InputText);
                    return;
                }

                onPrevious();
            }

            _dialogService.Show(player, inputDialog, CharacterCreationInputDialogHandler);
        }

        private void ShowCharacterCreationError(Player player, string step, string message, Action onRetry)
        {
            ShowMessage(player, DialogHelper.Retry, message, onRetry, CharacterCreationTitle, step);
        }

        private static CharacterCreation GetCharacterCreation(Player player)
        {
            return player.GetComponent<CharacterCreation>() ?? player.AddComponent<CharacterCreation>();
        }

        private static string? GetRequiredNameError(string? input, string nameLabel, int maxLength)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return $"The {nameLabel} for your character is required!";
            }

            return GetOptionalNameError(input, nameLabel, maxLength);
        }

        private static string? GetOptionalNameError(string? input, string nameLabel, int maxLength)
        {
            if (input != null && input.Trim().Length > maxLength)
            {
                return $"The {nameLabel} for your character may not be longer than {maxLength} characters.";
            }

            return null;
        }

        private static bool TryParseDateOfBirth(string? input, out DateTime dateOfBirth)
        {
            return DateTime.TryParseExact(input?.Trim(), DateOfBirthInputFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth);
        }

        private static int GetAgeInYears(DateTime dateOfBirth, DateTime today)
        {
            int age = today.Year - dateOfBirth.Year;
            bool hasHadBirthdayThisYear = dateOfBirth.Date <= today.AddYears(-age);

            return hasHadBirthdayThisYear ? age : age - 1;
        }
    }
}
