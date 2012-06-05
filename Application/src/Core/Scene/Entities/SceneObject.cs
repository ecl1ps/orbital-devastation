using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace Orbit.Core.Scene.Entities
{
    public abstract class SceneObject : ISceneObject
    {
        private List<IControl> controls;
        public long Id { get; set; }
        protected UIElement geometryElement;
        public Vector Position { get; set; }
        public bool Dead { get; set; }
        public SceneMgr SceneMgr { get; set; }

        private bool enabled;
        public bool Enabled {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                foreach (IControl control in GetControlsCopy())
                    control.Enabled = enabled;
            }
        }

        public SceneObject()
        {
            controls = new List<IControl>();
            Dead = false;
            SceneMgr = SceneMgr.GetInstance();
        }

        public void Update(float tpf)
        {
            UpdateControls(tpf);
        }

        private void UpdateControls(float tpf)
        {
            foreach (Control control in controls)
            {
                control.Update(tpf);
            }
        }

        public void AddControl(Control control)
        {
            if (controls.Contains(control))
                return;

            controls.Add(control);
            control.Init(this);
        }

        public void RemoveControl(Type type)
        {
            foreach (IControl control in controls)
            {
                if (type.IsAssignableFrom(control.GetType()))
                    controls.Remove(control);
            }
        }

        public IControl GetControlOfType(Type type)
        {
            foreach (IControl control in controls)
            {
                if (type.IsAssignableFrom(control.GetType()))
                    return control;
            }
            return null;
        }

        public IList<IControl> GetControlsCopy()
        {
            IControl[] copyOfControls = new IControl[controls.Count];
            controls.CopyTo(copyOfControls);
            return copyOfControls;
        }

        public abstract bool IsOnScreen(Size screenSize);

        public abstract void UpdateGeometric();

        public UIElement GetGeometry()
        {
            return geometryElement;
        }

        public void SetGeometry(UIElement geometryElement)
        {
            this.geometryElement = geometryElement;
        }

        public void DoRemove(ISceneObject obj)
        {
            SceneMgr.RemoveFromSceneDelayed(obj);
        }

        public virtual void OnRemove() { }


        public void DoRemoveMe()
        {
            DoRemove(this);
        }
    }

}
