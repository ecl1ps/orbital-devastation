using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using Lidgren.Network;
using Orbit.Core.Client;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class LimitedReverseDamageControl : Control, IDamageControl
    {
        public bool Vulnerable { get; set; }

        public LimitedReverseDamageControl(float time) : base()
        {
            events.AddEvent(1, new Event(time, EventType.ONE_TIME, new Action(() => { End(); })));
        }

        protected override void InitControl(Entities.ISceneObject me)
        {
            me.GetControlsOfType<IDamageControl>().ForEach(control => control.Vulnerable = false);

            Vulnerable = true;
        }

        public void ProccessDamage(int damage, Entities.ISceneObject causedBy)
        {
            if (causedBy is IDestroyable)
            {
                (causedBy as IDestroyable).TakeDamage(damage / 4, me);
                me.SceneMgr.FloatingTextMgr.AddFloatingText(damage / 4, me.Position, FloatingTextManager.TIME_LENGTH_3, FloatingTextType.DAMAGE);
            }

            if (me is MiningModule)
                SendDamage(damage / 4, causedBy, me as MiningModule);
        }

        private void End()
        {
            OnSceneObjectRemove();
            me.RemoveControl(this);
            me.GetControlsOfType<IDamageControl>().ForEach(control => control.Vulnerable = true);

            if (me is MiningModule)
                SendColorChange(me as MiningModule);
        }

        private void SendDamage(int damage, ISceneObject obj, MiningModule me)
        {
            NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.OBJECTS_TAKE_DAMAGE);
            msg.Write(me.Owner.GetId());
            msg.Write(1);
            msg.Write(damage);
            msg.Write(obj.Id);

            me.SceneMgr.SendMessage(msg);
        }

        private void SendColorChange(MiningModule module)
        {
            NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.MODULE_COLOR_CHANGE);
            msg.Write(module.Owner.GetId());
            msg.Write(module.Owner.GetPlayerColor());

            me.SceneMgr.SendMessage(msg);
        }
    }
}
