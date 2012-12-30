using System;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    public interface IControl
    {
        Boolean Enabled { get; set; }

        void OnSceneObjectRemove();

        void AddSceneObjectRemoveAction(Action a);

        void OnRemove();
    }
}
