using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.BaseManager.Entities
{
    public interface IDeletable
    {
        bool IsDeleted();
        void ProcessDeletion(bool processDeletion = true);
    }
}
