using System;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    public abstract class Control : IControl
    {
        protected ISceneObject me;

        public bool Enabled { get; set; }

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
        }

        public abstract void InitControl(ISceneObject me);

        public abstract void UpdateControl(float tpf);
    }
}
