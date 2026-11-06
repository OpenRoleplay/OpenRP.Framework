using OpenRP.Framework.Database.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class AccountModel : BaseModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public uint Level { get; set; }
        public uint Experience { get; set; }

        /// <summary>
        /// True while the account is logged in on a server. Only counts while LastSeenAt is recent: a crashed server
        /// can't reset it, so it expires instead.
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// When the server the account is logged in on last confirmed it's still online (UTC).
        /// </summary>
        public DateTime? LastSeenAt { get; set; }

        // Navigational Properties
        public List<CharacterModel> Characters { get; set; }

        // Constuctor
        public AccountModel()
        {
            Level = 1;
            Experience = 0;
            Characters = new List<CharacterModel>();
        }
    }
}
