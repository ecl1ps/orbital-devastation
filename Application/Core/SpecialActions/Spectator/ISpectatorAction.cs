using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.SpecialActions.Spectator
{
    public interface ISpectatorAction : ISpecialAction
    {
        float Percentage { get; }

        int Normal { get; }

        int Gold { get; }

        int MissingNormal { get; }

        int MissingGold { get; }
    }
}
