using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Scene.Entities
{
    public interface IHpBar : ISceneObject
    {
        float Percentage { get; set; }
    }
}
