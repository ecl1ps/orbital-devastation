using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Weapons;
using System.Windows;
using Orbit.Core.Players;
using Orbit.Core.Helpers;
using Microsoft.Xna.Framework;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Controls;

namespace Orbit.Core.AI
{
    /// <summary>
    /// bot urovne 2 - healuje se, strili nahodne z kanonu, pousti nahodne miny nad nepritelem a pouziva hook na nejblizsi asteroid, 
    /// nepocita s gravitaci, 
    /// na obou zbranich ma zpomaleni 2*
    /// </summary>
    public class HookerBot : IGameState
    {
        private SceneMgr sceneMgr;
        private List<ISceneObject> objects;
        private Player me;
        private Vector2 baseLauncherPosition;

        public HookerBot(SceneMgr mgr, List<ISceneObject> objects, Player bot)
        {
            sceneMgr = mgr;
            this.objects = objects;
            me = bot;
            baseLauncherPosition = new Vector2(me.GetBaseLocation().X + me.GetBaseLocation().Width / 2, me.GetBaseLocation().Y - 5);

            me.Data.MineCooldown = me.Data.MineCooldown * 2f;
            me.Data.BulletCooldown = me.Data.BulletCooldown * 2f;
        }

        public void Update(float tpf)
        {
            if (me.Canoon.IsReady())
                CannonShoot();

            if (me.Mine.IsReady())
                MineDrop();

            if (me.Hook.IsReady())
                ShootHook();

            CheckHeal();
        }

        private void ShootHook()
        {
            Asteroid nearest = GetNearestAsteroid();
            if (nearest == null)
                return;

            Vector2 contactPoint1 = AIUtils.ComputeDestinationPositionToHitTarget(nearest, me.Data.HookSpeed, baseLauncherPosition, me.SceneMgr.GetRandomGenerator());

            // nestrili, pokud tam nedosahne
            if ((baseLauncherPosition - contactPoint1).Length() > me.Data.HookLenght)
                return;

            // nestrili pod bazi
            if (me.GetBaseLocation().Y < contactPoint1.Y)
                return;

            // nestrili, pokud to je mimo scenu
            if (SceneMgr.IsPointInViewPort(contactPoint1.ToPoint()))
                me.Hook.Shoot(contactPoint1.ToPoint());
        }

        private Asteroid GetNearestAsteroid()
        {
            ISceneObject nearest = null;
            float nearestDistSqr = 10000000;
            float objDistantSqr = 10000000;

            foreach (ISceneObject obj in objects)
            {
                if (!(obj is Asteroid))
                    continue;

                objDistantSqr = (obj.Position - baseLauncherPosition).LengthSquared();
                if (objDistantSqr < nearestDistSqr)
                {
                    nearestDistSqr = objDistantSqr;
                    nearest = obj;
                }
            }

            return nearest == null ? null : nearest as Asteroid;
        }

        private void CheckHeal()
        {
            if (me.Data.BaseIntegrity + me.HealingKit.GetHealAmount() <= me.Data.MaxBaseIntegrity &&
                me.Data.Gold >= me.HealingKit.Cost)
            {
                me.Data.Gold -= me.HealingKit.Cost;
                me.HealingKit.Heal();
            }
        }

        private void MineDrop()
        {
            Rectangle opponentLoc = PlayerBaseLocation.GetBaseLocation(me.GetPosition() == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT);
            double xMin = opponentLoc.X;
            double xMax = opponentLoc.X + opponentLoc.Width;
            me.Mine.Shoot(new Vector2(sceneMgr.GetRandomGenerator().Next((int)xMin, (int)xMax), 1));
 
        }

        private void CannonShoot()
        {
            me.Canoon.Shoot(new Vector2(sceneMgr.GetRandomGenerator().Next(0, (int)SharedDef.VIEW_PORT_SIZE.Width), 0));
        }
    }
}
