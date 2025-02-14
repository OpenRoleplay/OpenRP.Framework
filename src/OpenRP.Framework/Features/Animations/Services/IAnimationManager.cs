using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Animations.Services
{
    public interface IAnimationManager
    {
        void PreloadAnimLibs(Player player);
    }
}
