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
using Orbit.Core.Scene.Controls;
using Orbit.Core.SpecialActions.Gamer;

namespace Orbit.Core.AI
{
    /// <summary>
    /// bot urovne 3 - nestrili nahodne, vylepsuje se pokud mu zbyvaji
    /// penize na jeden heal, nesnazi se sestrelit pritahovanej asteroid
    /// na obou zbranich ma zpomaleni 2*
    /// </summary>
    public class MedicoreBot : IGameState
    {



        public const string NAME = "MedicoreBot";
         
        private SceneMgr sceneMgr;
        private List<ISceneObject> objects;
        private List<ISceneObject> targets, oncatche;
        
        private Player me;
        private Vector baseLauncherPosition;

        public MedicoreBot(SceneMgr mgr, List<ISceneObject> objects, Player bot)
        {
            sceneMgr = mgr;
            this.objects = objects;
            me = bot;
            baseLauncherPosition = new Vector(me.GetBaseLocation().X + me.GetBaseLocation().Width / 2, me.GetBaseLocation().Y - 5);


            me.Data.MineCooldown = me.Data.MineCooldown * 2f;
            me.Data.BulletCooldown = me.Data.BulletCooldown * 2f;
             targets = new List<ISceneObject>();
             oncatche = new List<ISceneObject>();
        }

        public void Update(float tpf)
        {

            
            GetTargets();
            
            if (me.Mine.IsReady())
                MineDrop();

            

            if (me.Hook.IsReady())
                ShootHook();
            if (me.Canoon.IsReady())
                CannonShoot();


          

           
            CheckHeal();
            CheckUpgrade();

          
        }

        private void ShootHook()
        {
            ICatchable nearest;
            

            if (targets.Count > 0)
            {

                nearest = GetNearestTarget();

            }
            else {

                nearest = GetHookTarget();
            
            }

            if (nearest == null)
                return;

            Vector contactPoint1 = AIUtils.ComputeDestinationPositionToHitTarget(nearest, me.Data.HookSpeed, baseLauncherPosition, me.SceneMgr.GetRandomGenerator());

            // nestrili, pokud tam nedosahne
            if ((baseLauncherPosition - contactPoint1).Length > me.Data.HookLenght)
                return;

            // nestrili pod bazi
            if (me.GetBaseLocation().Y < contactPoint1.Y)
                return;

            // nestrili, pokud to je mimo scenu
            if (SceneMgr.IsPointInViewPort(contactPoint1.ToPoint()))
            {
                me.Hook.Shoot(contactPoint1.ToPoint());
                oncatche.Add(nearest);
              if(targets.Contains(nearest))  targets.Remove(nearest);
            }
           
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

               Asteroid current = obj as Asteroid;
               Console.WriteLine(current.Position);
               Console.WriteLine(current.Direction);
                
                objDistantSqr = (obj.Position - baseLauncherPosition).LengthSquared;
                if (objDistantSqr < nearestDistSqr)
                {
                    nearestDistSqr = objDistantSqr;
                    nearest = obj;
                }
            }

            return nearest == null ? null : nearest as Asteroid;
        }



        private Asteroid GetHookTarget()
        {
            ISceneObject nearest = null;
            double nearestDistSqr = 10000000;
            double objDistantSqr = 10000000;

            foreach (ISceneObject obj in targets)
            {
                if (!(obj is Asteroid))
                {

                    Asteroid asteroid = obj as Asteroid;


                    objDistantSqr = (asteroid.Position - baseLauncherPosition).LengthSquared;
                    if (objDistantSqr < nearestDistSqr && asteroid.AsteroidType == AsteroidType.GOLDEN)
                    {
                        nearestDistSqr = objDistantSqr;
                        nearest = asteroid;
                    }

                }
                else if (!(obj is StatPowerUp))
                {
                    StatPowerUp powerup = obj as StatPowerUp;


                    objDistantSqr = (powerup.Position - baseLauncherPosition).LengthSquared;
                    if (objDistantSqr < nearestDistSqr)
                    {
                        nearestDistSqr = objDistantSqr;
                        nearest = powerup;
                    }
                
                
                } 

            }

            return nearest == null ? null : nearest as Asteroid;
        }



        private Asteroid GetNearestTarget()
        {
            ISceneObject nearest = null;
            double nearestDistSqr = 10000000;
            double objDistantSqr = 10000000;

            foreach (ISceneObject obj in targets)
            {
                if (!(obj is Asteroid))
                    continue;

                Asteroid asteroid = obj as Asteroid;


                objDistantSqr = (asteroid.Position - baseLauncherPosition).LengthSquared;
                if (objDistantSqr < nearestDistSqr)
                {
                    nearestDistSqr = objDistantSqr;
                    nearest = obj;
                }
            }

            return nearest == null ? null : nearest as Asteroid;
        }

        private void GetTargets()
        {
            targets.Clear();

            foreach (ISceneObject obj in objects)
            {
                if (!(obj is Asteroid))
                    continue;

                Asteroid asteroid = obj as Asteroid;

               
                if (((asteroid.Position - baseLauncherPosition).LengthSquared > (asteroid.Position + asteroid.Direction - baseLauncherPosition).LengthSquared) && asteroid.Direction.Y > 0) {

                    targets.Add(asteroid);
                
                
                }

                //if ((asteroid.Position.X - baseLauncherPosition.X > asteroid.Position.X + asteroid.Direction.X - baseLauncherPosition.X) && asteroid.Direction.Y > 0 && !oncatche.Contains(asteroid))
                //{

                //    targets.Add(asteroid);
                
                //}

                    
                
            }

            
        }


        private Asteroid GetNearestAsteroid(Asteroid hooked)
        {
            ISceneObject nearest = null;
            double nearestDistSqr = 10000000;
            double objDistantSqr = 10000000;

            foreach (ISceneObject obj in objects)
            {
                if (!(obj is Asteroid))
                    continue;

                objDistantSqr = (obj.Position - baseLauncherPosition).LengthSquared;
                if (objDistantSqr < nearestDistSqr && obj != hooked)
                {
                    nearestDistSqr = objDistantSqr;
                    nearest = obj;
                }
            }

            return nearest == null ? null : nearest as Asteroid;
        }


        private void CheckHeal()
        {
            if (me.Data.BaseIntegrity <= SharedDef.BASE_MAX_INGERITY - ((SharedDef.BASE_MAX_INGERITY * SharedDef.HEAL_AMOUNT) + SharedDef.BONUS_HEAL) &&
                me.Data.Gold >= me.HealingKit.Cost)
            {
                me.Data.Gold -= me.HealingKit.Cost;
                me.HealingKit.Heal();
            }
        }

        private void CheckUpgrade()
        {

            if (me.Canoon.UpgradeLevel != UpgradeLevel.LEVEL2 && me.Data.Gold >= me.HealingKit.Cost + me.Canoon.Cost)
            {
                me.Data.Gold -= me.Canoon.Cost;

                me.Canoon = (me.Canoon.NextSpecialAction() as WeaponUpgrade).GetWeapon();

            }

            if (me.Hook.UpgradeLevel != UpgradeLevel.LEVEL2 && me.Data.Gold >= me.HealingKit.Cost + me.Hook.Cost)
            {
                me.Data.Gold -= me.Hook.Cost;

                me.Hook = (me.Hook.NextSpecialAction() as WeaponUpgrade).GetWeapon();

            }


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
            Asteroid nearest = GetNearestTarget();

           


            if (nearest == null)
                return;

            Vector contactPoint1 = AIUtils.ComputeDestinationPositionToHitTarget(nearest, me.Data.BulletSpeed, baseLauncherPosition, me.SceneMgr.GetRandomGenerator());

            // nestrili, pokud tam nedosahne
            if ((baseLauncherPosition - contactPoint1).Length > me.Data.HookLenght)
                return;

            // nestrili pod bazi
            if (me.GetBaseLocation().Y < contactPoint1.Y)
                return;

            // nestrili, pokud to je mimo scenu
            if (SceneMgr.IsPointInViewPort(contactPoint1.ToPoint()))
                me.Hook.Shoot(contactPoint1.ToPoint());

            me.Canoon.Shoot(contactPoint1.ToPoint());
            targets.Remove(nearest);
        }
    }
}