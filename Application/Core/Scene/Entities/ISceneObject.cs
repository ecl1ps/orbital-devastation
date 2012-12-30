using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Windows.Shapes;
using System.Collections.Generic;
using Orbit.Core.Client;
using Orbit.Core.Scene.CollisionShapes;

namespace Orbit.Core.Scene.Entities
{
    public interface ISceneObject
    {
        long Id { get; set; }

        Vector Position { get; set; }

        Vector Center { get; }

        bool Dead { get; set; }

        SceneMgr SceneMgr { get; set; }

        ICollisionShape CollisionShape { get; set; }
        
        bool Enabled { get; set; }

        bool Visible { get; set; }

        void Update(float tpf);

        void AddControl(IControl control);

        void RemoveControlsOfType<T>();

        void RemoveControl(IControl control);

        T GetControlOfType<T>();

        List<T> GetControlsOfType<T>();

        IList<IControl> GetControlsCopy();

        bool IsOnScreen(Size screenSize);

        void UpdateGeometric();

        UIElement GetGeometry();

        void SetGeometry(UIElement geometryElement);

        void DoRemove(ISceneObject obj);

        void DoRemoveMe();


        /// hooks

        void OnRemove();

        void OnAttach();
    }
}
