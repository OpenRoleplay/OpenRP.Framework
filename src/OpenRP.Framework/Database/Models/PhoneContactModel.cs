using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A name in a phone's phone book.
    /// </summary>
    public class PhoneContactModel : BaseModel
    {
        public ulong PhoneId { get; set; }
        public string Name { get; set; }
        public uint Number { get; set; }

        // Navigational Properties
        public PhoneModel? Phone { get; set; }

        public PhoneContactModel()
        {
            Name = string.Empty;
        }
    }
}
