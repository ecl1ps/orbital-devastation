﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using Lidgren.Network;
using Orbit.Core;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using Orbit.Core.Scene.Controls.Implementations;
using System.Windows;
using Orbit.Core.Scene.CollisionShapes;
using Orbit.Core.Scene.Controls.Collisions.Implementations;


namespace Orbit.Core.Scene.Entities.Implementations
{
    public class SingularityExplodingBullet : SingularityBullet
    {
        public SingularityExplodingBullet(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public void SpawnSmallBullets()
        {
            CreateSmallBullet(MathHelper.Pi / 12);
            CreateSmallBullet(0);
            CreateSmallBullet(-MathHelper.Pi / 12);
        }

        private void CreateSmallBullet(float rotation)
        {
            SingularityBullet smallBullet = new SingularityBullet(SceneMgr, IdMgr.GetNewId(SceneMgr.GetCurrentPlayer().GetId()));
            smallBullet.Owner = Owner;
            Vector2 dir = Direction;
            dir.Normalize();
            smallBullet.Direction = dir.Rotate(rotation);
            smallBullet.Position = Center;
            smallBullet.Radius = 2;
            smallBullet.Damage = Damage / 2;

            PointCollisionShape cs = new PointCollisionShape();
            cs.Center = smallBullet.Center;
            smallBullet.CollisionShape = cs;

            smallBullet.Color = Color;

            LinearMovementControl nmc = new LinearMovementControl();
            nmc.Speed = GetControlOfType<LinearMovementControl>().Speed;
            smallBullet.AddControl(nmc);

            SingularityBulletCollisionReactionControl cc = new SingularityBulletCollisionReactionControl();
            cc.StatReported = true;
            smallBullet.AddControl(cc);
            smallBullet.AddControl(new StickyPointCollisionShapeControl());

            SceneMgr.DelayedAttachToScene(smallBullet);
        }

        public override void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_SINGULARITY_EXPLODING_BULLET);
            msg.WriteObjectSingularityBullet(this);
            msg.WriteControls(GetControlsCopy());
        }

        public override void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectSingularityBullet(this);

            PointCollisionShape cs = new PointCollisionShape();
            cs.Center = Center;
            CollisionShape = cs;

            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}
