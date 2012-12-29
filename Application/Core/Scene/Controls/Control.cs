using System;
using Orbit.Core.Scene.Entities;
using System.Collections.Generic;

namespace Orbit.Core.Scene.Controls
{
    public abstract class Control : IControl
    {
        protected ISceneObject me;

        public virtual bool Enabled { get; set; }

        private List<Action> actions = new List<Action>();

        public Control() 
        {
            Enabled = true; 
        }

        public void Init(ISceneObject me)
        {
            this.me = me;
            InitControl(me);
        }

        public void Update(float tpf)
        {
            if (Enabled)
                UpdateControl(tpf);
        }

        public virtual void OnControlDestroy()
        {
            actions.ForEach(a => a.Invoke());
        }

        public void AddControlDestroyAction(Action a)
        {
            actions.Add(a);
        }

        protected virtual void InitControl(ISceneObject me) { }

        protected virtual void UpdateControl(float tpf) { }


        public void Destroy()
        {
            OnControlDestroy();
            me.RemoveControl(this);
        }
    }
}
