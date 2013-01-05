using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using System.Windows;
using Lidgren.Network;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.SpecialActions;
using Orbit.Core.SpecialActions.Gamer;

namespace Orbit.Core.Weapons
{
    public class AsteroidMineLauncher : TargetingMineLauncher, IActivableWeapon
    {
        public ActivableData Data { get; set; }

        private SingularityMine lastMine;

        public AsteroidMineLauncher(SceneMgr mgr, Player plr) : base(mgr, plr)
        {
            Name = "Asteroid Carrier";
            Cost = 750;
            UpgradeLevel = UpgradeLevel.LEVEL3;
            Data = new ActivableData("Artificial Asteroid", "active.png", 8);
        }

        public override ISpecialAction NextSpecialAction()
        {
            if (next == null)
                next = new ActiveWeapon(this);

            return next;
        }

        protected override void SpawnMine(Point point)
        {
            SingularityMine mine = SceneObjectFactory.CreateAsteroidDroppingSingularityMine(SceneMgr, point, Owner);
            Vector dir = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
            dir.Normalize();
            mine.Direction = dir;

            LinearMovementControl c = mine.GetControlOfType<LinearMovementControl>();
            c.Speed = Owner.Data.MineFallingSpeed * SharedDef.MINE_LAUNCHER_SPEED_MODIFIER;

            if (SceneMgr.GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                (mine as ISendable).WriteObject(msg);
                SceneMgr.SendMessage(msg);
            }

            HighlightingControl hc = new HighlightingControl();
            mine.AddControl(hc);

            lastMine = mine;
            SceneMgr.DelayedAttachToScene(mine);
        }

        public bool IsActivableReady()
        {
            bool ready = lastMine != null && !lastMine.Dead && !lastMine.GetControlOfType<AsteroidDroppingSingularityControl>().Detonated 
                && !lastMine.GetControlOfType<AsteroidDroppingSingularityControl>().AsteroidSpawned;

            if (ready)
                lastMine.GetControlOfType<HighlightingControl>().Enabled = true;

            return ready;
        }

        public void StartActivableAction()
        {
            lastMine.GetControlOfType<AsteroidDroppingSingularityControl>().SpawnAsteroid();
        }
    }
}
