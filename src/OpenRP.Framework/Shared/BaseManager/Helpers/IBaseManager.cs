using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.BaseManager.Helpers
{
    public interface IBaseManager
    {
        /// <summary>
        /// Loads every row as a component. Called once at startup.
        /// </summary>
        int LoadAll();

        /// <summary>
        /// Saves this server's changes, then picks up rows changed or deleted elsewhere. Called on a timer.
        /// </summary>
        void ProcessChanges();

        /// <summary>
        /// Saves this server's changes right now, without picking up others. Called on shutdown.
        /// </summary>
        void SaveNow();
    }
}
