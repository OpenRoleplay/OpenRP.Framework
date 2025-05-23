using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Accounts.Components;
using OpenRP.Framework.Features.Accounts.Services;
using OpenRP.Framework.Features.ActorConversations.Services;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Discord.Services;
using OpenRP.Framework.Features.MainMenu.Services.Dialogs;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Characters.Services.Dialogs
{
    public class CharacterDialogService
    {
        private readonly IAccountService _accountService;
        private readonly IDialogService _dialogService;
        private readonly IServerEventAggregator _serverEventAggregator;
        private readonly IActorConversationWithPlayerManager _actorConversationWithPlayerManager;
        private readonly IDiscordService _discordService;
        private readonly ITempCharacterService _tempCharacterService;
        public CharacterDialogService(IAccountService accountService, IDialogService dialogService, IServerEventAggregator serverEventAggregator, IActorConversationWithPlayerManager actorConversationWithPlayerManager, IDiscordService discordService, ITempCharacterService tempCharacterService)
        {
            _accountService = accountService;
            _dialogService = dialogService;
            _serverEventAggregator = serverEventAggregator;
            _actorConversationWithPlayerManager = actorConversationWithPlayerManager;
            _discordService = discordService;
            _tempCharacterService = tempCharacterService;
        }

        public void OpenCharacterSelection(Player player, Action onGoBack)
        {
            BetterListDialog choiceDialog = new BetterListDialog("Next", "Quit");
            choiceDialog.SetTitle(TitleType.Children, "Character Selection");
            Account account = player.GetComponent<Account>();
            List<CharacterModel> characterModels = _accountService.GetCharactersByAccountId(account.GetAccountId());

            void CharacterSelectionDialogHandler(ListDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    if (r.ItemIndex == characterModels?.Count)
                    {
                        OpenCreateCharacterFirstName(player, onGoBack);
                    }
                    else
                    {
                        CharacterModel selectedCharacter = characterModels.ElementAt(r.ItemIndex);
                        Character characterComponent = _tempCharacterService.ReloadCharacter(player, selectedCharacter.Id);
                        player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, String.Format("Logged in as {0}{1} {2}{3}!", ChatColor.CornflowerBlue, selectedCharacter.FirstName, selectedCharacter.LastName, ChatColor.White));
                        // player.OnCharacterSelected();

                        // Temporary for testing
                        player.ToggleSpectating(false);
                        player.ToggleControllable(true);
                        _actorConversationWithPlayerManager.DetachPlayerFromConversationAsync("CONV_FAMILY", player);
                        player.StopAudioStream();
                        player.SetSpawnInfo(0, selectedCharacter.Skin, new Vector3(2273.5562, 82.3747, 26.4844), 358);
                        player.VirtualWorld = 0;
                        player.Name = String.Format("{0}_{1}", selectedCharacter.FirstName, selectedCharacter.LastName);
                        player.Color = Color.White;
                        player.Spawn();
                        player.Skin = selectedCharacter.Skin;

                        #if (!DEBUG)
                            _discordService.SendGeneralChatMessage($"## {player.Name.Replace("_", " ")} is now playing on the server.");
                        #endif

                        var eventArgs = new OnCharacterSelectedEventArgs
                        {
                            Player = player,
                            Account = account,
                            Character = characterComponent
                        };
                        _serverEventAggregator.PublishAsync(eventArgs);
                    }
                }
                else
                {
                    player.Kick();
                }
            }

            if (characterModels != null)
            {
                foreach (CharacterModel character in characterModels)
                {
                    choiceDialog.Add(String.Format("{0}{1} {2}", ChatColor.CornflowerBlue, character.FirstName, character.LastName));
                }
            }

            choiceDialog.AddRow("Create a new character");

            _dialogService.Show(player.Entity, choiceDialog, CharacterSelectionDialogHandler);
        }

        private void OpenCreateCharacterFirstName(Player player, Action onGoBack)
        {
            BetterInputDialog characterDialog = new BetterInputDialog("Next", "Go Back");
            characterDialog.SetTitle(TitleType.Children, "Character Creation", "First name");
            characterDialog.SetContent("Pick a first name for your character. The first name of your character can be up to 35 characters long.");

            void CreateCharacterFirstNameDialogHandler(InputDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    player.DestroyComponents<CharacterCreation>();
                    CharacterCreation charCreationComponent = player.AddComponent<CharacterCreation>();

                    if (String.IsNullOrEmpty(r.InputText))
                    {
                        BetterMessageDialog firstNameRequired = new BetterMessageDialog("Retry");
                        firstNameRequired.SetTitle(TitleType.Children, "Character Creation", "First name");
                        firstNameRequired.SetContent("The first name for your character is required!");

                        void FirstNameRequiredDialogHandler(MessageDialogResponse r)
                        {
                            OpenCreateCharacterFirstName(player, onGoBack);
                        };

                        _dialogService.Show(player.Entity, firstNameRequired, FirstNameRequiredDialogHandler);
                    }
                    else if (r.InputText.Length > 35)
                    {
                        BetterMessageDialog firstNameTooLongDialog = new BetterMessageDialog(DialogHelper.Retry);
                        firstNameTooLongDialog.SetTitle(TitleType.Children, "Character Creation", "First name");
                        firstNameTooLongDialog.SetContent("The first name for your character may not be longer than 35 characters.");

                        void FirstNameTooLongDialogHandler(MessageDialogResponse r)
                        {
                            OpenCreateCharacterFirstName(player, onGoBack);
                        };

                        _dialogService.Show(player.Entity, firstNameTooLongDialog, FirstNameTooLongDialogHandler);
                    }
                    else
                    {
                        charCreationComponent.CreatingCharacter.FirstName = r.InputText.Trim();

                        OpenCreateCharacterMiddleName(player, onGoBack);
                    }
                }
                else
                {
                    onGoBack?.Invoke();
                }
            }

            _dialogService.Show(player.Entity, characterDialog, CreateCharacterFirstNameDialogHandler);
        }

        private void OpenCreateCharacterMiddleName(Player player, Action onGoBack)
        {
            BetterMessageDialog middleNameYesOrNoDialog = new BetterMessageDialog("Yes", "No");
            middleNameYesOrNoDialog.SetTitle(TitleType.Children, "Character Creation", "Middle name");
            middleNameYesOrNoDialog.SetContent("Pick a middle name for your character. The middle name of your character can be up to 30 characters long. You can also skip this step by not filling a middle name in.");

            void MiddleNameYesOrNoDialogHandler(MessageDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    BetterInputDialog characterDialog = new BetterInputDialog("Next", "Previous");
                    characterDialog.SetTitle(TitleType.Children, "Character Creation", "Middle name");
                    characterDialog.SetContent("Pick a middle name for your character. The middle name of your character can be up to 30 characters long. You can also skip this step by not filling a middle name in.");

                    void CreateCharacterMiddleNameDialogHandler(InputDialogResponse r)
                    {
                        if (r.Response == DialogResponse.LeftButton)
                        {
                            CharacterCreation charCreationComponent = player.GetComponent<CharacterCreation>();

                            if (String.IsNullOrEmpty(r.InputText))
                            {
                                charCreationComponent.CreatingCharacter.MiddleName = null;

                                OpenCreateCharacterLastName(player, onGoBack);
                            }
                            else if (r.InputText.Length > 30)
                            {
                                BetterMessageDialog middleNameTooLongDialog = new BetterMessageDialog("Retry");
                                middleNameTooLongDialog.SetTitle(TitleType.Children, "Character Creation", "Middle name");
                                middleNameTooLongDialog.SetContent("The middle name for your character may not be longer than 30 characters.");

                                void MiddleNameTooLongDialogHandler(MessageDialogResponse r)
                                {
                                    OpenCreateCharacterMiddleName(player, onGoBack);
                                };

                                _dialogService.Show(player.Entity, middleNameTooLongDialog, MiddleNameTooLongDialogHandler);
                            }
                            else
                            {
                                charCreationComponent.CreatingCharacter.MiddleName = r.InputText.Trim();

                                OpenCreateCharacterLastName(player, onGoBack);
                            }
                        }
                        else
                        {
                            OpenCreateCharacterFirstName(player, onGoBack);
                        }
                    }

                    _dialogService.Show(player.Entity, characterDialog, CreateCharacterMiddleNameDialogHandler);
                }
                else
                {
                    onGoBack?.Invoke();
                }
            }

            _dialogService.Show(player.Entity, middleNameYesOrNoDialog, MiddleNameYesOrNoDialogHandler);
        }

        private void OpenCreateCharacterLastName(Player player, Action onGoBack)
        {
            BetterInputDialog characterDialog = new BetterInputDialog("Next", "Previous");
            characterDialog.SetTitle(TitleType.Children, "Character Creation", "Last name");
            characterDialog.SetContent("Pick a last name for your character. The first name of your character can be up to 35 characters long.");

            void CreateCharacterLastNameDialogHandler(InputDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    CharacterCreation charCreationComponent = player.GetComponent<CharacterCreation>();

                    if (string.IsNullOrEmpty(r.InputText))
                    {
                        BetterMessageDialog firstNameRequired = new BetterMessageDialog("Retry");
                        firstNameRequired.SetTitle(TitleType.Children, "Character Creation", "Last name");
                        firstNameRequired.SetContent("The last name for your character is required!");

                        void LastNameRequiredDialogHandler(MessageDialogResponse r)
                        {
                            OpenCreateCharacterLastName(player, onGoBack);
                        }

                        _dialogService.Show(player.Entity, firstNameRequired, LastNameRequiredDialogHandler);
                    }
                    else if (r.InputText.Length > 35)
                    {
                        BetterMessageDialog lastNameTooLongDialog = new BetterMessageDialog("Retry");
                        lastNameTooLongDialog.SetTitle(TitleType.Children, "Character Creation", "Last name");
                        lastNameTooLongDialog.SetContent("The last name for your character may not be longer than 35 characters.");

                        void LastNameTooLongDialogHandler(MessageDialogResponse r)
                        {
                            OpenCreateCharacterLastName(player, onGoBack);
                        }

                        _dialogService.Show(player.Entity, lastNameTooLongDialog, LastNameTooLongDialogHandler);
                    }
                    else
                    {
                        charCreationComponent.CreatingCharacter.LastName = r.InputText.Trim();

                        OpenCreateCharacterDateOfBirth(player, onGoBack);
                    }
                }
                else
                {
                    onGoBack?.Invoke();
                }
            }

            _dialogService.Show(player.Entity, characterDialog, CreateCharacterLastNameDialogHandler);
        }

        private void OpenCreateCharacterDateOfBirth(Player player, Action onGoBack)
        {
            BetterInputDialog characterDialog = new BetterInputDialog("Next", "Previous");
            characterDialog.SetTitle(TitleType.Children, "Character Creation", "Date of Birth");
            characterDialog.SetContent("Pick a date of birth for your character. The appropriate format to use is DD/MM/YYYY.");

            void CreateCharacterDateOfBirthDialogHandler(InputDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    CharacterCreation charCreationComponent = player.GetComponent<CharacterCreation>();

                    DateTime characterDoB;
                    if (DateTime.TryParse(r.InputText, out characterDoB))
                    {
                        BetterMessageDialog confirmDateOfBirth = new BetterMessageDialog("Yes", "No");
                        confirmDateOfBirth.SetTitle(TitleType.Children, "Character Creation", "Date of Birth");
                        confirmDateOfBirth.SetContent(String.Format("Your chosen Date of Birth is {0}, meaning that your character would be {1} years old. Is that correct?", characterDoB.ToString("dd/MM/yyyy"), (DateTime.Today.Year - characterDoB.Year)));

                        void ConfirmDialogHandler(MessageDialogResponse confirmResponse)
                        {
                            if (confirmResponse.Response == DialogResponse.LeftButton)
                            {
                                charCreationComponent.CreatingCharacter.DateOfBirth = characterDoB;

                                // Next Step
                                _accountService.CreateCharacter(player, charCreationComponent.CreatingCharacter);
                                player.DestroyComponents<CharacterCreation>();

                                OpenCharacterSelection(player, onGoBack);
                            }
                            else
                            {
                                OpenCreateCharacterDateOfBirth(player, onGoBack);
                            }
                        }
                        ;

                        _dialogService.Show(player.Entity, confirmDateOfBirth, ConfirmDialogHandler);
                    }
                    else
                    {
                        MessageDialog incorrectFormatDialog = new MessageDialog(DialogHelper.GetTitle("Character Creation", "Date of Birth"), ChatColor.White + "Your chosen format for the Date of Birth is incorrect, please try again.", DialogHelper.Retry);

                        void IncorrectFormatDialogHandler(MessageDialogResponse r)
                        {
                            OpenCreateCharacterDateOfBirth(player, onGoBack);
                        }
                        ;

                        _dialogService.Show(player.Entity, incorrectFormatDialog, IncorrectFormatDialogHandler);
                    }
                }
                else
                {
                    onGoBack?.Invoke();
                }
            }

            _dialogService.Show(player.Entity, characterDialog, CreateCharacterDateOfBirthDialogHandler);
        }
    }
}
