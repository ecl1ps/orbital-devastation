using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace Orbit.Core.Scene.Entities
{
    public abstract class SceneObject : ISceneObject
    {
        protected IList<IControl> controls;
        protected long id;
        protected Vector position;
        protected UIElement geometryElement;
        protected bool isDead;

        public SceneObject()
        {
            controls = new List<IControl>();
            isDead = false;
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

        public void SetGeometry(UIElement geometryElement)
        {
            this.geometryElement = geometryElement;
        }

        public UIElement GetGeometry()
        {
            return geometryElement;
        }

        public void DoRemoveMe()
        {
            DoRemove(this);
        }

        public void DoRemove(ISceneObject obj)
        {
            SceneMgr.GetInstance().RemoveFromSceneDelayed(obj);
        }

        public void SetDead(bool dead)
        {
            this.isDead = dead;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public virtual void OnRemove()
        {

        }
    }

}
