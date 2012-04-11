using System;
using System.Windows;

namespace Orbit.Core.Scene.Entities
{
    public interface IMovable
    {
        Vector GetDirection();

        void SetDirection(Vector direction);
    }
}
