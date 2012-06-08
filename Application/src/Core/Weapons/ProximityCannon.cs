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
        public ISceneMgr SceneMgr { get; set; }
        public float ReloadTime { get; set;}
        public int Cost { get; set; }

        public ProximityCannon(ISceneMgr mgr)
        {
            SceneMgr = mgr;
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
                ReloadTime = SharedDef.BULLET_COOLDOWN;
            }
        }

        protected void SpawnBullet(Point point)
        {
            Player player = SceneMgr.GetMePlayer();

            if (point.Y > PlayerPositionProvider.GetVectorPosition(player.GetPosition()).Y)
                return;

                SingularityBullet bullet = SceneObjectFactory.CreateSingularityBullet(SceneMgr, new Point(point.X, point.Y), player);

                if (SceneMgr.GameType != Gametype.SOLO_GAME)
                {
                    NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                    (bullet as ISendable).WriteObject(msg);
                    SceneMgr.SendMessage(msg);
                }

                SceneMgr.AttachToScene(bullet);
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
