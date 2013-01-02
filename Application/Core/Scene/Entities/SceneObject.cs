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
        public Vector Position { get; set; }
        public Vector Direction { get; set; }
        public float Rotation { get; set; }
        public DrawingCategory Category { get; set; }
        public virtual Vector Center
        {
            get
            {
                return Position;
            }
        }
        public bool Dead { get; set; }
        public SceneMgr SceneMgr { get; set; }
        private bool visible;
        public bool Visible
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
        private bool enabled = true;
        public bool Enabled {
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
            Category = DrawingCategory.BACKGROUND;
        }

        public void Update(float tpf)
        {
            UpdateControls(tpf);
        }

        private void UpdateControls(float tpf)
        {
            for (int i = 0; i < controls.Count; ++i)
            {
                (controls[i] as Control).Update(tpf);
            }
        }

        public void AddControl(Control control)
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

        public abstract bool IsOnScreen(Size screenSize);

        public abstract void UpdateGeometric();

        public DrawingGroup GetGeometry()
        {
            return geometryElement;
        }

        public void SetGeometry(DrawingGroup geometryElement)
        {
            // zajisti, ze se spravne nastavi meta property
            visible = false;
            Visible = true;
            this.geometryElement = geometryElement;
        }

        public void DoRemove(ISceneObject obj)
        {
            SceneMgr.RemoveFromSceneDelayed(obj);

            controls.ForEach(control => control.OnRemove());
            controls.ForEach(control => control.OnSceneObjectRemove());
        }

        public void DoRemoveMe()
        {
            DoRemove(this);
        }

        public void RemoveControl(Control control)
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
