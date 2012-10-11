using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Windows.Shapes;
using System.Collections.Generic;
using Orbit.Core.Client;

namespace Orbit.Core.Scene.Entities
{
    public interface ISceneObject
    {
        long Id { get; set; }

        Vector Position { get; set; }

        bool Dead { get; set; }

        SceneMgr SceneMgr { get; set; }
        
        bool Enabled { get; set; }

        bool Visible { get; set; }

        void Update(float tpf);

        void AddControl(Control control);

        void RemoveControl(Type type);

        void RemoveControl(Control control);

        IControl GetControlOfType(Type type);

        List<IControl> GetControlsOfType(Type type);

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
