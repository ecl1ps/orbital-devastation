﻿using System;
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
using Orbit.Gui.Visuals;

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
            Category = DrawingCategory.PROJECTILES;
        }

        public void PrepareLine()
        {
            HookControl control = GetControlOfType<HookControl>();
            lineGeom = SceneGeometryFactory.CreateLineGeometry(SceneMgr, Colors.LightSteelBlue, 2, Colors.Black, control.Origin, control.Origin);
            SceneMgr.AttachGraphicalObjectToScene(lineGeom, DrawingCategory.PROJECTILE_BACKGROUND);
        }

        protected override void UpdateGeometricState()
        {
            ((lineGeom.Children[0] as GeometryDrawing).Geometry as LineGeometry).EndPoint = RopeContactPoint.ToPoint();
        }

        public override void OnRemove()
        {
            SceneMgr.RemoveGraphicalObjectFromScene(lineGeom, DrawingCategory.PROJECTILE_BACKGROUND);
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

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Radius = Radius / 2;
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