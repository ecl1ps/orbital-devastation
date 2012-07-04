using System;
using System.Windows;

namespace Orbit.Core.Scene.Entities
{
    public interface IMovable : ISceneObject
    {
        Vector Direction { get; set; }
    }
}
