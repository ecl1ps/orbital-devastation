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
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.Controls.Collisions.Implementations;
using Orbit.Core.Scene.CollisionShapes;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public enum HookType
    {
        HOOK_NORMAL,
        HOOK_POWER
    }

    public class Hook : Sphere, ISendable, IRotable, IProjectile
    {
        public Player Owner { get; set; } // neposilano
        public float Rotation { get; set; }
        public ICatchable CaughtObject { get; set; } // neposilano
        public HookType HookType { get; set; }
        public Vector RopeContactPoint
        {
            get
            {
                return Center - Direction * Radius;
            }
        }

        private System.Windows.Shapes.Line line; // neposilano
        
        public Hook(SceneMgr mgr) : base(mgr)
        {
            HookType = HookType.HOOK_NORMAL;
        }

        public void PrepareLine()
        {
            SceneMgr.Invoke(new Action(() =>
            {
                HookControl control = GetControlOfType<HookControl>();
                line = new System.Windows.Shapes.Line();
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

        public virtual void OnCatch()
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.HOOK_HIT);
            msg.Write(Id);
            msg.Write(CaughtObject.Id);
            msg.Write(CaughtObject.Position);
            msg.Write(GetControlOfType<HookControl>().HitVector);
            SceneMgr.SendMessage(msg);
        }

        public virtual void PulledCaughtObjectToBase()
        {
            ProccesCaughtObject(CaughtObject);
        }

        protected virtual void ProccesCaughtObject(ICatchable caught) 
        {
            if (caught != null && !caught.Dead)
            {
                if (caught is IContainsGold)
                {
                    AddGoldToOwner((caught as IContainsGold).Gold);
                    caught.DoRemoveMe();
                }
                else
                {
                    caught.Enabled = true;
                    if (caught is IMovable)
                        (caught as IMovable).Direction = new Vector(0, 100);
                }
            }

            DoRemoveMe();
            SceneMgr.RemoveGraphicalObjectFromScene(line);
        }

        protected virtual void AddGoldToOwner(int gold) 
        {
            Owner.AddGoldAndShow(gold);
        }

        public virtual bool HasCaughtObject()
        {
            return CaughtObject != null;
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

            PointCollisionShape cs = new PointCollisionShape();
            cs.Center = Center;
            CollisionShape = cs;

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