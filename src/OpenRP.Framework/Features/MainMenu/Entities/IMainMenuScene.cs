using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.MainMenu.Entities
{
    public interface IMainMenuScene
    {
        void Prepare();
        void Play(Player player);
    }
}
