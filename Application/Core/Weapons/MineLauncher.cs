using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using System.Windows;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene;
using Lidgren.Network;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using System.Windows.Input;
using Orbit.Core.SpecialActions;
using Orbit.Core.SpecialActions.Gamer;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Weapons
{
    public class MineLauncher : IWeapon
    {
        public Player Owner { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public float ReloadTime { get; set; }
        public int Cost { get; set; }
        public DeviceType DeviceType { get; set; }
        public UpgradeLevel UpgradeLevel { get; set; }
        public String Name { get; set; }

        protected ISpecialAction next = null;

        private List<IWeaponClickListener> listeners = new List<IWeaponClickListener>();


        public MineLauncher(SceneMgr mgr, Player owner)
        {
            SceneMgr = mgr;
            Owner = owner;
            DeviceType = DeviceType.MINE;
            UpgradeLevel = UpgradeLevel.LEVEL1;
        }

        public virtual ISpecialAction NextSpecialAction()
        {
            if (next == null)
                next = new WeaponUpgrade(new TargetingMineLauncher(SceneMgr, Owner));

            return next;
        }

        public ISceneObject Shoot(Vector2 point, bool noControl = false)
        {
            if (IsReady() || noControl)
            {
                ISceneObject obj = SpawnMine(point);
                if(!noControl)
                    ReloadTime = Owner.Data.MineCooldown;

                Owner.Statistics.MineFired++;
                return obj;
            }

            return null;
        }

        protected virtual ISceneObject SpawnMine(Vector2 point)
        {
            SingularityMine mine = SceneObjectFactory.CreateDroppingSingularityMine(SceneMgr, point, Owner);

            if (SceneMgr.GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                (mine as ISendable).WriteObject(msg);
                SceneMgr.SendMessage(msg);
            }

            SceneMgr.DelayedAttachToScene(mine);
            return mine;
        }

        public bool IsReady()
        {
            return ReloadTime <= 0;
        }

        public virtual void TriggerUpgrade(IWeapon old)
        {
            if (old != null)
                old.SceneMgr.StateMgr.RemoveGameState(old);

            SceneMgr.StateMgr.AddGameState(this);
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

        virtual public void Update(float tpf)
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
