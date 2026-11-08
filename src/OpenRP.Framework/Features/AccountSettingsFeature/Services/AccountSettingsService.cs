using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Accounts.Components;
using OpenRP.Framework.Features.AccountSettingsFeature.Components;
using OpenRP.Framework.Features.AccountSettingsFeature.Enums;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Database;
using OpenRP.OpenMp.Streamer;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Features.AccountSettingsFeature.Services
{
    public sealed class AccountSettingsService : IAccountSettingsService
    {
        private readonly IStreamerService _streamerService;
        private readonly IDialogService _dialogService;
        private readonly BaseDataContext _dataContext;
        public AccountSettingsService(IStreamerService streamerService, IDialogService dialogService, BaseDataContext dataContext)
        {
            _streamerService = streamerService;
            _dialogService = dialogService;
            _dataContext = dataContext;
        }

        public AccountSettings? ReloadAccountSettings(Player player)
        {
            Account account = player.GetPlayerCurrentlyLoggedInAccount();

            if (account != null)
            {
                account.DestroyComponents<AccountSettings>();

                AccountSettings accountSettings = GetAccountSettings(player);

                ReloadGraphicPreset(player, accountSettings);

                return accountSettings;
            }
            return null;
        }

        public AccountSettings? GetAccountSettings(Player player)
        {
            Account account = player.GetPlayerCurrentlyLoggedInAccount();

            if (account != null)
            {
                AccountSettings accountSettings = account.GetComponent<AccountSettings>();

                if (accountSettings != null)
                {
                    return accountSettings;
                }

                AccountSettingsModel settings = _dataContext.AccountSettings
                    .FirstOrDefault(i => i.AccountId == account.GetAccountId());

                if (settings == null)
                {
                    settings = new AccountSettingsModel();
                    settings.AccountId = account.GetAccountId();

                    _dataContext.AccountSettings.Add(settings);
                    _dataContext.SaveChanges();
                }

                if (accountSettings == null)
                {
                    accountSettings = account.AddComponent<AccountSettings>(settings);
                    return accountSettings;
                }
            }
            return null;
        }

        public void ReloadGraphicPreset(Player player, AccountSettings accountSettings)
        {
            (int visibleObjects, float radiusMultiplier) = GetStreamerSettings(accountSettings.GetAccountGraphicPreset());
            ApplyStreamerSettings(player, visibleObjects, radiusMultiplier);
        }

        private static (int VisibleObjects, float RadiusMultiplier) GetStreamerSettings(AccountGraphicPreset preset)
        {
            return preset switch
            {
                AccountGraphicPreset.Low => (500, 1.0f),
                AccountGraphicPreset.Medium => (1000, 1.0f),
                AccountGraphicPreset.High => (2000, 1.0f),
                AccountGraphicPreset.VeryHigh => (2000, 1.5f),
                _ => (2000, 2.0f),
            };
        }

        private void ApplyStreamerSettings(Player player, int visibleObjects, float radiusMultiplier)
        {
            _streamerService.SetVisibleItems(StreamerType.Object, visibleObjects, player);
            _streamerService.SetRadiusMultiplier(StreamerType.Object, radiusMultiplier, player);
            _streamerService.Update(player, StreamerType.Object);
        }

        public void SwitchGraphicPreset(Player player, AccountSettings accountSettings)
        {
            ulong databaseId = accountSettings.GetDatabaseId();
            AccountSettingsModel accountSettingsModel = _dataContext.AccountSettings.Find(databaseId);

            AccountGraphicPreset accountGraphicPreset = accountSettings.GetAccountGraphicPreset();
            switch (accountGraphicPreset)
            {
                case AccountGraphicPreset.Low:
                    accountSettingsModel.AccountGraphicPreset = AccountGraphicPreset.Medium;
                    break;
                case AccountGraphicPreset.Medium:
                    accountSettingsModel.AccountGraphicPreset = AccountGraphicPreset.High;
                    break;
                case AccountGraphicPreset.High:
                    accountSettingsModel.AccountGraphicPreset = AccountGraphicPreset.VeryHigh;
                    break;
                case AccountGraphicPreset.VeryHigh:
                    accountSettingsModel.AccountGraphicPreset = AccountGraphicPreset.Ultra;
                    break;
                case AccountGraphicPreset.Ultra:
                    accountSettingsModel.AccountGraphicPreset = AccountGraphicPreset.Low;
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must relog in order to see the decrease in draw distance!");
                    break;
            }

            _dataContext.SaveChanges();

            ReloadAccountSettings(player);
        }

        private string GetCurrentGraphicPresetValue(AccountSettings accountSettings)
        {
            AccountGraphicPreset accountGraphicPreset = accountSettings.GetAccountGraphicPreset();
            switch (accountGraphicPreset)
            {
                case AccountGraphicPreset.Low:
                    return $"{Color.DarkRed}Low";

                case AccountGraphicPreset.Medium:
                    return $"{Color.Yellow}Medium";

                case AccountGraphicPreset.High:
                    return $"{Color.Green}High";

                case AccountGraphicPreset.VeryHigh:
                    return $"{Color.DarkGreen}Very High";

                case AccountGraphicPreset.Ultra:
                    return $"{Color.DarkGreen}Ultra";
            }
            return "N/A";
        }

        public void SwitchGlobalChat(Player player, AccountSettings accountSettings)
        {
            ulong databaseId = accountSettings.GetDatabaseId();
            AccountSettingsModel accountSettingsModel = _dataContext.AccountSettings.Find(databaseId);
            accountSettingsModel.GlobalChatEnabled = !accountSettingsModel.GlobalChatEnabled;
            _dataContext.SaveChanges();
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"OOC chat has been {(accountSettingsModel.GlobalChatEnabled ? "enabled" : "disabled")}.");
            ReloadAccountSettings(player);
        }

        private string GetCurrentGlobalChatValue(AccountSettings accountSettings)
        {
            return accountSettings.GetGlobalChatEnabled() ? $"{Color.Green}Enabled" : $"{Color.DarkRed}Disabled";
        }

        public void SwitchNewbieChat(Player player, AccountSettings accountSettings)
        {
            ulong databaseId = accountSettings.GetDatabaseId();
            AccountSettingsModel accountSettingsModel = _dataContext.AccountSettings.Find(databaseId);
            accountSettingsModel.NewbieChatEnabled = !accountSettingsModel.NewbieChatEnabled;
            _dataContext.SaveChanges();
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Newbie chat has been {(accountSettingsModel.NewbieChatEnabled ? "enabled" : "disabled")}.");
            ReloadAccountSettings(player);
        }

        private string GetCurrentNewbieChatValue(AccountSettings accountSettings)
        {
            return accountSettings.GetNewbieChatEnabled() ? $"{Color.Green}Enabled" : $"{Color.DarkRed}Disabled";
        }

        public void OpenAccountSettingsDialog(Player player)
        {
            AccountSettings accountSettings = player.GetComponent<AccountSettings>();

            BetterTablistDialog tablistDialog = new BetterTablistDialog("Change", "Exit", 2);
            tablistDialog.SetTitle(TitleType.Parents, "Account Settings");
            tablistDialog.AddHeaders("Setting", "Value");

            tablistDialog.AddHeaders("Graphics");
            int graphicPreset = tablistDialog.AddRow("Graphic Preset", GetCurrentGraphicPresetValue(accountSettings));

            tablistDialog.AddHeaders("Chats");
            int globalChat = tablistDialog.AddRow("OOC Chat", GetCurrentGlobalChatValue(accountSettings));
            int newbieChat = tablistDialog.AddRow("Newbie Chat", GetCurrentNewbieChatValue(accountSettings));

            void DialogHandler(TablistDialogResponse r)
            {
                if (r.Response != DialogResponse.LeftButton)
                {
                    return;
                }

                int index = r.ItemIndex;

                // Graphic Preset
                if (index == graphicPreset)
                {
                    SwitchGraphicPreset(player, accountSettings);
                    OpenAccountSettingsDialog(player);
                }

                // OOC Chat Toggle
                if (index == globalChat)
                {
                    SwitchGlobalChat(player, accountSettings);
                    OpenAccountSettingsDialog(player);
                }

                // Newbie Chat Toggle
                if (index == newbieChat)
                {
                    SwitchNewbieChat(player, accountSettings);
                    OpenAccountSettingsDialog(player);
                }
            }

            _dialogService.Show(player, tablistDialog, DialogHandler);
        }
    }
}
