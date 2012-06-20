using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using Orbit.Core.Players;
using Orbit.Core.Scene;
using Orbit.Core.Scene.Entities;
using Lidgren.Network;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;

namespace Orbit.Core.Weapons
{
    class ProximityCannon : IWeapon
    {
        public Player Owner { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public float ReloadTime { get; set;}
        public int Cost { get; set; }
        public WeaponType WeaponType { get; set; }
        public String Name { get; set; }

        public ProximityCannon(SceneMgr mgr, Player owner)
        {
            SceneMgr = mgr;
            Owner = owner;
        }

        public IWeapon Next()
        {
            throw null;
        }

        public void Shoot(Point point)
        {
            if (IsReady())
            {
                SpawnBullet(point);
                ReloadTime = Owner.Data.BulletCooldown;
            }
        }

        protected void SpawnBullet(Point point)
        {
            if (point.Y > Owner.VectorPosition.Y)
                point.Y = Owner.VectorPosition.Y;

            SingularityBullet bullet = SceneObjectFactory.CreateSingularityBullet(SceneMgr, point, Owner);

            if (SceneMgr.GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                (bullet as ISendable).WriteObject(msg);
                SceneMgr.SendMessage(msg);
            }

            SceneMgr.DelayedAttachToScene(bullet);
        }

        public bool IsReady()
        {
            return ReloadTime <= 0;
        }

        public void UpdateTimer(float value)
        {
            if (ReloadTime > 0)
                ReloadTime -= value;
        }
    }
}
