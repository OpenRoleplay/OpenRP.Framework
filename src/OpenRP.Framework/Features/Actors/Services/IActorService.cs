using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Actors.Services
{
    public interface IActorService
    {
        List<ActorModel> GetActors();
        void LoadActors();
    }
}
