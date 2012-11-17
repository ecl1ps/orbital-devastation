using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Health;

namespace Orbit.Core.Scene.Controls.Health.Implementations
{
    public class HpRegenControl : Control
    {
        public float MaxRegenTime { get; set; }

        public float RegenTimer { get; set; }
        public float RegenSpeed { get; set; }
        private IHpControl obj;
        private float health = 0;

        protected override void InitControl(Entities.ISceneObject me)
        {
            obj = me.GetControlOfType<IHpControl>();
            if (obj == null)
                throw new Exception("HpRegenControl must be attached to object containing HpControl");
        }

        protected override void UpdateControl(float tpf)
        {
            if (RegenTimer < MaxRegenTime)
                RegenTimer += tpf;
            else
            {
                if (obj.Hp < obj.MaxHp)
                {
                    health += (obj.MaxHp / RegenSpeed) * tpf;
                    if (health >= 1)
                    {
                        obj.Hp += (int)health;
                        health = 0;
                    }
                }
            }
        }

        public void TakeHit()
        {
            RegenTimer = 0;
        }
    }
}
