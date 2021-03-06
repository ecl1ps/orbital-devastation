using System;
using Orbit.Core.Scene.Entities;
using System.Collections.Generic;

namespace Orbit.Core.Scene.Controls
{
    public abstract class Control : IControl
    {
        protected ISceneObject me;

        private bool enabled;
        public virtual bool Enabled { get { return enabled; } set { enabled = value; } }

        private List<Action> actions = new List<Action>();
        
        protected EventProcessor events = new EventProcessor();

        public Control() 
        {
            enabled = true; 
        }

        public void Init(ISceneObject me)
        {
            this.me = me;
            InitControl(me);
        }

        public void Update(float tpf)
        {
            if (Enabled)
            {
                UpdateControl(tpf);
                events.Update(tpf);
            }
        }

        public virtual void OnSceneObjectRemove()
        {
            actions.ForEach(a => a.Invoke());
        }

        public void AddSceneObjectRemoveAction(Action a)
        {
            actions.Add(a);
        }

        public void Destroy()
        {
            OnRemove();
            me.RemoveControl(this);
        }


        public virtual void OnRemove() { }

        public virtual void AfterUpdate(float tpf) { }

        protected virtual void InitControl(ISceneObject me) { }

        protected virtual void UpdateControl(float tpf) { }
    }
}
