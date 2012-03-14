using System;
using System.Windows;
using Orbit.Scene.Controls;
using System.Collections.Generic;

namespace Orbit.Scene.Entities
{
    public abstract class SceneObject : ISceneObject
    {
        private IList<IControl> controls;
        private long id;
        private Vector position;

        public void Update(float tpf)
        {
            throw new Exception("Not implemented");
        }

        private void UpdateControls(float tpf)
        {
            throw new Exception("Not implemented");
        }

        public void AddControl(IControl control)
        {
            throw new Exception("Not implemented");
        }

        public void RemoveControl(Type type)
        {
            throw new Exception("Not implemented");
        }

        public IControl GetControlOfType(Type type)
        {
            throw new NotImplementedException();
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
    }

}
