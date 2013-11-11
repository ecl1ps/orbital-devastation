using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using Orbit.Core.Client;
using Orbit.Core.Scene.CollisionShapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbit.Core.Scene.Entities
{
    public interface ISceneObject : IUpdatable
    {
        long Id { get; set; }

        Vector2 Position { get; set; }

        Vector2 Center { get; }

        Vector2 Direction { get; set; }

        bool Dead { get; set; }

        SceneMgr SceneMgr { get; set; }

        Color Color { get; set; }

        float Rotation { get; set; }

        float Opacity { get; set; }

        ICollisionShape CollisionShape { get; set; }
        
        bool Enabled { get; set; }

        bool Visible { get; set; }

        void AddControl(IControl control);

        void RemoveControlsOfType<T>();

        void RemoveControl(IControl control);

        T GetControlOfType<T>();

        bool HasControlOfType<T>();

        List<T> GetControlsOfType<T>();

        IList<IControl> GetControlsCopy();

        void ToggleControls(bool p, Control except = null);

        bool IsOnScreen(Rectangle screenSize);

        void UpdateGeometric(SpriteBatch spriteBatch);

        /// helpers

        void DoRemove(ISceneObject obj);

        void DoRemoveMe();

        List<ISceneObject> FindNearbyObjects<T>(double radius);

        /// hooks

        void OnRemove();

        void OnAttach();

        void AfterUpdate(float tpf);
    }
}
