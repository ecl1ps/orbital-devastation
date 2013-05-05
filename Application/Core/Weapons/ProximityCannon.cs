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
using Orbit.Core.Client.GameStates;
using Orbit.Core.SpecialActions;
using Orbit.Core.SpecialActions.Gamer;

namespace Orbit.Core.Weapons
{
    public class ProximityCannon : IWeapon
    {
        public Player Owner { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public float ReloadTime { get; set;}
        public int Cost { get; set; }
        public DeviceType DeviceType { get; set; }
        public UpgradeLevel UpgradeLevel { get; set; }
        public String Name { get; set; }

        private Boolean shooting;
        protected ISpecialAction next;

        private List<IWeaponClickListener> listeners = new List<IWeaponClickListener>();


        public ProximityCannon(SceneMgr mgr, Player owner)
        {
            SceneMgr = mgr;
            Owner = owner;
            DeviceType = DeviceType.CANNON;
            UpgradeLevel = UpgradeLevel.LEVEL1;
        }

        public virtual ISpecialAction NextSpecialAction()
        {
            if (next == null)
                next = new WeaponUpgrade(new ProximityCannonII(SceneMgr, Owner));

            return next;
        }

        public ISceneObject Shoot(Point point, bool noControl = false)
        {
            if (IsReady() || noControl)
            {
                ISceneObject obj = SpawnBullet(point);
                if(!noControl)
                    ReloadTime = Owner.Data.BulletCooldown;
                //SoundManager.Instance.StartPlayingOnce(SharedDef.MUSIC_SHOOT);
                Owner.Statistics.BulletFired++;
                return obj;
            }

            return null;
        }

        protected virtual ISceneObject SpawnBullet(Point point)
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
            return bullet;
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


        public virtual void ProccessClickEvent(Point point, MouseButton button, MouseButtonState buttonState)
        {
            foreach (IWeaponClickListener listener in listeners)
            {
                if (listener.ProccessClickEvent(point, button, buttonState))
                    return;
            }

            shooting = buttonState == MouseButtonState.Pressed;
        }

        public void Update(float tpf)
        {
            if (ReloadTime > 0)
                ReloadTime -= tpf;
            else if(shooting)
                Shoot(StaticMouse.GetPosition());

        }

        public IWeaponClickListener AddClickListener(IWeaponClickListener listener)
        {
            listeners.Add(listener);
            return listener;
        }

        public void RemoveClickListener(IWeaponClickListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
