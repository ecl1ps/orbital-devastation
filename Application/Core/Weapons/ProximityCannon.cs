﻿using System;
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
using System.Windows.Input;

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

        private Boolean shooting;

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

        public void triggerUpgrade(IWeapon old)
        {
            if(old != null)
                old.SceneMgr.StateMgr.RemoveGameState(old);
            
            SceneMgr.StateMgr.AddGameState(this);
        }


        public void ProccessClickEvent(Point point, MouseButton button, MouseButtonState buttonState)
        {
            if (button != MouseButton.Right)
                return;

            shooting = buttonState == MouseButtonState.Pressed;
        }

        public void Update(float tpf)
        {
            if (ReloadTime > 0)
                ReloadTime -= tpf;
            else if(shooting)
                Shoot(StaticMouse.GetPosition());

        }
    }
}
