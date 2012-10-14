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

namespace Orbit.Core.Weapons
{
    class AsteroidMineLauncher : TargetingMineLauncher
    {
        public AsteroidMineLauncher(SceneMgr mgr, Player plr)
            : base(mgr, plr)
        {
            Name = "Asteroid Carrier";
            Cost = 750;
        }

        public override IWeapon Next()
        {
            return null;
        }

        protected override void SpawnMine(System.Windows.Point point)
        {
            SingularityMine mine = SceneObjectFactory.CreateAsteroidDroppingSingularityMine(SceneMgr, point, Owner);
            Vector dir = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
            dir.Normalize();
            mine.Direction = dir;
            mine.Direction *= SharedDef.MINE_LAUNCHER_SPEED_MODIFIER * SharedDef.MINE_FALLING_SPEED;

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            (mine as ISendable).WriteObject(msg);
            SceneMgr.SendMessage(msg);

            SceneMgr.DelayedAttachToScene(mine);
        }
    }
}
