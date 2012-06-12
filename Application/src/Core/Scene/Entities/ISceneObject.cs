using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Orbit.Core.Scene.Entities
{
    public interface ISceneObject
    {
        long Id { get; set; }

        Vector Position { get; set; }

        bool Dead { get; set; }

        SceneMgr SceneMgr { get; set; }

        void Update(float tpf);

        void AddControl(Control control);

        void RemoveControl(Type type);

        IControl GetControlOfType(Type type);

        IList<IControl> GetControlsCopy();

        bool IsOnScreen(Size screenSize);

        void UpdateGeometric();

        UIElement GetGeometry();

        void SetGeometry(UIElement geometryElement);

        void DoRemove(ISceneObject obj);

        void DoRemoveMe();

        void OnRemove();
    }
}
