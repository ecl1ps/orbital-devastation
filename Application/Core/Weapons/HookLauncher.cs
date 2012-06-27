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
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using System.Windows.Input;

namespace Orbit.Core.Weapons
{
    public class HookLauncher : IWeapon
    {
        public Player Owner { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public float ReloadTime { get; set; }
        public int Cost { get; set; }
        public String Name { get; set; }
        public WeaponType WeaponType { get; set; }

        protected Hook hook;
        protected IWeapon next;

        public HookLauncher(SceneMgr mgr, Player owner)
        {
            SceneMgr = mgr;
            Owner = owner;
            Name = "Hook launcher";
            WeaponType = WeaponType.HOOK;
            ReloadTime = 0;
        }

        public void Shoot(Point point)
        {
            if(IsReady()) {
                SpawnHook(point);
                ReloadTime = Owner.Data.HookCooldown;
            }
        }

        public virtual IWeapon Next()
        {
            if (next == null)
                next = new DoubleHookLauncher(SceneMgr, Owner);

            return next;
        }

        protected virtual void SpawnHook(Point point)
        {
            if (point.Y > Owner.GetBaseLocation().Y - 5)
                point.Y = Owner.GetBaseLocation().Y - 5;

                hook = CreateHook(point);

                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                hook.WriteObject(msg);
                SceneMgr.SendMessage(msg);

                SceneMgr.DelayedAttachToScene(hook);
        }

        protected virtual Hook CreateHook(Point point)
        {
            return SceneObjectFactory.CreateHook(SceneMgr, point, Owner);
        }

        public virtual bool IsReady()
        {
            return (hook == null || hook.Dead) && ReloadTime <= 0;
        }

        public virtual void TriggerUpgrade(IWeapon old)
        {
            if (old != null)
                old.SceneMgr.StateMgr.RemoveGameState(old);

            SceneMgr.StateMgr.AddGameState(this);
        }

        public Hook getHook()
        {
            return hook;
        }


        public void ProccessClickEvent(Point point, MouseButton button, MouseButtonState state)
        {
            if (button == MouseButton.Middle && state == MouseButtonState.Pressed)
                Shoot(point);
        }

        public virtual void Update(float tpf)
        {
            if (ReloadTime > 0)
                ReloadTime -= tpf;
        }
    }
}
