using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using System.Windows;
using Lidgren.Network;

namespace Orbit.Core.Scene.Entities
{
    public class Hook : SpherePoint, ISendable
    {
        public Player Player { get; set; }
        public IContainsGold GoldObject { get; set; }

        protected override void UpdateGeometricState()
        {

        }

        public override void DoCollideWith(ICollidable other)
        {
            if ((other is IContainsGold && !(GetControlOfType(typeof(HookControl)) as HookControl).Returning))
                CatchGold(other as IContainsGold);
        }

        private void CatchGold(IContainsGold gold)
        {
            if (gold is IContainsGold)
                GoldObject = gold as IContainsGold;
            foreach (IControl control in gold.GetControlsCopy())
            {
                control.Enabled = false;
            }

            (GetControlOfType(typeof(HookControl)) as HookControl).Returning = true;
        }

        public void AddGoldToPlayer()
        {
            if (GoldObject != null)
            {
                Player.Data.Gold += GoldObject.Gold;
                GoldObject.DoRemoveMe();
            }
            DoRemoveMe();
        }

        public Boolean IsFull()
        {
            return GoldObject != null;
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_HOOK);
            msg.WriteObjectSphere(this);
            msg.WriteControls(GetControlsCopy());
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectSphere(this);
            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }
    }
}

namespace Orbit.Core.Scene.Controls
{
    public class HookControl : Control
    {
        public int Speed { get; set; }
        public int Lenght { get; set; }
        public Vector Origin { get; set; }
        public bool Returning { get; set; }

        private Hook hook;

        public override void InitControl(ISceneObject me)
        {
            Returning = false;

            if (me is Hook)
                hook = me as Hook;
        }

        public override void UpdateControl(float tpf)
        {
            if (hook.IsFull() || Returning)
            {
                MoveBackwards(tpf);
                if (hook.IsFull())
                    MoveWithObject(hook.GoldObject);

                if (IsStart())
                    hook.AddGoldToPlayer();
            }
            else
            {
                MoveForward(tpf);
                if (IsEnd())
                    Returning = true;
            }

        }

        private void MoveWithObject(IContainsGold obj)
        {
            obj.Position = hook.Position;
        }

        private void MoveForward(float tpf)
        {
            hook.Position += (hook.Direction * Speed * tpf);
        }

        private void MoveBackwards(float tpf)
        {
            hook.Position -= (hook.Direction * Speed * tpf);
        }

        private bool IsEnd()
        {
            return GetDistanceFromOrigin() > Lenght;
        }

        private bool IsStart()
        {
            return GetDistanceFromOrigin() < 50;
        }

        private double GetDistanceFromOrigin()
        {
            return (Origin - hook.Position).Length;
        }
    }
}
