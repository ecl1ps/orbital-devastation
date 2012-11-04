﻿using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using System.Windows.Shapes;
using Orbit.Core.Client;
using Orbit.Core.Scene.CollisionShapes;
using System.Collections;
using System.Linq;

namespace Orbit.Core.Scene.Entities
{
    public abstract class SceneObject : ISceneObject
    {
        private List<IControl> controls;
        public long Id { get; set; }
        protected UIElement geometryElement;
        public ICollisionShape CollisionShape { get; set; }
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

        public void RemoveControlsOfType<T>()
        {
            for (int i = 0; i < controls.Count; ++i)
            {
                if (typeof(T).IsAssignableFrom(controls[i].GetType()))
                    controls.Remove(controls[i]);
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
