using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Players;
using System.Windows;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using Lidgren.Network;
using Orbit.Core.Scene.Entities;
using Orbit.Core.SpecialActions;

namespace Orbit.Core.Weapons
{
    public class ProximityCannonIII : ProximityCannonII
    {
        public ProximityCannonIII(SceneMgr mgr, Player owner)
            : base(mgr, owner)
        {
            Cost = 700;
            Name = "Cannon Mark 3";
            UpgradeLevel = UpgradeLevel.LEVEL3;
        }

        public override ISpecialAction NextSpecialAction()
        {
            //i dont have next - but don't worry, you will ;)
            return null;
        }

        protected override void SpawnBullet(Point point)
        {
            if (point.Y > Owner.GetBaseLocation().Y)
                point.Y = Owner.GetBaseLocation().Y;

            SingularityBouncingBullet bullet = SceneObjectFactory.CreateSingularityBouncingBullet(SceneMgr, point, Owner);

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            bullet.WriteObject(msg);
            SceneMgr.SendMessage(msg);

            SceneMgr.DelayedAttachToScene(bullet);
        }
    }
}
