using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Orbit.Core.Scene.Entities
{
    public interface IHeavyWeightSceneObject : IEmpty
    {
        UIElement HeavyWeightGeometry { get; set; }
    }
}
