using OpenRP.Framework.Features.Animations.Services;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Animations.Systems
{
    public class AnimationSystem : ISystem
    {
        [Event]
        public void OnPlayerConnect(Player player, IAnimationManager animationManager)
        {
            animationManager.PreloadAnimLibs(player);
        }

        [Event]
        public void OnPlayerSpawn(Player player, IAnimationManager animationManager)
        {
            animationManager.PreloadAnimLibs(player);
        }
    }
}
