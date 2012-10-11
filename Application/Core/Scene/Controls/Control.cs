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

        public void Init(ISceneObject me)
        {
            Enabled = true;
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

        public void addControlDestroyAction(Action a)
        {
            actions.Add(a);
        }

        public abstract void InitControl(ISceneObject me);

        public abstract void UpdateControl(float tpf);
    }
}
