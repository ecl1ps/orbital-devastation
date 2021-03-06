﻿using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using Lidgren.Network;
using Orbit.Core.Scene.Controls;
using System.Collections.Generic;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Players;
using Orbit.Core.Weapons;
using Orbit.Core.Scene.CollisionShapes;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class MinorAsteroid : Asteroid
    {
        public UnstableAsteroid Parent { get; set; }
        private Player lastHitTakenFrom;

        public MinorAsteroid(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override void TakeDamage(int damage, ISceneObject from)
        {
            base.TakeDamage(damage, from);
            if (from is IProjectile)
                lastHitTakenFrom = (from as IProjectile).Owner;
        }

        public override void OnRemove()
        {
            base.OnRemove();
            if (lastHitTakenFrom != null)
                Parent.NoticeChildAsteroidDestroyedBy(lastHitTakenFrom, this);
        }

        public new void WriteObject(NetOutgoingMessage msg)
        {
            msg.WriteObjectMinorAsteroid(this);
            msg.WriteControls(GetControlsCopy());
        }

        public new void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectMinorAsteroid(this);

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = Center;
            cs.Radius = Radius;
            CollisionShape = cs;

            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }

        public void ResetLastHit()
        {
            lastHitTakenFrom = null;
        }
    }
}
