using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.ActorRoleplay.Entities;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Banking.Helpers;
using OpenRP.Framework.Features.Banking.Services;
using OpenRP.Framework.Features.Banking.Services.Dialogs;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Banking.Actions
{
    /// <summary>
    /// A teller opening one bank dialog for a customer who asked for it. The teller never moves money: the customer
    /// fills in and confirms the dialog, which checks everything again.
    /// </summary>
    public class BankTellerAction : IActorAction
    {
        private readonly BankOperation _operation;
        private readonly bool _opensFullMenu;
        private readonly ITellerService _tellerService;
        private readonly IBankService _bankService;
        private readonly IBankDialogService _bankDialogService;

        public string Name { get; }
        public string Description { get; }
        public string? DetailDescription { get; }
        public string RequiredPermission => BankingRules.TellerPermissions[_operation];

        public BankTellerAction(string name, string description, string? detailDescription, BankOperation operation, bool opensFullMenu,
            ITellerService tellerService, IBankService bankService, IBankDialogService bankDialogService)
        {
            Name = name;
            Description = description;
            DetailDescription = detailDescription;
            _operation = operation;
            _opensFullMenu = opensFullMenu;
            _tellerService = tellerService;
            _bankService = bankService;
            _bankDialogService = bankDialogService;
        }

        /// <summary>
        /// Only a teller standing inside a branch of its own bank.
        /// </summary>
        public bool IsAvailableFor(ServerActor actor)
        {
            BankModel? bank = _tellerService.GetBank(actor);
            return bank != null && _bankService.IsInsideBranch(actor, bank.Id);
        }

        public void Perform(ServerActor actor, Player player, string? detail)
        {
            string? error = _tellerService.GetServiceError(actor, player, _operation);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            if (_opensFullMenu)
            {
                _bankDialogService.OpenBankMenu(player, actor);
                return;
            }
            _bankDialogService.OpenOperation(player, actor, _operation, BankAmounts.Parse(detail));
        }
    }
}
