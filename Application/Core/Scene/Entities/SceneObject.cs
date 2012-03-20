using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;

namespace Orbit.Core.Scene.Entities
{
    public abstract class SceneObject : ISceneObject
    {
        private IList<IControl> controls;
        private long id;
        private Vector position;

        public SceneObject()
        {
            controls = new List<IControl>();
        }

        public void Update(float tpf)
        {
            UpdateControls(tpf);
        }

        private void UpdateControls(float tpf)
        {
            foreach (IControl control in controls)
            {
                control.UpdateControl(tpf);
            }
        }

        public void AddControl(IControl control)
        {
            if (controls.Contains(control))
                return;

            controls.Add(control);
            control.InitControl(this);
        }

        public void RemoveControl(Type type)
        {
            foreach (IControl control in controls)
            {
                if (control.GetType().Equals(type))
                    controls.Remove(control);
            }
        }

        public IControl GetControlOfType(Type type)
        {
            foreach (IControl control in controls)
            {
                if (control.GetType().Equals(type))
                    return control;
            }
            return null;
        }

        public Vector GetPosition()
        {
            return position;
        }

        public void SetPosition(Vector pos)
        {
            position = pos;
        }

        public abstract bool IsOnScreen();

        public abstract void Render();


        public long GetId()
        {
            return id;
        }

        public void Setid(long id)
        {
            this.id = id;
        }
    }

}
