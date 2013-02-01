using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using Orbit.Core.Client;
using Orbit.Core.Scene.CollisionShapes;
using System.Collections;
using System.Linq;
using System.Windows.Media;
using MB.Tools;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Scene.Entities
{
    public abstract class SceneObject : ISceneObject, IRotable, IMovable
    {
        private List<IControl> controls;

        protected DrawingGroup geometryElement;

        public long Id { get; set; }
        public ICollisionShape CollisionShape { get; set; }
        public virtual Vector Position { get; set; }
        public Vector Direction { get; set; }
        public float Rotation { get; set; }
        public DrawingCategory Category { get; set; }
        public abstract Vector Center { get; }
        public bool Dead { get; set; }
        public SceneMgr SceneMgr { get; set; }
        protected bool visible;
        public virtual bool Visible
        {
            get 
            { 
                return visible; 
            } 
            set 
            {
                if (geometryElement == null)
                    return;

                if (value != visible)
                {
                    SceneMgr.BeginInvoke(new Action(() =>
                    {
                        MetaPropertyExtender.SetMetaProperty(geometryElement, "IsVisible", value);
                    }));
                }

                visible = value;
            } 
        }
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
            visible = true;
            Category = DrawingCategory.BACKGROUND;
        }

        public virtual void Update(float tpf)
        {
            if(Enabled)
                UpdateControls(tpf);
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

        public abstract bool IsOnScreen(Size screenSize);

        public abstract void UpdateGeometric();

        public DrawingGroup GetGeometry()
        {
            return geometryElement;
        }

        public virtual void SetGeometry(DrawingGroup geometryElement)
        {
            this.geometryElement = geometryElement;
            // zajisti, ze se spravne nastavi meta property
            visible = false;
            Visible = true;
        }

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

        /// hooks

        public virtual void OnRemove() { }

        public virtual void OnAttach() { }
    }
}
