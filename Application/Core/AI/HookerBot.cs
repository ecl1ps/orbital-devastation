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
using System.Windows.Media;
using Orbit.Core.Scene.Controls.Implementations;

namespace Orbit.Core.AI
{
    /// <summary>
    /// bot urovne 2 - healuje se, strili nahodne z kanonu, pousti nahodne miny nad nepritelem a pouziva hook na nejblizsi asteroid, 
    /// nepocita s gravitaci, 
    /// na obou zbranich ma zpomaleni 2*
    /// </summary>
    public class HookerBot : IGameState
    {
        public const string NAME = "HookerBot";

        private SceneMgr sceneMgr;
        private List<ISceneObject> objects;
        private Player me;
        private Vector baseLauncherPosition;

        public HookerBot(SceneMgr mgr, List<ISceneObject> objects, Player bot)
        {
            sceneMgr = mgr;
            this.objects = objects;
            me = bot;
            baseLauncherPosition = new Vector(me.GetBaseLocation().X + me.GetBaseLocation().Width / 2, me.GetBaseLocation().Y - 5);

            me.Data.MineCooldown = me.Data.MineCooldown * 2f;
            me.Data.BulletCooldown = me.Data.BulletCooldown * 2f;
        }

        public void Update(float tpf)
        {
            me.Canoon.UpdateTimer(tpf);
            me.Mine.UpdateTimer(tpf);

            if (me.Canoon.IsReady())
                CannonShoot();

            if (me.Mine.IsReady())
                MineDrop();

            if (me.Hook.IsReady())
                ShootHook();

            CheckHeal();
        }

        // debug
        /*private Asteroid last;
        private VectorLine asteroidMovementLine;
        private VectorLine cLine;
        private Circle pX1;*/

        private Asteroid nearest;
        private double v1;
        private double v2;
        private Vector cVec;
        private Vector dVec;
        private double c;
        private double cosAlpha;
        private double D;
        private double sqrtD;
        private double x1;
        private Vector contactPoint1;

        private void ShootHook()
        {
            nearest = GetNearestAsteroid();
            if (nearest == null)
                return;

            // rychlost hooku
            v1 = me.Data.HookSpeed;
            // rychlost objektu
            v2 = nearest.Direction.Length;
            // vektor od objketu k launcheru hooku
            cVec = baseLauncherPosition - nearest.Center;
            // vektor smeru objektu
            dVec = (nearest as IMovable).Direction;

            /*if (last == null || nearest.Id != last.Id)
            {
                last = nearest;
                if (asteroidMovementLine != null)
                {
                    (asteroidMovementLine.GetControlOfType(typeof(VectorLineObjectMovementControl)) as VectorLineObjectMovementControl).Parent = nearest;
                }
                else
                {
                    asteroidMovementLine = SceneObjectFactory.CreateVectorLine(sceneMgr, nearest.Center, dVec, Colors.Yellow, nearest);
                    sceneMgr.DelayedAttachToScene(asteroidMovementLine);
                }
            }         
            if (cLine != null)
                cLine.DoRemoveMe();
            cLine = SceneObjectFactory.CreateVectorLine(sceneMgr, nearest.Center, cVec, Colors.Green);
            sceneMgr.DelayedAttachToScene(cLine);*/

            // vzdalenost mezi launcherem a objektem
            c = cVec.Length;
            // cosinus uhlu, ktery sviraji vektory pohybu objektu a smeru k launcheru
            cosAlpha = (cVec.X * dVec.X + cVec.Y * dVec.Y) / (cVec.Length * dVec.Length);

            // diskriminant pro kvadratickou rovnici cosinovy vety
            D = Math.Pow(2 * c * cosAlpha, 2) - 4 * c * c * (1 - Math.Pow(v1 / v2, 2));
            // nebyl nalezen trojuhelnik (komplexni cisla)
            if (D < 0)
                return;

            sqrtD = Math.Sqrt(D);

            // kvadraticka rovnice cosinovy vety
            // odectenim D ziskame bod pred telesem, prictenim bychom ziskali bod za telesem (ve smeru jeho pohybu)
            x1 = (2 * c * cosAlpha - sqrtD) / (2 - 2 * Math.Pow(v1 / v2, 2));

            dVec.Normalize();

            // bod do ktereho je potreba strelit
            contactPoint1 = nearest.Center + (dVec * x1);

            /*if (pX1 == null || pX1.Dead)
            {
                pX1 = SceneObjectFactory.CreateCircle(sceneMgr, contactPoint1, Colors.Blue);
                sceneMgr.DelayedAttachToScene(pX1);
            }
            pX1.Position = contactPoint1;
            sceneMgr.ShowStatusText(7, "x1 " + ((int)contactPoint1.X) + " " + ((int)contactPoint1.Y));*/

            // nestrili, pokud tam nedosahne
            if ((baseLauncherPosition - contactPoint1).Length > me.Data.HookLenght)
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
            double nearestDistSqr = 10000000;
            double objDistantSqr = 10000000;

            foreach (ISceneObject obj in objects)
            {
                if (!(obj is Asteroid))
                    continue;

                objDistantSqr = (obj.Position - baseLauncherPosition).LengthSquared;
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
            if (me.Data.BaseIntegrity <= SharedDef.BASE_MAX_INGERITY - SharedDef.HEAL_AMOUNT)
                me.HealingKit.Heal();
        }

        private void MineDrop()
        {
            Rect opponentLoc = PlayerBaseLocation.GetBaseLocation(me.GetPosition() == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT);
            double xMin = opponentLoc.X;
            double xMax = opponentLoc.X + opponentLoc.Width;
            me.Mine.Shoot(new Point(sceneMgr.GetRandomGenerator().Next((int)xMin, (int)xMax), 1));
 
        }

        private void CannonShoot()
        {
            double xMin = 0, xMax = 0;
            if (me.GetPosition() == PlayerPosition.LEFT)
            {
                xMin = SharedDef.VIEW_PORT_SIZE.Width * 0.25;
                xMax = SharedDef.VIEW_PORT_SIZE.Width;
            }
            else
            {
                xMin = 0;
                xMax = SharedDef.VIEW_PORT_SIZE.Width * 0.75;
            }
            me.Canoon.Shoot(new Point(sceneMgr.GetRandomGenerator().Next((int)xMin, (int)xMax), 0));
        }
    }
}
