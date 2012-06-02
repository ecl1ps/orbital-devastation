using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Scene;
using Orbit.Core.Players;
using Lidgren.Network;

namespace Orbit.src.Core.weapons
{
    class HookLauncher : IWeapon
    {
        public float ReloadTime { get; set; }
        public int Cost { get; set; }

        private Hook hook;

        public void Shoot(Point point)
        {
            spawnHook(point);
        }

        public IWeapon Next()
        {
            return null;
        }

        private void spawnHook(Point point)
        {
            Player player = SceneMgr.GetInstance().GetMePlayer();

            if (point.Y > PlayerPositionProvider.getVectorPosition(player.GetPosition()).Y)
                return;

            if (IsReady())
            {
                hook = SceneObjectFactory.CreateHook(point, player);

                if (SceneMgr.GetInstance().GameType != Gametype.SOLO_GAME)
                {
                    NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                    (hook as ISendable).WriteObject(msg);
                    SceneMgr.SendMessage(msg);
                }

                SceneMgr.GetInstance().AttachToScene(hook);
            }
        }

        public bool IsReady()
        {
            return hook == null || hook.Dead;
        }

        public void updateTimer(float value)
        {
            //i dont need this
        }
    }
}
