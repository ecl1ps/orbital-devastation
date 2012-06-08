using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using System.Windows;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene;
using Lidgren.Network;

namespace Orbit.Core.Weapons
{
    class MineLauncher : IWeapon
    {
        public ISceneMgr SceneMgr { get; set; }
        public float ReloadTime { get; set; }
        public int Cost { get; set; }

        public MineLauncher(ISceneMgr mgr)
        {
            SceneMgr = mgr;
        }

        public IWeapon Next()
        {
            return null;
        }

        public void Shoot(Point point)
        {
            if (IsReady())
            {
                SpawnMine(point);
                ReloadTime = SharedDef.MINE_COOLDOWN;
            }
        }

        private void SpawnMine(Point point)
        {
                SingularityMine mine = SceneObjectFactory.CreateDroppingSingularityMine(this, point, SceneMgr.GetMePlayer());

                if (SceneMgr.GameType != Gametype.SOLO_GAME)
                {
                    NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                    (mine as ISendable).WriteObject(msg);
                    SceneMgr.SendMessage(msg);
                }

                SceneMgr.AttachToScene(mine);
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
