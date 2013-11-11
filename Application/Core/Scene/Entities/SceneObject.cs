using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using Orbit.Core.Client;
using Orbit.Core.Scene.CollisionShapes;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using MB.Tools;

namespace Orbit.Core.Scene.Entities
{
    public abstract class SceneObject : ISceneObject, IMovable
    {
        protected List<IControl> controls;


        public long Id { get; set; }
        public ICollisionShape CollisionShape { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        public float Opacity { get; set; }
        public virtual Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public virtual Vector2 Center { get { return Position; } }
        public bool Dead { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public bool Visible { get; set; }
        protected bool enabled = true;
        public virtual bool Enabled {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                foreach (IControl control in controls)
                    control.Enabled = enabled;
            }
        }

        public SceneObject(SceneMgr mgr, long id)
        {
            controls = new List<IControl>();
            Dead = false;
            SceneMgr = mgr;
            Id = id;
            Visible = true;
        }

        public virtual void Update(float tpf)
        {
            if (Enabled)
                UpdateControls(tpf);
        }

        public virtual void AfterUpdate(float tpf)
        {
            if (!Enabled)
                return;

            for (int i = 0; i < controls.Count; ++i)
            {
                (controls[i] as Control).AfterUpdate(tpf);
            }
        }

        private void UpdateControls(float tpf)
        {
            for (int i = 0; i < controls.Count; ++i)
            {
                (controls[i] as Control).Update(tpf);
            }
        }

        public void AddControl(IControl control)
        {
            if (controls.Contains(control))
                return;

            controls.Add(control);
            control.Init(this);
        }

        public void RemoveControlsOfType<T>()
        {
            for (int i = 0; i < controls.Count; ++i)
            {
                if (typeof(T).IsAssignableFrom(controls[i].GetType()))
                {
                    controls[i].OnRemove();
                    controls.RemoveAt(i);
                }
            }
        }

        public T GetControlOfType<T>()
        {
            for (int i = 0; i < controls.Count; ++i)
            {
                if (typeof(T).IsAssignableFrom(controls[i].GetType()))
                    return (T)controls[i];
            }
            return default(T);
        }

        public List<T> GetControlsOfType<T>()
        {
            List<T> res = new List<T>();
            for (int i = 0; i < controls.Count; ++i)
            {
                if (typeof(T).IsAssignableFrom(controls[i].GetType()))
                    res.Add((T)controls[i]);
            }
            return res;
        }

        public IList<IControl> GetControlsCopy()
        {
            IControl[] copyOfControls = new IControl[controls.Count];
            controls.CopyTo(copyOfControls);
            return copyOfControls;
        }

        public bool HasControlOfType<T>()
        {
            for (int i = 0; i < controls.Count; ++i)
            {
                if (typeof(T).IsAssignableFrom(controls[i].GetType()))
                    return true;
            }
            return false;
        }

        public abstract bool IsOnScreen(Rectangle screenSize);

        public virtual void DoRemove(ISceneObject obj)
        {
            SceneMgr.RemoveFromSceneDelayed(obj);

            controls.ForEach(control => control.OnRemove());
            controls.ForEach(control => control.OnSceneObjectRemove());
        }

        public virtual void DoRemoveMe()
        {
            DoRemove(this);
        }

        public List<ISceneObject> FindNearbyObjects<T>(double radius)
        {
            return SceneMgr.GetSceneObjectsInDist<T>(Center, radius);
        }

        public void RemoveControl(IControl control)
        {
            control.OnRemove();
            controls.Remove(control);
        }

        public void ToggleControls(bool enable, Control except = null)
        {
            foreach (Control c in controls)
            {
                if (except == null || except != c)
                    c.Enabled = enable;
            }
        }

        public virtual void UpdateGeometric(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;
        }

        /// hooks

        public virtual void OnRemove() { }

        public virtual void OnAttach() { }
    }
}
