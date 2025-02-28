using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Accounts.Services;
using OpenRP.Framework.Features.ActorConversations.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Accounts.Services.Dialogs;
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Features.MainMenu.Services.Dialogs
{
    public class MainMenuDialogService : IMainMenuDialogService
    {
        private IDialogService _dialogService;
        private IAccountService _accountService;
        private IAccountDialogService _accountDialogService;
        public MainMenuDialogService(IDialogService dialogService, IAccountService accountService, IAccountDialogService accountDialogService)
        {
            _dialogService = dialogService;
            _accountService = accountService;
            _accountDialogService = accountDialogService;
        }

        public void OpenMainMenu(Player player)
        {
            BetterMessageDialog betterMessageDialog = new BetterMessageDialog("Continue");
            betterMessageDialog.SetNewLinesAtLength(50);
            betterMessageDialog.SetTitle(TitleType.Parents, $"Welcome to {ChatColor.Highlight}Open Roleplay{ChatColor.White}!");
            betterMessageDialog.SetContent($"{ChatColor.CornflowerBlue}Welcome to Open Roleplay!{ChatColor.White}\r\nStep into an ambitious roleplaying server designed to push the boundaries of San Andreas Multiplayer. Open Roleplay offers an immersive world where every choice matters and every story is worth telling.\r\n\r\n{ChatColor.CornflowerBlue}A Unique Setting{ChatColor.White}  \r\nInstead of a US state, we roleplay in the {ChatColor.Highlight}United States of San Andreas{ChatColor.White}, a fictional nation with its own laws, politics, and identity, that's meant to be a parody of the US. For those craving a grittier experience, there's also {ChatColor.Highlight}Caeroyna{ChatColor.White}, a more challenging parody of Russia.\r\n\r\n{ChatColor.CornflowerBlue}Breaking Boundaries{ChatColor.White}  \r\nFrom a dynamic {ChatColor.Highlight}Biome Generator{ChatColor.White} to AI-driven actors, and RPG elements like {ChatColor.Highlight}inventories, crafting, and skills{ChatColor.White}, every feature enhances the strict roleplay experience.\r\n\r\n{ChatColor.CornflowerBlue}Open in Name, Open in Spirit{ChatColor.White}  \r\nWe believe in transparency, community feedback, and a world shaped by player actions. While not open-source, we're always open to collaboration and fresh ideas.\r\n\r\nYour story starts here. Welcome to {ChatColor.Highlight}Open Roleplay{ChatColor.White}.");

            void MainMenuDialogHandler(MessageDialogResponse r)
            {
                OpenMainMenuChoiceMenu(player);
            }

            _dialogService.Show(player, betterMessageDialog, MainMenuDialogHandler);
        }

        public void OpenMainMenuChoiceMenu(Player player)
        {
            BetterListDialog betterListDialog = new BetterListDialog("Continue", "Exit");
            betterListDialog.SetTitle(TitleType.Parents, $"Welcome to {ChatColor.Highlight}Open Roleplay{ChatColor.White}!");

            string characterName = player.Name;
            List<AccountModel>? characterAccounts = _accountService.GetAccountsByCharacterName(characterName);
            AccountModel? account = _accountService.GetAccountByUsername(player.Name);

            int loginToCurrentAccountIndex = -1;
            if (account != null)
            {
                loginToCurrentAccountIndex = betterListDialog.AddRow($"Log in to username {ChatColor.Highlight}{account.Username}");
            }

            int[] characterAccountsIndex = new int[0];
            if (characterAccounts != null)
            {
                characterAccountsIndex = new int[characterAccounts.Count];
                int charAccIndex = 0;
                foreach (AccountModel characterAccount in characterAccounts)
                {
                    characterAccountsIndex[charAccIndex] = betterListDialog.AddRow($"Log in to username {ChatColor.Highlight}{characterAccount.Username}");
                    charAccIndex++;
                }
            }
            int loginToDifferentUsername = betterListDialog.AddRow($"Log in to a different username");
            int createNewAccount = betterListDialog.AddRow($"Create a new account");

            void MainMenuChoiceDialogHandler(ListDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    if (r.ItemIndex == loginToCurrentAccountIndex) // Log in to [CURRENT USERNAME]
                    {
                        _accountDialogService.OpenLoginAskPasswordDialog(player, player.Name, () => OpenMainMenuChoiceMenu(player));
                    }
                    else if (r.ItemIndex == loginToDifferentUsername) // Log in to a different username
                    {
                        _accountDialogService.OpenLoginAskUsernameDialog(player, () => OpenMainMenuChoiceMenu(player));
                    }
                    else if (r.ItemIndex == createNewAccount) // Create a new account
                    {
                        _accountDialogService.OpenRegistrationAskUsernameDialog(player, () => OpenMainMenuChoiceMenu(player));
                    }
                    else
                    {
                        if (characterAccounts != null)
                        {
                            for (int i = 0; i < characterAccounts.Count; i++)
                            {
                                if (r.ItemIndex == characterAccountsIndex[i])
                                {
                                    _accountDialogService.OpenLoginAskPasswordDialog(player, characterAccounts.ElementAt(i).Username, () => OpenMainMenuChoiceMenu(player));
                                }
                            }
                        }
                    }
                }
                else
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "Hope to see you back!");
                    player.Kick();
                }
            }

            _dialogService.Show(player, betterListDialog, MainMenuChoiceDialogHandler);
        }
    }

}
