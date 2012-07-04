using System;
using System.Windows;

namespace Orbit.Core.Scene.Entities
{
    public interface IRotable : ISceneObject
    {
        float Rotation { get; set; }
    }
}
