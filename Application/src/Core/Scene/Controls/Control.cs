using System;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    public abstract class Control : IControl
    {
        protected ISceneObject me;

        public void Init(ISceneObject me)
        {
            this.me = me;
            InitControl(me);
        }

        public void Update(float tpf)
        {
            UpdateControl(tpf);
        }

        public abstract void InitControl(ISceneObject me);

        public abstract void UpdateControl(float tpf);

    }
}
