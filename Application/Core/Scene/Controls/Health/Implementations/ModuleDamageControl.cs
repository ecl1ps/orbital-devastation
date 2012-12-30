using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Lidgren.Network;
using Orbit.Core.Scene.Controls.Health;
using Orbit.Core.Scene.Controls.Health.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class ModuleDamageControl : HpControl, IDamageControl
    {
        public bool Vulnerable { get; set; }

        private MiningModule module;

        protected override void InitControl(Entities.ISceneObject me)
        {
            Vulnerable = true;
            hp = MaxHp;

            if (me is MiningModule)
                module = me as MiningModule;
            else
                throw new Exception("ModuleDamageControl must be attached to MiningModule object");
        }

        public void ProccessDamage(int damage, ISceneObject causedBy)
        {
            if (!Vulnerable)
                return;

            Hp -= damage;
            HpRegenControl control = module.GetControlOfType<HpRegenControl>();
            if (control != null)
                control.TakeHit();

            if (module.Owner.IsCurrentPlayer())
            {
                NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
                msg.Write((int)PacketType.MINING_MODULE_DMG_TAKEN);
                msg.Write(module.Owner.GetId());
                msg.Write(damage);
                msg.Write(Hp);

                me.SceneMgr.SendMessage(msg);
            }
        }

        protected override void OnDeath()
        {
            module.GetControlOfType<HpBarControl>().Bar.Percentage = 0;
            module.GetControlOfType<RespawningObjectControl>().Die(SharedDef.SPECTATOR_RESPAWN_TIME);
        }
    }
}
