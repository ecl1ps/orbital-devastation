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
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using System.Diagnostics;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class Hook : SpherePoint, ISendable, IRotable
    {
        public Player Owner { get; set; } // neposilano
        public float Rotation { get; set; }
        public IContainsGold GoldObject { get; set; } // neposilano
        public Vector RopeContactPoint
        {
            get
            {
                return Center - Direction * Radius;
            }
        }

        private Line line; // neposilano
        
        public Hook(SceneMgr mgr) : base(mgr)
        {
        }

        public void PrepareLine()
        {
            SceneMgr.Invoke(new Action(() =>
            {
                HookControl control = GetControlOfType(typeof(HookControl)) as HookControl;
                line = new Line();
                line.Stroke = Brushes.LightSteelBlue;
                line.X1 = control.Origin.X;
                line.Y1 = control.Origin.Y;
                line.X2 = control.Origin.X;
                line.Y2 = control.Origin.Y;
                line.HorizontalAlignment = HorizontalAlignment.Left;
                line.VerticalAlignment = VerticalAlignment.Center;
                line.StrokeThickness = 2;
                line.Fill = new SolidColorBrush(Colors.Black);
            }));

            SceneMgr.AttachGraphicalObjectToScene(line);
        }

        protected override void UpdateGeometricState()
        {
            line.X2 = RopeContactPoint.X;
            line.Y2 = RopeContactPoint.Y;
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
            if (SceneMgr.GameType != Gametype.SOLO_GAME && Owner.GetPosition() == SceneMgr.GetOpponentPlayer().GetPosition())
                return;

            Vector hitVector = gold.Position - Position;

            if (gold.Enabled)
            {
                Catch(gold, hitVector);
                if (SceneMgr.GameType != Gametype.SOLO_GAME)
                {
                    NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                    msg.Write((int)PacketType.HOOK_HIT);
                    msg.Write(Id);
                    msg.Write(gold.Id);
                    msg.Write(gold.Position);
                    msg.Write(hitVector);
                    SceneMgr.SendMessage(msg);
                }
            }
        }

        public void Catch(IContainsGold gold, Vector hitVector)
        {
            GoldObject = gold;
            gold.Enabled = false;

            (GetControlOfType(typeof(HookControl)) as HookControl).CaughtObject(hitVector);
        }

        public void AddGoldToPlayer()
        {
            if (GoldObject != null && !GoldObject.Dead)
            {
                Owner.AddGoldAndShow(GoldObject.Gold);
                GoldObject.DoRemoveMe();
            }
            DoRemoveMe();
            SceneMgr.RemoveGraphicalObjectFromScene(line);
        }

        public Boolean HasCaughtObject()
        {
            return GoldObject != null;
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_HOOK);
            msg.WriteObjectHook(this);
            msg.WriteControls(GetControlsCopy());
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectHook(this);
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