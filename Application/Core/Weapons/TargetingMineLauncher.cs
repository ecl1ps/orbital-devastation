using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using Lidgren.Network;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Client.GameStates;
using Orbit.Core.SpecialActions;
using Orbit.Core.SpecialActions.Gamer;

namespace Orbit.Core.Weapons
{
    public class TargetingMineLauncher : MineLauncher
    {
        protected Vector2 startPoint;
        protected Vector2 endPoint;
        protected bool targeting = false;

        public TargetingMineLauncher(SceneMgr mgr, Player player) : base(mgr, player)
        {
            Cost = 350;
            Name = "Targeted mine";
            UpgradeLevel = UpgradeLevel.LEVEL2;
        }

        public override ISpecialAction NextSpecialAction()
        {
            if (next == null)
                next = new WeaponUpgrade(new AsteroidMineLauncher(SceneMgr, Owner));

            return next;
        }

        public override void ProccessClickEvent(Vector2 point, MouseButtons button, bool down)
        {
            if (down && IsReady())
            {
                targeting = true;
                startPoint = new Vector2(point.X, 0);
                endPoint = point;
            }
            else if (!down && targeting)
            {
                targeting = false;
                endPoint = point;
                Shoot(startPoint);
            }
        }

        protected override ISceneObject SpawnMine(Vector2 point)
        {
            SingularityMine mine = SceneObjectFactory.CreateDroppingSingularityMine(SceneMgr, point, Owner);
            Vector2 dir = new Vector2(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
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

            SceneMgr.DelayedAttachToScene(mine);
            return mine;
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);

            if (targeting)
            {
                MoveLine();
            }
        }

        private void MoveLine()
        {
            SceneMgr.Invoke(new Action(() => 
            {
                Vector2 v = new Vector2(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                if (v.Length() != 0)
                    v.Normalize();

                //FIXME chtelo by to vypocitat kolizi
                v *= 1000;
                v.X += startPoint.X;
                v.Y += startPoint.Y;
            }));
        }
    }
}
