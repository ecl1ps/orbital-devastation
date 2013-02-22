using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Controls.Collisions;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Client.GameStates;
using System.Windows;
using Lidgren.Network;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class SingularityBulletCollisionReactionControl : Control, ICollisionReactionControl
    {
        protected bool statReported = false;
        public bool StatReported { get { return statReported; } set { statReported = value; } }

        private bool hit = false;
        private SingularityBullet bullet;

        protected override void InitControl(ISceneObject me)
        {
            bullet = me as SingularityBullet;
        }

        protected bool CanCollideWithObject(ISceneObject other)
        {
            if (!(other is Asteroid) && !(other is MiningModule))
                return false;

            // vypnuti friendly fire
            if (other is MiningModule && (other as MiningModule).Owner.IsFriendOf(bullet.Owner))
                return false;

            // nebude hitovat asteroidy, ktere jsou tazeny hookem, ale zaroven to zamezi hitovani vsech disabled objektu
            if (!other.Enabled)
                return false;

            return true;
        }

        public virtual void DoCollideWith(ISceneObject other, float tpf)
        {
            if (!CanCollideWithObject(other))
                return;

            if (other is Asteroid)
            {
                bullet.DoRemoveMe();
                if (!bullet.Owner.IsCurrentPlayerOrBot())
                    return;

                HitAsteroid(other as IDestroyable);
                addHitStat();
            }
            else if (other is MiningModule)
            {
                ModuleDamageControl control = other.GetControlOfType<ModuleDamageControl>();
                if (!control.Vulnerable)
                    return;

                (other as IDestroyable).TakeDamage(bullet.Damage, bullet);
                bullet.DoRemoveMe();
                addHitStat();

            }
            else if (other is IDestroyable)
            {
                bullet.DoRemoveMe();
                (other as IDestroyable).TakeDamage(bullet.Damage, bullet);
                bullet.Owner.AddScoreAndShow(ScoreDefines.CANNON_HIT);
                addHitStat();
            }
        }

        protected void addHitStat()
        {
            if (!StatReported)
            {
                (me as IProjectile).Owner.Statistics.BulletHit++;
                StatReported = true;
            }
        }

        public virtual void HitAsteroid(IDestroyable asteroid)
        {
            if (hit)
                return;
            else
                hit = true;

            if (bullet.Owner.IsCurrentPlayerOrBot())
            {
                bullet.Owner.AddScoreAndShow(ScoreDefines.CANNON_HIT);
                if (bullet.Owner.IsCurrentPlayer())
                    bullet.SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.CANNON_HIT, bullet.Center, FloatingTextManager.TIME_LENGTH_1,
                        FloatingTextType.SCORE);

                if (asteroid is UnstableAsteroid)
                {
                    Rect opponentLoc = PlayerBaseLocation.GetBaseLocation(bullet.Owner.GetPosition() == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT);
                    double xMin = opponentLoc.X;
                    double xMax = opponentLoc.X + opponentLoc.Width;

                    if (asteroid.Position.Y > SharedDef.VIEW_PORT_SIZE.Height * 0.4 &&
                        asteroid.Position.X >= xMin && asteroid.Position.X <= xMax)
                    {
                        if (bullet.Owner.IsCurrentPlayer())
                            bullet.SceneMgr.FloatingTextMgr.AddFloatingText(Strings.ft_score_cannon_unstable_above_enemy, bullet.Center,
                                FloatingTextManager.TIME_LENGTH_4, FloatingTextType.BONUS_SCORE, FloatingTextManager.SIZE_BIG, false, true);
                        bullet.Owner.AddScoreAndShow(ScoreDefines.CANNON_DESTROYED_UNSTABLE_ASTEROID_ABOVE_ENEMY);
                    }
                }
            }

            asteroid.TakeDamage(bullet.Damage, bullet);

            NetOutgoingMessage msg = bullet.SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.BULLET_HIT);
            msg.Write(bullet.Id);
            msg.Write(asteroid.Id);
            msg.Write(bullet.Damage);
            bullet.SceneMgr.SendMessage(msg);
        }
    }
}
