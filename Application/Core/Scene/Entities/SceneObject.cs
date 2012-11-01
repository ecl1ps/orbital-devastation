﻿using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using System.Windows.Shapes;
using Orbit.Core.Client;

namespace Orbit.Core.Scene.Entities
{
    public abstract class SceneObject : ISceneObject
    {
        private List<IControl> controls;
        public long Id { get; set; }
        protected UIElement geometryElement;
        public Vector Position { get; set; }
        public virtual Vector Center
        {
            get
            {
                return Position;
            }
        }
        public bool Dead { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public bool Visible { get { return geometryElement.Visibility != Visibility.Hidden; } 
            set 
            {
                if (geometryElement == null)
                    return;

                //TODO: ve vlaknu gui?
                if (value)
                    geometryElement.Visibility = Visibility.Visible;
                else
                    geometryElement.Visibility = Visibility.Hidden;
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
                foreach (IControl control in GetControlsCopy())
                    control.Enabled = enabled;
            }
        }

        private SceneObject()
        {
        }

        public SceneObject(SceneMgr mgr)
        {
            controls = new List<IControl>();
            Dead = false;
            SceneMgr = mgr;
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

        public void RemoveControl(Type type)
        {
            for (int i = 0; i < controls.Count; ++i)
            {
                if (type.IsAssignableFrom(controls[i].GetType()))
                    controls.Remove(controls[i]);
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

        public List<IControl> GetControlsOfType(Type type)
        {
            List<IControl> temp = new List<IControl>();

            foreach (IControl control in controls)
            {
                if (type.IsAssignableFrom(control.GetType()))
                    temp.Add(control);
            }

            return temp;
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

            controls.ForEach(control => control.OnControlDestroy());
        }

        public void DoRemoveMe()
        {
            DoRemove(this);
        }


        public void RemoveControl(Control control)
        {
            controls.Remove(control);
        }


        /// hooks

        public virtual void OnRemove() { }

        public virtual void OnAttach() { }
    }

}
