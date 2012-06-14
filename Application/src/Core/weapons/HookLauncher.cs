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
using Orbit.src.Core.Weapons;

namespace Orbit.Core.Weapons
{
    class HookLauncher : IWeapon
    {
        public Player Owner { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public float ReloadTime { get; set; }
        public int Cost { get; set; }
        public String Name { get; set; }
        public WeaponType WeaponType { get; set; }

        protected Hook hook;
        protected IWeapon next;

        public HookLauncher()
        {
            Name = "Hook launcher";
            WeaponType = WeaponType.HOOK;
        }

        public HookLauncher(SceneMgr mgr, Player owner)
        {
            SceneMgr = mgr;
            Owner = owner;
        }

        public void Shoot(Point point)
        {
            SpawnHook(point);
        }

        public virtual IWeapon Next()
        {
            if(next == null)
                next = new DoubleHookLauncher();

            return next;
        }

        protected virtual void SpawnHook(Point point)
        {

            if (point.Y > Owner.VectorPosition.Y)
                return;

            if (IsReady())
            {
                hook = createHook(point, Owner);

                if (SceneMgr.GameType != Gametype.SOLO_GAME)
                {
                    NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                    (hook as ISendable).WriteObject(msg);
                    SceneMgr.SendMessage(msg);
                }

                SceneMgr.AttachToScene(hook);
            }
        }

        protected virtual Hook createHook(Point point, Player player)
        {
            return SceneObjectFactory.CreateHook(SceneMgr, point, player);
        }

        public virtual bool IsReady()
        {
            return hook == null || hook.Dead;
        }

        public void UpdateTimer(float value)
        {
            //i dont need this
        }
    }
}
