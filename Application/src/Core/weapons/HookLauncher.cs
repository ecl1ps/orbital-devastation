﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Scene;
using Orbit.Core.Players;
using Lidgren.Network;

namespace Orbit.Core.Weapons
{
    class HookLauncher : IWeapon
    {
        public ISceneMgr SceneMgr { get; set; }
        public float ReloadTime { get; set; }
        public int Cost { get; set; }

        private Hook hook;

        public HookLauncher(ISceneMgr mgr)
        {
            SceneMgr = mgr;
        }

        public void Shoot(Point point)
        {
            hook = SpawnHook(point);
        }

        public IWeapon Next()
        {
            return null;
        }

        private Hook SpawnHook(Point point)
        {
            Player player = SceneMgr.GetMePlayer();
            Hook hook = null;

            if (point.Y > PlayerPositionProvider.GetVectorPosition(player.GetPosition()).Y)
                return hook;

            if (IsReady())
            {
                hook = SceneObjectFactory.CreateHook(SceneMgr, point, player);

                if (SceneMgr.GameType != Gametype.SOLO_GAME)
                {
                    NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                    (hook as ISendable).WriteObject(msg);
                    SceneMgr.SendMessage(msg);
                }

                SceneMgr.AttachToScene(hook);
            }

            return hook;
        }

        public bool IsReady()
        {
            return hook == null || hook.Dead;
        }

        public void UpdateTimer(float value)
        {
            //i dont need this
        }
    }
}
