using System;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    public interface IControl
    {
        Boolean Enabled { get; set; }

        void OnControlDestroy();

        void addControlDestroyAction(Action a);
    }
}
