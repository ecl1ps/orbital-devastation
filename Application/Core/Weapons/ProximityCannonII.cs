﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene;
using System.Windows;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Lidgren.Network;

namespace Orbit.Core.Weapons
{
    class ProximityCannonII : ProximityCannon
    {
        public ProximityCannonII(SceneMgr mgr, Player owner) : base(mgr, owner)
        {
            Cost = 300;
            Name = "ProximityCannonII";
            WeaponType = WeaponType.CANNON;
        }

        public override IWeapon Next()
        {
            return null;
        }

        protected override void SpawnBullet(Point point)
        {
            if (point.Y > Owner.VectorPosition.Y)
                point.Y = Owner.VectorPosition.Y;

            SingularityExplodingBullet bullet = SceneObjectFactory.CreateSingularityExploadingBullet(SceneMgr, point, Owner);

            if (SceneMgr.GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                (bullet as ISendable).WriteObject(msg);
                SceneMgr.SendMessage(msg);
            }

            SceneMgr.DelayedAttachToScene(bullet);
        }
    }
}
