using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Currencies.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Currencies.Commands
{
    public class AcceptPayCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Accept a transaction request from another player.",
            CommandGroups = new string[] { "Currencies" })]
        public void AcceptPay(Player player, IPaymentService paymentService)
        {
            paymentService.AcceptPayment(player);
        }
    }
}
