using System;
using System.Windows;

namespace Orbit.Core.Scene.Entities
{
    public interface IRotable
    {
        int GetRotation();

        void SetRotation(int angle);
    }
}
