using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A property that's a branch of a bank. A bank's tellers only serve inside its branches.
    /// </summary>
    public class BankBranchModel : BaseModel
    {
        public ulong BankId { get; set; }
        public ulong PropertyId { get; set; }

        // Navigational Properties
        public BankModel? Bank { get; set; }
        public PropertyModel? Property { get; set; }
    }
}
