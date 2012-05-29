using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using System.Windows;

namespace Orbit.src.Core.Scene.Entities.Implementations
{
   public class Hook : SpherePoint
    {
        public Player Player { get; set; }

        private IContainsGold goldObject;

        protected override void UpdateGeometricState()
        {

        }

        public override void DoCollideWith(ICollidable other)
        {
            if (other is IContainsGold)
                goldObject = other as IContainsGold;
            else if (other is Base)
                addGoldToPlayer();
        }

        private void addGoldToPlayer()
        {
            Player.Data.Gold += goldObject.Gold;
        }

        public Boolean isFull()
        {
            return goldObject != null;
        }
    }

   public class HookControl : Control 
   {
       public int Speed { get; set; }
       public int Lenght { get; set; }
       public Vector Origin { get; set; }

       private Hook hook;
       private bool returning;

       public override void InitControl(ISceneObject me)
       {
           returning = false;

           if (me is Hook)
               hook = me as Hook;
       }

       public override void UpdateControl(float tpf)
       {
           if (hook.isFull() || returning)
               moveBackwards(tpf);
           else
               moveForward(tpf);
       }

       private void moveForward(float tpf)
       {
           hook.Position += (hook.Direction * Speed);
       }

       private void moveBackwards(float tpf)
       {
           hook.Position += (hook.Direction * Speed);
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
