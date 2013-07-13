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
using Orbit.Core.SpecialActions;
using Orbit.Core.SpecialActions.Gamer;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Weapons
{
    public class HookLauncher : IWeapon
    {
        public Player Owner { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public float ReloadTime { get; set; }
        public int Cost { get; set; }
        public String Name { get; set; }
        public DeviceType DeviceType { get; set; }
        public UpgradeLevel UpgradeLevel { get; set; }

        protected Hook hook;
        protected ISpecialAction next;

        private List<IWeaponClickListener> listeners = new List<IWeaponClickListener>();


        public HookLauncher(SceneMgr mgr, Player owner)
        {
            SceneMgr = mgr;
            Owner = owner;
            Name = "Hook launcher";
            DeviceType = DeviceType.HOOK;
            UpgradeLevel = UpgradeLevel.LEVEL1;
            ReloadTime = 0;
        }

        public ISceneObject Shoot(Vector2 point, bool noControl = false)
        {
            if (IsReady() || noControl) {
                ISceneObject obj = SpawnHook(point);
                if(!noControl)
                    ReloadTime = Owner.Data.HookCooldown;
                
                Owner.Statistics.HookFired++;

                return obj;
            }

            return null;
        }

        public virtual ISpecialAction NextSpecialAction()
        {
            if (next == null)
                next = new WeaponUpgrade(new DoubleHookLauncher(SceneMgr, Owner));

            return next;
        }

        protected virtual ISceneObject SpawnHook(Vector2 point)
        {
            if (point.Y > Owner.GetBaseLocation().Y - 5)
                point.Y = Owner.GetBaseLocation().Y - 5;

                hook = CreateHook(point);

                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                hook.WriteObject(msg);
                SceneMgr.SendMessage(msg);

                SceneMgr.DelayedAttachToScene(hook);

                return hook;
        }

        protected virtual Hook CreateHook(Vector2 point)
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


        public virtual void ProccessClickEvent(Vector2 point, MouseButton button, MouseButtonState state)
        {
            foreach (IWeaponClickListener listener in listeners)
            {
                if (listener.ProccessClickEvent(point, button, state))
                    return;
            }

            if (state == MouseButtonState.Pressed)
                Shoot(point);
        }

        public virtual void Update(float tpf)
        {
            if (ReloadTime > 0)
                ReloadTime -= tpf;
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
