using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.SpecialActions.Spectator
{
    public interface ISpectatorAction : ISpecialAction
    {
        float CastingTime { get; set; }

        float Percentage { get; }

        RangeGroup Normal { get; set; }

        RangeGroup Gold { get; set; }

        int ComputeMissing(RangeGroup range);

    }
}
