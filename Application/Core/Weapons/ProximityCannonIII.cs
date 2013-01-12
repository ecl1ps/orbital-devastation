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
using Orbit.Core.SpecialActions.Gamer;
using Orbit.Core.Scene.Controls.Implementations;

namespace Orbit.Core.Weapons
{
    public class ProximityCannonIII : ProximityCannonII, IActivableWeapon
    {
        public ActivableData Data { get; set; }

        private SingularityBouncingBullet lastBullet;

        public ProximityCannonIII(SceneMgr mgr, Player owner)
            : base(mgr, owner)
        {
            Cost = 700;
            Name = "Cannon Mark 3";
            UpgradeLevel = UpgradeLevel.LEVEL3;
            Data = new ActivableData("Intelligent Bullets", "active.png", 7);
        }

        public override ISpecialAction NextSpecialAction()
        {
            if (next == null)
                next = new ActiveWeapon(this);

            return next;
        }

        protected override void SpawnBullet(Point point)
        {
            if (lastBullet != null)
                lastBullet.GetControlOfType<HighlightingControl>().Enabled = false;

            if (point.Y > Owner.GetBaseLocation().Y)
                point.Y = Owner.GetBaseLocation().Y;

            lastBullet = SceneObjectFactory.CreateSingularityBouncingBullet(SceneMgr, point, Owner);

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            lastBullet.WriteObject(msg);
            SceneMgr.SendMessage(msg);

            HighlightingControl hc = new HighlightingControl();
            lastBullet.AddControl(hc);

            SceneMgr.DelayedAttachToScene(lastBullet);
        }

        public bool IsActivableReady()
        {
            bool ready = lastBullet != null && !lastBullet.Dead;

            if (ready)
                lastBullet.GetControlOfType<HighlightingControl>().Enabled = true;

            return ready;
        }

        public void StartActivableAction()
        {
            lastBullet.GetControlOfType<BouncingSingularityBulletControl>().EmitBullets();
        }
    }
}
