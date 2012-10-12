using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using Lidgren.Network;
using Orbit.Core.Client;

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
            {
                (causedBy as IDestroyable).TakeDamage(damage / 4, me);
                me.SceneMgr.FloatingTextMgr.AddFloatingText(damage / 4, me.Position, FloatingTextManager.TIME_LENGTH_3, FloatingTextType.DAMAGE);
            }

            if (me is MiningModule)
                sendDamage(damage / 4, causedBy, me as MiningModule);
        }

        private void end()
        {
            OnControlDestroy();
            me.RemoveControl(this);
            me.GetControlsOfType(typeof(IDamageControl)).ForEach(control => (control as IDamageControl).Vulnerable = true);

            if (me is MiningModule)
                sendColorChange(me as MiningModule);
        }

        private void sendDamage(int damage, ISceneObject obj, MiningModule me)
        {
            NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.OBJECTS_TAKE_DAMAGE);
            msg.Write(me.Owner.GetId());
            msg.Write(1);
            msg.Write(damage);
            msg.Write(obj.Id);

            me.SceneMgr.SendMessage(msg);
        }

        private void sendColorChange(MiningModule module)
        {
            NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.MODULE_COLOR_CHANGE);
            msg.Write(module.Owner.GetId());
            msg.Write(module.Owner.GetPlayerColor());

            me.SceneMgr.SendMessage(msg);
        }
    }
}
