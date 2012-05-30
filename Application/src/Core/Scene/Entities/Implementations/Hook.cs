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
        public IContainsGold GoldObject { get; set;}

        protected override void UpdateGeometricState()
        {

        }

        public override void DoCollideWith(ICollidable other)
        {
            if ((other is Asteroid && !(GetControlOfType(typeof(HookControl)) as HookControl).Returning))
                catchGold(other as Asteroid);
            else if ((other is Base && (GetControlOfType(typeof(HookControl)) as HookControl).Returning))
                addGoldToPlayer();
        }

        private void catchGold(Asteroid gold)
        {
            if(gold is IContainsGold)
                GoldObject = gold as IContainsGold;
            foreach (IControl control in gold.GetControlsCopy())
            {
                control.Enabled = false;
            }

            (GetControlOfType(typeof(HookControl)) as HookControl).Returning = true;
        }

        private void addGoldToPlayer()
        {
            if(GoldObject != null)
                Player.Data.Gold += GoldObject.Gold;

            DoRemoveMe();
        }

        public Boolean isFull()
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
    }

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
           if (hook.isFull() || Returning)
           {
               moveBackwards(tpf);
               if(hook.isFull())
                    moveWithObject(hook.GoldObject);
           }
           else
           {
               moveForward(tpf);
               if (isEnd())
                   Returning = true;
           }

       }

       private void moveWithObject(IContainsGold obj)
       {
           obj.Position = hook.Position;
       }

       private void moveForward(float tpf)
       {
           hook.Position += (hook.Direction * Speed * tpf);
       }

       private void moveBackwards(float tpf)
       {
           hook.Position -= (hook.Direction * Speed * tpf);
       }

       private bool isEnd()
       {
           return getDistanceFromOrigin() > Lenght;
       }

       private double getDistanceFromOrigin()
       {
           return (Origin - hook.Position).Length;
       }
   }
}
