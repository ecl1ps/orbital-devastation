using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;

namespace Orbit.src.Core.weapons
{
    interface IWeapon
    {
        ISceneObject bullet { get; set; }

    }
}
