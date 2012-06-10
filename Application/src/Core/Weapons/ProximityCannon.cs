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

namespace Orbit.Core.Weapons
{
    class ProximityCannon : IWeapon
    {
        public float ReloadTime { get; set;}
        public int Cost { get; set; }
        public WeaponType WeaponType { get; set; }
        public String Name { get; set; }

        public IWeapon Next()
        {
            throw null;
        }

        public void Shoot(Point point)
        {
            if (IsReady())
            {
                SpawnBullet(point);
                ReloadTime = SharedDef.BULLET_COOLDOWN;
            }
        }

        protected void SpawnBullet(Point point)
        {
            Player player = SceneMgr.GetInstance().GetMePlayer();

            if (point.Y > PlayerPositionProvider.GetVectorPosition(player.GetPosition()).Y)
                return;

                SingularityBullet bullet = SceneObjectFactory.CreateSingularityBullet(new System.Windows.Point(point.X, point.Y), player);

                if (SceneMgr.GetInstance().GameType != Gametype.SOLO_GAME)
                {
                    NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                    (bullet as ISendable).WriteObject(msg);
                    SceneMgr.SendMessage(msg);
                }

                SceneMgr.GetInstance().AttachToScene(bullet);
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
