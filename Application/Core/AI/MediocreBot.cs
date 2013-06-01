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
        private SceneMgr sceneMgr;
        private List<ISceneObject> objects;
        private Queue<Target> targets;
        private List<HookTarget> hookslaunched;
        
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
            targets = new Queue<Target>();
            hookslaunched = new List<HookTarget>();
        }

        public void Update(float tpf)
        {
            
            for (int i = hookslaunched.Count-1; i >= 0; i--) {
                if (hookslaunched.ElementAt(i).hook.Returning) hookslaunched.RemoveAt(i);
            
            }


         // List<Target> targetList = GetTargets().OrderBy(Target => Target.Priority).ToList();
          //  if(targetList.Count>0)
            targets = new Queue<Target>(GetTargets().OrderBy(Target => Target.Priority).ToList());
           

           if (me.Hook.IsReady())
               ShootHook();

               
            
            
           

            if (me.Canoon.IsReady())
                CannonShoot();
          
            if (me.Mine.IsReady())
                MineDrop();



            
            





            CheckHeal();
           CheckUpgrade();

           
        }

        private void ShootHook()
        {
            ICatchable target;


            if (targets.Count > 0)
            {

                target = targets.Dequeue().Asteroid;
               
            }
            else
            {

                target = GetHookTarget();

            }

            if (target == null)
                return;

            Vector contactPoint1 = AIUtils.ComputeDestinationPositionToHitTarget(target, me.Data.HookSpeed, baseLauncherPosition, me.SceneMgr.GetRandomGenerator());

            // nestrili, pokud tam nedosahne
            if ((baseLauncherPosition - contactPoint1).Length > me.Data.HookLenght - 10)
            {
              //  targets.Remove(nearest);
                return;
            }
            // nestrili pod bazi
            if (me.GetBaseLocation().Y < contactPoint1.Y)
            {
             //   targets.Remove(nearest);
                return;
            }

            // nestrili, pokud to je mimo scenu
            if (SceneMgr.IsPointInViewPort(contactPoint1.ToPoint()))
            {
               ISceneObject pr= me.Hook.Shoot(contactPoint1.ToPoint());
             
                hookslaunched.Add(new HookTarget(pr.GetControlOfType<HookControl>(),target.Id));
                
               // oncatche.Add(nearest);
               // if (targets.Contains(nearest)) targets.Remove(nearest);
            }

        }

        
        private ICatchable GetHookTarget()
        {
            ICatchable nearest = null;
            double nearestDistSqr = 100000000;
            double objDistantSqr = 100000000;

            foreach (ISceneObject obj in objects)
            {
                if (obj is StatPowerUp || (obj is Asteroid && (obj as Asteroid).AsteroidType==AsteroidType.GOLDEN))
                {

                  


                  objDistantSqr = (obj.Position - baseLauncherPosition).LengthSquared;
                 
                    if (objDistantSqr < nearestDistSqr)
                  {
                       nearestDistSqr = objDistantSqr;
                       nearest = obj as ICatchable;
                    }

                }
                

            }
            
            return nearest;
        }



      

        private List<Target> GetTargets()
        {
            List<Target> targets = new List<Target>();

            foreach (ISceneObject obj in objects)
            {
                if (!(obj is Asteroid))
                    continue;
                Boolean onhook =false;

                Asteroid asteroid = obj as Asteroid;
                    foreach (HookTarget hook in hookslaunched){
                        if (hook.target == obj.Id) onhook = true;
                    }
                    if (onhook)
                        continue;

                if (asteroid.Enabled && !asteroid.Dead)
                {
                    if (((asteroid.Position - baseLauncherPosition).LengthSquared > (asteroid.Position + asteroid.Direction - baseLauncherPosition).LengthSquared) && asteroid.Direction.Y > 0)
                    //if (((asteroid.Position - baseLauncherPosition).LengthSquared > (asteroid.Position + asteroid.Direction - baseLauncherPosition).LengthSquared) && asteroid.Direction.Y > 0)
                    {
                        Target newTarget = new Target();
                        int priority = 0;

                        newTarget.Asteroid = asteroid;

                        if ((asteroid.Position - baseLauncherPosition).Length < 400)
                        {

                            priority++;
                            newTarget.Status = asteroid.AsteroidType == AsteroidType.GOLDEN ? TargetStatus.HOOKONLY : TargetStatus.AVAILABLE;


                            if ((asteroid.Position - baseLauncherPosition).Length < 350)
                            {
                                priority++;
                                priority += (int)(asteroid.GetHp() / 10 - 1);

                                if ((asteroid.Position - baseLauncherPosition).Length < 300)
                                {
                                    priority++;
                                    priority += (int)(asteroid.GetHp() / 10 - 1);
                                    priority += asteroid.AsteroidType == AsteroidType.UNSTABLE ? 2 : 0;

                                    if ((asteroid.Position - baseLauncherPosition).Length < 200)
                                    {
                                        priority++;
                                        priority += (int)(asteroid.GetHp() / 10 - 1);
                                        priority += asteroid.AsteroidType == AsteroidType.UNSTABLE ? 1 : 0;

                                        if ((asteroid.Position - baseLauncherPosition).Length < 100)
                                        {
                                            priority++;
                                            priority += (int)(asteroid.GetHp() / 10 - 1);
                                            priority -= asteroid.AsteroidType == AsteroidType.UNSTABLE ? 4 : 0;


                                            if ((asteroid.Position - baseLauncherPosition).Length < 50)
                                            {
                                                priority++;
                                                priority += (int)(asteroid.GetHp() / 10 - 1);
                                                newTarget.Status = asteroid.AsteroidType == AsteroidType.UNSTABLE ? TargetStatus.UNAVAILABLE : TargetStatus.AVAILABLE;
                                            }
                                        }
                                    }
                                }
                            }


                          



                            if (newTarget.Status != TargetStatus.UNAVAILABLE)
                            {
                                newTarget.Priority = 100 - priority;
                                targets.Add(newTarget);

                            }

                        }

                    }//end of if (asteroid.Position - baseLauncherPosition).LengthSquared >.......

                } //end of if (asteroid.Enabled && !asteroid.Dead)

            } //end of foreach

            return targets;
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

        private void CheckUpgrade()
        {

            if (me.Hook.UpgradeLevel != UpgradeLevel.LEVEL2 && me.Data.Gold >= me.HealingKit.Cost + me.Hook.NextSpecialAction().Cost)
            {
                me.Data.Gold -= (int)me.Hook.NextSpecialAction().Cost;

                me.Hook = (me.Hook.NextSpecialAction() as WeaponUpgrade).GetWeapon();

            }

            if (me.Canoon.UpgradeLevel != UpgradeLevel.LEVEL2 && me.Data.Gold >= me.HealingKit.Cost + me.Canoon.NextSpecialAction().Cost)
            {
                me.Data.Gold -= (int)me.Canoon.NextSpecialAction().Cost;

                me.Canoon = (me.Canoon.NextSpecialAction() as WeaponUpgrade).GetWeapon();

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
            Asteroid nearest = null;
            if (targets.Count > 0)
            {
                nearest = targets.Dequeue().Asteroid;

              
            }
            else { return; }
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
            //targets.Remove(nearest);
        }
    }
}