using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Windows.Shapes;

namespace Orbit.Core.Scene.Entities
{
    public interface ISceneObject
    {
        void Update(float tpf);

        long GetId();

        void Setid(long id);

        void SetGeometry(Path path);

        Path GetGeometry();

        Vector GetPosition();

        void SetPosition(Vector position);

        void AddControl(Control control);

        void RemoveControl(Type type);

        IControl GetControlOfType(Type type);

        bool IsOnScreen();

        void UpdateGeometric();
    }
}
