using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.SpecialActions.Spectator
{
    public interface ISpectatorAction : ISpecialAction
    {
        float Percentage { get; }
    }
}
