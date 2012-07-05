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
using System.Windows.Input;

namespace Orbit.Core.Weapons
{
    class ProximityCannon : IWeapon
    {
        public Player Owner { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public float ReloadTime { get; set;}
        public int Cost { get; set; }
        public DeviceType DeviceType { get; set; }
        public UpgradeLevel UpgradeLevel { get; set; }
        public String Name { get; set; }

        private Boolean shooting;
        protected IWeapon next;

        public ProximityCannon(SceneMgr mgr, Player owner)
        {
            SceneMgr = mgr;
            Owner = owner;
            DeviceType = DeviceType.CANNON;
            UpgradeLevel = UpgradeLevel.LEVEL1;
        }

        public virtual IWeapon Next()
        {
            if (next == null)
                next = new ProximityCannonII(SceneMgr, Owner);
            return next;
        }

        public void Shoot(Point point)
        {
            if (IsReady())
            {
                SpawnBullet(point);
                ReloadTime = Owner.Data.BulletCooldown;
            }
        }

        protected virtual void SpawnBullet(Point point)
        {
            if (point.Y > Owner.GetBaseLocation().Y)
                point.Y = Owner.GetBaseLocation().Y;

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

        public void TriggerUpgrade(IWeapon old)
        {
            if (old != null)
                old.SceneMgr.StateMgr.RemoveGameState(old);
            
            SceneMgr.StateMgr.AddGameState(this);
        }


        public void ProccessClickEvent(Point point, MouseButton button, MouseButtonState buttonState)
        {
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
