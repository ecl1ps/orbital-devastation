using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using System.Windows.Media;
using System.Windows.Controls;
using Lidgren.Network;
using Orbit.Core.Players;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class MiningModule : Sphere, IRotable, IDestroyable, IHasHp
    {
        public float Rotation { get; set; }
        public float Hp { get; set; }

        private Player owner;

        public MiningModule(SceneMgr mgr, Player owner)
            : base(mgr)
        {
            this.owner = owner;

            Hp = SharedDef.SPECTATOR_MAX_HP;
            HasPositionInCenter = false;
        }

        protected override void UpdateGeometricState()
        {
            base.UpdateGeometricState();
            geometryElement.RenderTransform = new RotateTransform(Rotation);
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            //i dont want to be destroyed when moving off screen
            return true;
        }

        public override void DoCollideWith(ICollidable other, float tpf)
        {
            MiningModuleControl control = (GetControlOfType(typeof(MiningModuleControl)) as MiningModuleControl);
            
            if (control != null)
                control.Collide(other);
        }

        public void TakeDamage(int damage, ISceneObject from)
        {
            Hp -= damage;
            (GetControlOfType(typeof(HpRegenControl)) as HpRegenControl).TakeHit();

            if (Hp <= 0)
                (GetControlOfType(typeof(RespawningObjectControl)) as RespawningObjectControl).die(SharedDef.SPECTATOR_RESPAWN_TIME);

            if(owner.IsCurrentPlayer()) {
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                msg.Write((int) PacketType.MINING_MODULE_DMG_TAKEN);
                msg.Write(owner.GetId());
                msg.Write(damage);
                msg.Write(Hp);

                SceneMgr.SendMessage(msg);
            }
        }

        public void refillHp()
        {
            Hp = SharedDef.SPECTATOR_MAX_HP;
        }
    }
}
