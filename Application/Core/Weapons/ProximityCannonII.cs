using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene;
using System.Windows;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Lidgren.Network;
using Orbit.Core.SpecialActions;
using Orbit.Core.SpecialActions.Gamer;

namespace Orbit.Core.Weapons
{
    public class ProximityCannonII : ProximityCannon
    {
        public ProximityCannonII(SceneMgr mgr, Player owner) : base(mgr, owner)
        {
            Cost = 300;
            Name = "Proximity Cannon II";
            UpgradeLevel = UpgradeLevel.LEVEL2;
        }

        public override ISpecialAction NextSpecialAction()
        {
            if (next == null)
                next = new WeaponUpgrade(new ProximityCannonIII(SceneMgr, Owner));

            return next;
        }

        protected override ISceneObject SpawnBullet(Point point)
        {
            if (point.Y > Owner.GetBaseLocation().Y)
                point.Y = Owner.GetBaseLocation().Y;

            SingularityExplodingBullet bullet = SceneObjectFactory.CreateSingularityExploadingBullet(SceneMgr, point.ToVector(), Owner);

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            (bullet as ISendable).WriteObject(msg);
            SceneMgr.SendMessage(msg);

            SceneMgr.DelayedAttachToScene(bullet);

            return bullet;
        }
    }
}
