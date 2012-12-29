using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Orbit.Core.Scene.Entities
{
    public interface IHpBar : ISceneObject
    {
        float Percentage { get; set; }

        Color Color { get; set; }
    }
}
