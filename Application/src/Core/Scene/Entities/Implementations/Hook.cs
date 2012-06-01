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
    public class Hook : Sphere, ISendable
    {
        public Player Player { get; set; }
        public IContainsGold GoldObject { get; set; }

        private Line line;

        public void prepareLine()
        {
            SceneMgr.GetInstance().GetUIDispatcher().BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                HookControl control = GetControlOfType(typeof(HookControl)) as HookControl;
                line = new Line();
                line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                line.X1 = control.Origin.X;
                line.Y1 = control.Origin.Y;
                line.X2 = control.Origin.X;
                line.Y2 = control.Origin.Y;
                line.HorizontalAlignment = HorizontalAlignment.Left;
                line.VerticalAlignment = VerticalAlignment.Center;
                line.StrokeThickness = 2;
                line.Fill = new SolidColorBrush(Colors.Black);
            
                SceneMgr.GetInstance().GetCanvas().Children.Add(line);
            }));
        }

        protected override void UpdateGeometricState()
        {
            line.X2 = Position.X;
            line.Y2 = Position.Y;
        }

        public override void DoCollideWith(ICollidable other)
        {
            if (HasCaughtObject())
                return;

            if ((other is IContainsGold && !(GetControlOfType(typeof(HookControl)) as HookControl).Returning))
                CatchObjectWithGold(other as IContainsGold);
        }

        private void CatchObjectWithGold(IContainsGold gold)
        {
            GoldObject = gold;

            foreach (IControl control in gold.GetControlsCopy())
                control.Enabled = false;

            (GetControlOfType(typeof(HookControl)) as HookControl).CaughtObject();
        }

        public void AddGoldToPlayer()
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
            SceneMgr.GetInstance().ShowStatusText(3, "Gold: " + Player.Data.Gold);
        }

        public Boolean HasCaughtObject()
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
        public Vector HitVector { get; set; } //neposilano
        public bool Returning { get; set; } // neposilano

        private Hook hook;

        public override void InitControl(ISceneObject me)
        {
            Returning = false;

            if (me is Hook)
                hook = me as Hook;
        }

        public override void UpdateControl(float tpf)
        {
            if (hook.HasCaughtObject() || Returning)
            {
                MoveBackwards(tpf);
                if (hook.HasCaughtObject())
                    MoveWithObject(hook.GoldObject);

                if (IsAtStart())
                    hook.AddGoldToPlayer();
            }
            else
            {
                MoveForward(tpf);
                if (IsAtEnd())
                    Returning = true;
            }
        }

        private void MoveWithObject(IContainsGold obj)
        {
            obj.Position = hook.Position + HitVector;
        }

        private void MoveForward(float tpf)
        {
            hook.Position += (hook.Direction * Speed * tpf);
        }

        private void MoveBackwards(float tpf)
        {
            hook.Position -= (hook.Direction * Speed * tpf);
        }

        private bool IsAtEnd()
        {
            return GetDistanceFromOrigin() > Lenght;
        }

        private bool IsAtStart()
        {
            return GetDistanceFromOrigin() < 50;
        }

        private double GetDistanceFromOrigin()
        {
            return (Origin - hook.Position).Length;
        }

        public void CaughtObject()
        {
            HitVector = hook.GoldObject.Position - hook.Position;
        }
    }
}
