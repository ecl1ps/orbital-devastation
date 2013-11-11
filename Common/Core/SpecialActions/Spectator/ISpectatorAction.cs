using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Orbit.Core.SpecialActions.Spectator
{
    public interface ISpectatorAction : ISpecialAction
    {
        float CastingTime { get; set; }

        Color CastingColor { get; set; }

        Range Range { get; set; }

        int RangeCount { get; }

        bool IsExact();

    }
}
