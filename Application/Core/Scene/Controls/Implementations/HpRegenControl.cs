using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class HpRegenControl : Control
    {
        private float regenTimer;
        private MiningModule module;

        public override void InitControl(Entities.ISceneObject me)
        {
            regenTimer = SharedDef.SPECTATOR_HP_REGEN_CD;
            module = me as MiningModule;
        }

        public override void UpdateControl(float tpf)
        {
            if (regenTimer < SharedDef.SPECTATOR_HP_REGEN_CD)
                regenTimer += tpf;
            else
            {
                if (module.Hp < SharedDef.SPECTATOR_MAX_HP)
                    module.Hp += (SharedDef.SPECTATOR_MAX_HP / SharedDef.SPECTATOR_REGEN_SPEED) * tpf;
            }
        }

        public void TakeHit()
        {
            regenTimer = 0;
        }
    }
}
