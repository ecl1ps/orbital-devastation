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

        protected IWeapon next = null;

        public MineLauncher(SceneMgr mgr, Player owner)
        {
            SceneMgr = mgr;
            Owner = owner;
            DeviceType = DeviceType.MINE;
            UpgradeLevel = UpgradeLevel.LEVEL1;
        }

        public virtual IWeapon Next()
        {
            if (next == null)
                next = new TargetingMineLauncher(SceneMgr, Owner);
            
            return next;
        }

        public void Shoot(Point point)
        {
            if (IsReady())
            {
                SpawnMine(point);
                ReloadTime = Owner.Data.MineCooldown;
            }
        }

        protected virtual void SpawnMine(Point point)
        {
                SingularityMine mine = SceneObjectFactory.CreateDroppingSingularityMine(SceneMgr, point, Owner);

                if (SceneMgr.GameType != Gametype.SOLO_GAME)
                {
                    NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                    (mine as ISendable).WriteObject(msg);
                    SceneMgr.SendMessage(msg);
                }

                SceneMgr.DelayedAttachToScene(mine);
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


        virtual public void ProccessClickEvent(Point point, MouseButton button, MouseButtonState state)
        {
            if (state == MouseButtonState.Pressed)
                Shoot(point);
        }

        virtual public void Update(float tpf)
        {
            if (ReloadTime > 0)
                ReloadTime -= tpf;
        }
    }
}
