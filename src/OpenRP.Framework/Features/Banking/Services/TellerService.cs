using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Actors.Services;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Linq;

namespace OpenRP.Framework.Features.Banking.Services
{
    public class TellerService : ITellerService
    {
        // A customer stands at the counter, within a few steps of the teller.
        private const float ServiceRange = 4.0f;

        private readonly IEntityManager _entityManager;
        private readonly IBankService _bankService;
        private readonly IActorPermissionService _actorPermissionService;

        public TellerService(IEntityManager entityManager, IBankService bankService, IActorPermissionService actorPermissionService)
        {
            _entityManager = entityManager;
            _bankService = bankService;
            _actorPermissionService = actorPermissionService;
        }

        public ServerActor? GetNearbyTeller(Player player)
        {
            return _entityManager.GetComponents<ServerActor>()
                .Where(actor => !actor.IsDeleted() && IsNear(actor, player))
                .OrderBy(actor => Vector3.Distance(actor.GetActor()!.Position, player.Position))
                .FirstOrDefault(actor => GetBank(actor) != null);
        }

        public string? GetServiceError(ServerActor teller, Player customer, BankOperation operation)
        {
            if (!teller.IsComponentAlive || teller.IsDeleted() || !customer.IsComponentAlive)
                return "The teller isn't there anymore.";
            if (!IsNear(teller, customer))
                return $"You need to stand at the counter, near {teller.GetName()}.";

            BankModel? bank = GetBank(teller);
            if (bank == null)
                return $"{teller.GetName()} doesn't work for a bank.";
            if (!_bankService.IsInsideBranch(teller, bank.Id))
                return $"{teller.GetName()} isn't at a branch of {bank.Name}.";
            return IsAllowed(teller, operation) ? null : $"{teller.GetName()} can't help you with that here.";
        }

        public bool IsAllowed(ServerActor teller, BankOperation operation)
        {
            return _actorPermissionService.HasPermission(teller, BankingRules.TellerPermissions[operation]);
        }

        public BankModel? GetBank(ServerActor teller)
        {
            return _bankService.GetTellerBank(teller);
        }

        private static bool IsNear(ServerActor teller, Player player)
        {
            Actor? actor = teller.GetActor();
            return actor != null
                && actor.VirtualWorld == player.VirtualWorld
                && Vector3.Distance(actor.Position, player.Position) <= ServiceRange;
        }
    }
}
