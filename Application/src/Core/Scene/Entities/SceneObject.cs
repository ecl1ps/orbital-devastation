using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace Orbit.Core.Scene.Entities
{
    public abstract class SceneObject : ISceneObject
    {
        private IList<IControl> controls;
        public long Id { get; set; }
        public UIElement GeometryElement { get; set; }
        public Vector Position { get; set; }
        public bool Dead { get; set; }

        public SceneObject()
        {
            controls = new List<IControl>();
            Dead = false;
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

        public abstract bool IsOnScreen(Size screenSize);

        public abstract void UpdateGeometric();

        public void DoRemoveMe()
        {
            DoRemove(this);
        }

        public void DoRemove(ISceneObject obj)
        {
            SceneMgr.GetInstance().RemoveFromSceneDelayed(obj);
        }

        public virtual void OnRemove() { }
    }

}
