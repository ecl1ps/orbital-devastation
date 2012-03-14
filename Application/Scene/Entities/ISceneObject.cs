using System;
using System.Windows;
using Orbit.Scene.Controls;

namespace Orbit.Scene.Entities
{
    public interface ISceneObject
    {
        void Update(float tpf);

        void Render();

        Vector GetPosition();

        void SetPosition(Vector position);

        void AddControl(IControl control);

        void RemoveControl(Type type);

        IControl GetControlOfType(Type type);

        bool IsOnScreen();
    }
}
