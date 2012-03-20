using System;
using System.Windows;
using Orbit.Core.Scene.Controls;

namespace Orbit.Core.Scene.Entities
{
    public interface ISceneObject
    {
        void Update(float tpf);

        void Render();

        long GetId();

        void Setid(long id);

        Vector GetPosition();

        void SetPosition(Vector position);

        void AddControl(IControl control);

        void RemoveControl(Type type);

        IControl GetControlOfType(Type type);

        bool IsOnScreen();
    }
}
