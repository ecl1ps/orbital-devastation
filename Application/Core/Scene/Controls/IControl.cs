using System;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    public interface IControl
    {
        void Init(ISceneObject me);

        Boolean Enabled { get; set; }

        void OnControlDestroy();

        void AddControlDestroyAction(Action a);

        void Destroy();
    }
}
