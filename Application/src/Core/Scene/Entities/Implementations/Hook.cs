using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using System.Windows;
using Lidgren.Network;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media;

namespace Orbit.Core.Scene.Entities
{
   public class Hook : SpherePoint, ISendable
    {
        public Player Player { get; set; }
        public IContainsGold GoldObject { get; set;}

        private Line line;

        public void prepareLine(Point origin)
        {
            SceneMgr.GetInstance().GetUIDispatcher().BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                line = new Line();
                line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                line.X1 = origin.X;
                line.Y1 = origin.Y;
                line.X2 = origin.X;
                line.Y2 = origin.Y;
                line.HorizontalAlignment = HorizontalAlignment.Left;
                line.VerticalAlignment = VerticalAlignment.Center;
                line.StrokeThickness = 2;
                line.Fill = new SolidColorBrush(Colors.Black);
            
                SceneMgr.GetInstance().GetCanvas().Children.Add(line);
            }));
        }

        protected override void UpdateGeometricState()
        {
            line.X2 = Position.X - (Radius / 2) - 1;
            line.Y2 = Position.Y - (Radius / 2) - 1;
        }

        public override void DoCollideWith(ICollidable other)
        {
            if ((other is IContainsGold && !(GetControlOfType(typeof(HookControl)) as HookControl).Returning))
                catchGold(other as IContainsGold);
        }

        private void catchGold(IContainsGold gold)
        {
            if(gold is IContainsGold)
                GoldObject = gold as IContainsGold;
            foreach (IControl control in gold.GetControlsCopy())
            {
                control.Enabled = false;
            }

            (GetControlOfType(typeof(HookControl)) as HookControl).Returning = true;
        }

        public void addGoldToPlayer()
        {
            if (GoldObject != null)
            {
                Player.Data.Gold += GoldObject.Gold;
                GoldObject.DoRemoveMe();
            }
            DoRemoveMe();
            SceneMgr.GetInstance().GetUIDispatcher().BeginInvoke(DispatcherPriority.DataBind, (Action)(delegate
            {
                SceneMgr.GetInstance().GetCanvas().Children.Remove(line);
            }));
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

               if (isStart())
                   hook.addGoldToPlayer();
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

       private bool isStart()
       {
           return getDistanceFromOrigin() < 50;
       }

       private double getDistanceFromOrigin()
       {
           return (Origin - hook.Position).Length;
       }
   }
}
