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
        private long id;
        protected Vector position;
        protected Path path;

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

        public abstract bool IsOnScreen(Size screenSize);

        public abstract void UpdateGeometric();

        public long GetId()
        {
            return id;
        }

        public void Setid(long id)
        {
            this.id = id;
        }

        public void SetGeometry(Path path)
        {
            this.path = path;
        }

        public Path GetGeometry()
        {
            return path;
        }
    }

}
