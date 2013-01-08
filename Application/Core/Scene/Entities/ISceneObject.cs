using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using Orbit.Core.Client;
using Orbit.Core.Scene.CollisionShapes;
using System.Windows.Media;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Scene.Entities
{
    public interface ISceneObject : IUpdatable
    {
        long Id { get; set; }

        Vector Position { get; set; }

        Vector Center { get; }

        bool Dead { get; set; }

        SceneMgr SceneMgr { get; set; }

        ICollisionShape CollisionShape { get; set; }
        
        bool Enabled { get; set; }

        bool Visible { get; set; }

        DrawingCategory Category { get; set; }

        void AddControl(IControl control);

        void RemoveControlsOfType<T>();

        void RemoveControl(IControl control);

        T GetControlOfType<T>();

        bool HasControlOfType<T>();

        List<T> GetControlsOfType<T>();

        IList<IControl> GetControlsCopy();

        void ToggleControls(bool p, Control except = null);

        bool IsOnScreen(Size screenSize);

        void UpdateGeometric();

        DrawingGroup GetGeometry();

        void SetGeometry(DrawingGroup geometryElement);

        /// helpers

        void DoRemove(ISceneObject obj);

        void DoRemoveMe();

        List<ISceneObject> FindNearbyObjects(double radius);

        /// hooks

        void OnRemove();

        void OnAttach();


    }
}
