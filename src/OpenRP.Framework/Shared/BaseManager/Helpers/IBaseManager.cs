using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.BaseManager.Helpers
{
    public interface IBaseManager
    {
        void ProcessChanges();
        int LoadAll();
    }
}
