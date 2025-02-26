using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Chat.Constants
{
    public class PlayerInfoMessagePrefix
    {
        public const string INFO = ChatColor.White + "[" + ChatColor.CornflowerBlue + "INFO" + ChatColor.White + "] ";
        public const string ERROR = ChatColor.White + "[" + ChatColor.Red + "ERROR" + ChatColor.White + "] ";
        public const string SYNTAX = ChatColor.White + "[" + ChatColor.ChromeYellow + "SYNTAX" + ChatColor.White + "] ";
        public const string DEBUG = ChatColor.White + "[" + ChatColor.ButterflyBlue + "DEBUG" + ChatColor.White + "] ";
        public const string ADMIN = ChatColor.White + "[" + ChatColor.EstorilBlue + "ADMIN" + ChatColor.White + "] ";
    }
}
