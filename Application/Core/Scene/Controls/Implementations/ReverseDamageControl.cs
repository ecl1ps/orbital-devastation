using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class LimitedReverseDamageControl : Control, IDamageControl
    {
        public bool Vulnerable { get; set; }

        private float remainingTime;

        public LimitedReverseDamageControl(float time)
            : base()
        {
            remainingTime = time;
        }

        public override void InitControl(Entities.ISceneObject me)
        {
            me.GetControlsOfType(typeof(IDamageControl)).ForEach(control => (control as IDamageControl).Vulnerable = false);

            Vulnerable = true;
        }

        public override void UpdateControl(float tpf)
        {
            remainingTime -= tpf;

            if (remainingTime <= 0)
                end();
        }

        public void proccessDamage(int damage, Entities.ISceneObject causedBy)
        {
            if (causedBy is IDestroyable)
                (causedBy as IDestroyable).TakeDamage(damage / 4, me);
        }

        private void end()
        {
            OnControlDestroy();
            me.RemoveControl(this);
            me.GetControlsOfType(typeof(IDamageControl)).ForEach(control => (control as IDamageControl).Vulnerable = true);
        }
    }
}
