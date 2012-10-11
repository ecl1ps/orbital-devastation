using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Lidgren.Network;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class ModuleDamageControl : Control, IDamageControl
    {
        public bool Vulnerable { get; set; }

        private MiningModule module;

        public override void InitControl(Entities.ISceneObject me)
        {
            Vulnerable = true;

            if (me is MiningModule)
                module = me as MiningModule;
            else
                throw new Exception("ModuleDamageControl must be attached to MiningModule object");
        }

        public override void UpdateControl(float tpf)
        {
        }

        public void proccessDamage(int damage, ISceneObject causedBy)
        {
            if (!Vulnerable)
                return;

            module.Hp -= damage;
            HpRegenControl control = module.GetControlOfType(typeof(HpRegenControl)) as HpRegenControl;
            if (control != null)
                control.TakeHit();

            if (module.Hp <= 0)
                (module.GetControlOfType(typeof(RespawningObjectControl)) as RespawningObjectControl).die(SharedDef.SPECTATOR_RESPAWN_TIME);

            if (module.Owner.IsCurrentPlayer())
            {
                NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
                msg.Write((int)PacketType.MINING_MODULE_DMG_TAKEN);
                msg.Write(module.Owner.GetId());
                msg.Write(damage);
                msg.Write(module.Hp);

                me.SceneMgr.SendMessage(msg);
            }
        }
    }
}
