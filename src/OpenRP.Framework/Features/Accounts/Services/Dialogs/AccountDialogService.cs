using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Accounts.Components;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.ActorConversations.Services;
using OpenRP.Framework.Database;
using OpenRP.Framework.Features.MainMenu.Services.Dialogs;

namespace OpenRP.Framework.Features.Accounts.Services.Dialogs
{
    public class AccountDialogService : IAccountDialogService
    {
        private IDialogService _dialogService;
        private IAccountService _accountService;
        private IActorConversationWithPlayerManager _actorConversationWithPlayerManager;
        public AccountDialogService(IDialogService dialogService, IAccountService accountService, IActorConversationWithPlayerManager actorConversationWithPlayerManager)
        {
            _dialogService = dialogService;
            _accountService = accountService;
            _actorConversationWithPlayerManager = actorConversationWithPlayerManager;
        }
        public void OpenLoginAskUsernameDialog(Player player, Action onGoBack)
        {
            BetterInputDialog loginDialog = new BetterInputDialog("Continue", "Go Back");
            loginDialog.SetTitle(TitleType.Children, "Login", "Username");
            loginDialog.SetContent(
                $"Before we dive into the world of {ChatColor.Highlight}Open Roleplay{ChatColor.White}, " +
                "we need to know who you are. Please enter your " +
                $"{ChatColor.Highlight}username{ChatColor.White} to continue.\r\n\r\n" +
                $"{ChatColor.CornflowerBlue}Type your username below:{ChatColor.White}");
            loginDialog.SetNewLinesAtLength(50);

            void StepOneDialogHandler(InputDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    if (_accountService.DoesAccountExist(r.InputText))
                    {
                        OpenLoginAskPasswordDialog(player, r.InputText, onGoBack);
                    }
                    else
                    {
                        BetterMessageDialog accountDoesNotExist = new BetterMessageDialog("Retry");
                        accountDoesNotExist.SetTitle(TitleType.Children, "Login", $"{ChatColor.Red}Login Failed{ChatColor.White}");
                        accountDoesNotExist.SetContent(
                            "There is " + $"{ChatColor.Highlight}no account{ChatColor.White} associated with the username you entered.\r\n\r\n" +
                            "Please double-check your " + $"{ChatColor.Highlight}username{ChatColor.White} for any typos or errors.\r\n\r\n" +
                            "If you're " + $"{ChatColor.Highlight}new here{ChatColor.White}, you can create an account by selecting the " +
                            $"{ChatColor.Highlight}\"Create a new account\"{ChatColor.White} option.\r\n\r\n" +
                            $"{ChatColor.CornflowerBlue}Need Help?{ChatColor.White} Reach out to our team on " +
                            $"{ChatColor.Highlight}Discord{ChatColor.White} for assistance.\r\n\r\n" +
                            $"{ChatColor.CornflowerBlue}Please try again:{ChatColor.White}");

                        void RetryUsernameDialogHandler(MessageDialogResponse response)
                        {
                            OpenLoginAskUsernameDialog(player, onGoBack);
                        };

                        _dialogService.Show(player.Entity, accountDoesNotExist, RetryUsernameDialogHandler);
                    }
                }
                else
                {
                    onGoBack?.Invoke();
                }
            }

            _dialogService.Show(player.Entity, loginDialog, StepOneDialogHandler);
        }

        public void OpenLoginAskPasswordDialog(Player player, string username, Action onGoBack)
        {
            BetterInputDialog loginDialog = new BetterInputDialog("Continue", "Go Back");
            loginDialog.SetTitle(TitleType.Children, "Login", "Password");
            loginDialog.SetContent($"You're logging into the account with the username {ChatColor.Highlight}{username}{ChatColor.White}. \r\n\r\nPlease enter your {ChatColor.Highlight}password{ChatColor.White} to continue.\r\n\r\n{ChatColor.CornflowerBlue}Note:{ChatColor.White} Your password is {ChatColor.Highlight}case-sensitive{ChatColor.White}, so make sure you're entering it correctly.\r\n\r\n{ChatColor.CornflowerBlue}Type your password below:{ChatColor.White}");
            loginDialog.SetNewLinesAtLength(50);
            loginDialog.IsPassword = true;

            void StepTwoDialogHandler(InputDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    if (_accountService.CheckPassword(username, r.InputText))
                    {
                        _accountService.LoginPlayer(player, username);
                    }
                    else
                    {
                        OpenLoginAskUsernameDialog(player, onGoBack);
                    }
                }
                else
                {
                    onGoBack?.Invoke();
                }
            }

            _dialogService.Show(player.Entity, loginDialog, StepTwoDialogHandler);
        }

        public void OpenRegistrationAskUsernameDialog(Player player, Action onGoBack)
        {
            BetterInputDialog registerDialog = new BetterInputDialog("Continue", "Go Back");
            registerDialog.SetTitle(TitleType.Children, "Account Creation", "Username");
            registerDialog.SetContent($"Welcome to {ChatColor.Highlight}Open Roleplay{ChatColor.White}! Let's get started by creating your account.\r\n\r\nPlease enter your {ChatColor.Highlight}desired username{ChatColor.White}. This will be your {ChatColor.Highlight}account name{ChatColor.White} and cannot be changed later.\r\n\r\n{ChatColor.CornflowerBlue}Username Requirements:{ChatColor.White}\r\n- Must be between {ChatColor.Highlight}3 and 24 characters{ChatColor.White}.\r\n- Can only contain {ChatColor.Highlight}letters{ChatColor.White}, {ChatColor.Highlight}numbers{ChatColor.White}, and {ChatColor.Highlight}underscores (_){ChatColor.White}.\r\n- Should not include {ChatColor.Highlight}offensive{ChatColor.White} or {ChatColor.Highlight}inappropriate words{ChatColor.White}.");
            registerDialog.SetNewLinesAtLength(50);

            void StepOneDialogHandler(InputDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    if (r.InputText.Length < 3)
                    {
                        BetterMessageDialog usernameLongerThanThreeCharactersDialog = new BetterMessageDialog("Retry");
                        usernameLongerThanThreeCharactersDialog.SetTitle(TitleType.Children, "Account Creation", $"{ChatColor.Red}Invalid Username{ChatColor.White}");
                        usernameLongerThanThreeCharactersDialog.SetContent($"Your chosen username is {ChatColor.Highlight}too short{ChatColor.White}. \r\n\r\n{ChatColor.CornflowerBlue}Username Requirements:{ChatColor.White}\r\n- Must be between {ChatColor.Highlight}3 and 24 characters{ChatColor.White}.\r\n- Can only contain {ChatColor.Highlight}letters{ChatColor.White}, {ChatColor.Highlight}numbers{ChatColor.White}, and {ChatColor.Highlight}underscores (_){ChatColor.White}.\r\n- Should not include {ChatColor.Highlight}offensive{ChatColor.White} or {ChatColor.Highlight}inappropriate words{ChatColor.White}.\r\n\r\nPlease {ChatColor.Highlight}try again{ChatColor.White} and choose a username that meets these requirements.");
                        usernameLongerThanThreeCharactersDialog.SetNewLinesAtLength(50);

                        void UsernameLongerThanThreeCharactersDialogHandler(MessageDialogResponse r)
                        {
                            OpenRegistrationAskUsernameDialog(player, onGoBack);
                        };

                        _dialogService.Show(player, usernameLongerThanThreeCharactersDialog, UsernameLongerThanThreeCharactersDialogHandler);
                    }

                    if (r.InputText.Length > 24)
                    {
                        BetterMessageDialog usernameNoLongerThanTwentyFourCharactersDialog = new BetterMessageDialog("Retry");
                        usernameNoLongerThanTwentyFourCharactersDialog.SetTitle(TitleType.Children, "Account Creation", $"{ChatColor.Red}Invalid Username{ChatColor.White}");
                        usernameNoLongerThanTwentyFourCharactersDialog.SetContent($"Your chosen username is {ChatColor.Highlight}too long{ChatColor.White}. \r\n\r\n{ChatColor.CornflowerBlue}Username Requirements:{ChatColor.White}\r\n- Must be between {ChatColor.Highlight}3 and 24 characters{ChatColor.White}.\r\n- Can only contain {ChatColor.Highlight}letters{ChatColor.White}, {ChatColor.Highlight}numbers{ChatColor.White}, and {ChatColor.Highlight}underscores (_){ChatColor.White}.\r\n- Should not include {ChatColor.Highlight}offensive{ChatColor.White} or {ChatColor.Highlight}inappropriate words{ChatColor.White}.\r\n\r\nPlease {ChatColor.Highlight}try again{ChatColor.White} and choose a username that meets these requirements.");
                        usernameNoLongerThanTwentyFourCharactersDialog.SetNewLinesAtLength(50);

                        void UsernameNoLongerThanTwentyFourCharactersDialogHandler(MessageDialogResponse r)
                        {
                            OpenRegistrationAskUsernameDialog(player, onGoBack);
                        };

                        _dialogService.Show(player.Entity, usernameNoLongerThanTwentyFourCharactersDialog, UsernameNoLongerThanTwentyFourCharactersDialogHandler);
                    }

                    if (_accountService.DoesAccountExist(r.InputText))
                    {
                        OpenRegistrationUsernameAlreadyExistsDialog(player, onGoBack);
                    }
                    else
                    {
                        player.DestroyComponents<Account>();
                        player.DestroyComponents<AccountCreation>();
                        AccountCreation accountComponent = player.AddComponent<AccountCreation>();

                        accountComponent.Account = new AccountModel();
                        accountComponent.Account.Username = r.InputText;

                        BetterMessageDialog usernameSet = new BetterMessageDialog("Continue");
                        usernameSet.SetTitle(TitleType.Children, "Account Creation", "Username Set");
                        usernameSet.SetContent($"Your username has been {ChatColor.Highlight}successfully set{ChatColor.White} to: {ChatColor.Highlight}\"{r.InputText}\"{ChatColor.White}.\r\n\r\nLet's move on to the {ChatColor.Highlight}next step{ChatColor.White} in creating your account.");
                        usernameSet.SetNewLinesAtLength(50);

                        void GoToStepTwoDialogHandler(MessageDialogResponse r)
                        {
                            OpenRegistrationAskPasswordDialog(player, onGoBack);
                        };

                        _dialogService.Show(player.Entity, usernameSet, GoToStepTwoDialogHandler);
                    }
                }
                else
                {
                    onGoBack?.Invoke();
                }
            }

            _dialogService.Show(player.Entity, registerDialog, StepOneDialogHandler);
        }

        public void OpenRegistrationUsernameAlreadyExistsDialog(Player player, Action onGoBack)
        {
            BetterMessageDialog usernameExistsDialog = new BetterMessageDialog("Retry");
            usernameExistsDialog.SetTitle(TitleType.Children, "Account Creation", $"{ChatColor.Red}Username Already Taken{ChatColor.White}");
            usernameExistsDialog.SetContent($"Your chosen username is {ChatColor.Highlight}already in use{ChatColor.White}.  \r\n\r\nPlease {ChatColor.Highlight}choose a different username{ChatColor.White} to continue with the registration process.  \r\n\r\n{ChatColor.CornflowerBlue}Tip:{ChatColor.White} Try adding {ChatColor.Highlight}numbers{ChatColor.White} or a {ChatColor.Highlight}unique twist{ChatColor.White} to your preferred username.");
            usernameExistsDialog.SetNewLinesAtLength(50);

            void UsernameAlreadyExistsDialogHandler(MessageDialogResponse r)
            {
                OpenRegistrationAskUsernameDialog(player, onGoBack);
            }

            _dialogService.Show(player.Entity, usernameExistsDialog, UsernameAlreadyExistsDialogHandler);
        }

        public void OpenRegistrationAskPasswordDialog(Player player, Action onGoBack)
        {
            BetterInputDialog registerDialog = new BetterInputDialog("Continue", "Go Back");
            registerDialog.SetTitle(TitleType.Children, "Account Creation", "Password");
            registerDialog.SetContent($"Please choose a {ChatColor.Highlight}secure password{ChatColor.White} for your account.\r\n\r\nMake sure it's something you can {ChatColor.Highlight}remember{ChatColor.White}, but {ChatColor.Highlight}hard for others to guess{ChatColor.White}.");
            registerDialog.SetNewLinesAtLength(50);
            registerDialog.IsPassword = true;

            void StepTwoDialogHandler(InputDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    if (r.InputText.Length < 8)
                    {
                        BetterMessageDialog passwordMustBeLongerThanEightCharactersDialog = new BetterMessageDialog("Retry");
                        passwordMustBeLongerThanEightCharactersDialog.SetTitle(TitleType.Children, "Account Creation", $"{ChatColor.Red}Password Too Short{ChatColor.White}");
                        passwordMustBeLongerThanEightCharactersDialog.SetContent($"Your password must be at least {ChatColor.Highlight}8 characters long{ChatColor.White}.\r\n\r\nPlease choose a {ChatColor.Highlight}stronger password{ChatColor.White} to keep your account secure.");
                        passwordMustBeLongerThanEightCharactersDialog.SetNewLinesAtLength(50);

                        void PasswordMustBeLongerThanEightCharactersDialogHandler(MessageDialogResponse r)
                        {
                            OpenRegistrationAskPasswordDialog(player, onGoBack);
                        };

                        _dialogService.Show(player.Entity, passwordMustBeLongerThanEightCharactersDialog, PasswordMustBeLongerThanEightCharactersDialogHandler);
                        return;
                    }

                    AccountCreation accountCreation = player.GetComponent<AccountCreation>();

                    accountCreation.Account.Password = BCrypt.Net.BCrypt.HashPassword(r.InputText, 11);

                    BetterMessageDialog passwordSet = new BetterMessageDialog("Continue");
                    passwordSet.SetTitle(TitleType.Children, "Account Creation", "Password Set");
                    passwordSet.SetContent($"Your password has been {ChatColor.Highlight}successfully set{ChatColor.White}.\r\n\r\nLet's move on to the {ChatColor.Highlight}next step{ChatColor.White} in creating your account.");
                    passwordSet.SetNewLinesAtLength(50);

                    void PasswordSetHandler(MessageDialogResponse r)
                    {
                        OpenRegistrationAskPasswordConfirmationDialog(player, onGoBack);
                    };

                    _dialogService.Show(player.Entity, passwordSet, PasswordSetHandler);
                    return;
                }
                else
                {
                    OpenRegistrationAskUsernameDialog(player, onGoBack);
                }
            }

            _dialogService.Show(player.Entity, registerDialog, StepTwoDialogHandler);
        }

        public void OpenRegistrationAskPasswordConfirmationDialog(Player player, Action onGoBack)
        {
            BetterInputDialog registerDialog = new BetterInputDialog("Continue", "Go Back");
            registerDialog.SetTitle(TitleType.Children, "Account Creation", "Password Confirmation");
            registerDialog.SetContent($"Please re-enter your {ChatColor.Highlight}password{ChatColor.White} to ensure it matches the one you just set.\r\n\r\nThis step helps to {ChatColor.Highlight}secure your account{ChatColor.White} and prevent typos.");
            registerDialog.SetNewLinesAtLength(50);
            registerDialog.IsPassword = true;

            void StepTwoDialogHandler(InputDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    AccountCreation accountCreation = player.GetComponent<AccountCreation>();

                    if (BCrypt.Net.BCrypt.Verify(r.InputText, accountCreation?.Account?.Password))
                    {
                        MessageDialog passwordSetDialog = new MessageDialog(DialogHelper.GetTitle("Registration", "Password Confirmation"), ChatColor.White + "Your password has been set. You may now log in to your account.", DialogHelper.Next);

                        void PasswordSetDialogHandler(MessageDialogResponse r)
                        {
                            if (_accountService.CreateAccount(player, accountCreation.Account.Username, accountCreation.Account.Password))
                            {
                                OpenLoginAskPasswordDialog(player, accountCreation.Account.Username, onGoBack);
                            } else
                            {
                                onGoBack?.Invoke();
                            }
                        };

                        _dialogService.Show(player.Entity, passwordSetDialog, PasswordSetDialogHandler);
                    }
                    else
                    {
                        BetterMessageDialog passwordsDoNotMatchDialog = new BetterMessageDialog("Retry");
                        passwordsDoNotMatchDialog.SetTitle(TitleType.Children, "Account Creation", $"{ChatColor.Red}Passwords Do Not Match{ChatColor.White}");
                        passwordsDoNotMatchDialog.SetContent($"The {ChatColor.Highlight}passwords{ChatColor.White} you entered do not match.\r\n\r\nPlease make sure both passwords are {ChatColor.Highlight}identical{ChatColor.White} and try again. ");
                        passwordsDoNotMatchDialog.SetNewLinesAtLength(50);

                        void PasswordsDoNotMatchDialogHandler(MessageDialogResponse r)
                        {
                            OpenRegistrationAskPasswordDialog(player, onGoBack);
                        };

                        _dialogService.Show(player.Entity, passwordsDoNotMatchDialog, PasswordsDoNotMatchDialogHandler);
                    }
                }
                else
                {
                    OpenRegistrationAskPasswordDialog(player, onGoBack);
                }
            }

            _dialogService.Show(player.Entity, registerDialog, StepTwoDialogHandler);
        }
    }
}
