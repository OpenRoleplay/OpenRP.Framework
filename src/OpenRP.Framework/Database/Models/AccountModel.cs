using OpenRP.Framework.Database.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class AccountModel : BaseEntity
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public uint Level { get; set; }
        public uint Experience { get; set; }

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
