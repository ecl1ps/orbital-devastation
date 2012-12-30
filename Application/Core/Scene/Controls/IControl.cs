using System;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    public interface IControl
    {
        void Init(ISceneObject me);

        Boolean Enabled { get; set; }

        void OnSceneObjectRemove();

        void AddSceneObjectRemoveAction(Action a);

        void Destroy();
        void OnRemove();
    }
}
