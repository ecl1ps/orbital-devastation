using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using System.Windows;
using Lidgren.Network;
using System.Windows.Media;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
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

    public class Hook : Sphere, ISendable, IProjectile
    {
        public Player Owner { get; set; } // neposilano
        public ICatchable CaughtObject { get; set; } // neposilano
        public HookType HookType { get; set; }
        public Vector RopeContactPoint
        {
            get
            {
                return Center - Direction * Radius;
            }
        }

        private DrawingGroup lineGeom;

        public Hook(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            HookType = HookType.HOOK_NORMAL;
        }

        public void PrepareLine()
        {
            HookControl control = GetControlOfType<HookControl>();
            lineGeom = SceneGeometryFactory.CreateLineGeometry(SceneMgr, Colors.LightSteelBlue, 2, Colors.Black, control.Origin, control.Origin);
            SceneMgr.AttachGraphicalObjectToScene(lineGeom);
        }

        protected override void UpdateGeometricState()
        {
            ((lineGeom.Children[0] as GeometryDrawing).Geometry as LineGeometry).EndPoint = RopeContactPoint.ToPoint();
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
                        (caught as IMovable).Direction = new Vector(0, 1);
                }
            }

            DoRemoveMe();
            SceneMgr.RemoveGraphicalObjectFromScene(lineGeom);
        }

        public override void OnRemove()
        {
            SceneMgr.RemoveGraphicalObjectFromScene(lineGeom);
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