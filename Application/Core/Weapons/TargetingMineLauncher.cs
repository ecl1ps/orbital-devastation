using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
        protected Point startPoint;
        protected Point endPoint;
        protected bool targeting = false;
        private DrawingGroup lineGeom;

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

        public override void ProccessClickEvent(Point point, MouseButton button, MouseButtonState state)
        {
            if (state == MouseButtonState.Pressed && IsReady())
            {
                targeting = true;
                startPoint = new Point(point.X, 0);
                endPoint = point;
                PrepareLine();
            }
            else if (state == MouseButtonState.Released && targeting)
            {
                targeting = false;
                endPoint = point;
                RemoveLine();
                Shoot(startPoint);
            }
        }

        private void RemoveLine()
        {
            SceneMgr.Invoke(new Action(() =>
            {
                SceneMgr.RemoveGraphicalObjectFromScene(lineGeom);
            }));
        }

        private void PrepareLine()
        {
            lineGeom = SceneGeometryFactory.CreateLineGeometry(SceneMgr, Colors.Crimson, 1, Colors.Red, startPoint.ToVector(), endPoint.ToVector());
            SceneMgr.AttachGraphicalObjectToScene(lineGeom);
        }

        protected override void SpawnMine(Point point)
        {
            SingularityMine mine = SceneObjectFactory.CreateDroppingSingularityMine(SceneMgr, point, Owner);
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

            SceneMgr.DelayedAttachToScene(mine);
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);

            if (targeting)
            {
                endPoint = StaticMouse.GetPosition();
                MoveLine();
            }
        }

        private void MoveLine()
        {
            SceneMgr.Invoke(new Action(() => 
            {
                Vector v = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                if (v.Length != 0)
                    v.Normalize();

                //FIXME chtelo by to vypocitat kolizi
                v *= 1000;
                v.X += startPoint.X;
                v.Y += startPoint.Y;
                ((lineGeom.Children[0] as GeometryDrawing).Geometry as LineGeometry).EndPoint = v.ToPoint();
            }));
        }
    }
}
