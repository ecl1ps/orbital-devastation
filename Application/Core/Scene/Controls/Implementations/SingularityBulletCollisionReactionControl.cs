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
        private bool hit = false;
        private SingularityBullet bullet;

        protected override void InitControl(ISceneObject me)
        {
            bullet = me as SingularityBullet;
        }

        public virtual void DoCollideWith(Entities.ISceneObject other, float tpf)
        {
            if (other is Asteroid)
            {
                if (!bullet.Owner.IsCurrentPlayerOrBot())
                    return;

                HitAsteroid(other as IDestroyable);
                bullet.DoRemoveMe();
            }
            else if (other is IDestroyable)
            {
                (other as IDestroyable).TakeDamage(bullet.Damage, bullet);
                bullet.Owner.AddScoreAndShow(ScoreDefines.CANNON_HIT);
                bullet.DoRemoveMe();
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
                        bullet.SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.CANNON_DESTROYED_UNSTABLE_ASTEROID_ABOVE_ENEMY, bullet.Center,
                            FloatingTextManager.TIME_LENGTH_4, FloatingTextType.SCORE, FloatingTextManager.SIZE_BIG, false, true);
                        bullet.Owner.AddScoreAndShow(ScoreDefines.CANNON_DESTROYED_UNSTABLE_ASTEROID_ABOVE_ENEMY);
                    }
                }
            }

            asteroid.TakeDamage(bullet.Damage, bullet);

            if (bullet.SceneMgr.GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = bullet.SceneMgr.CreateNetMessage();
                msg.Write((int)PacketType.BULLET_HIT);
                msg.Write(bullet.Id);
                msg.Write(asteroid.Id);
                msg.Write(bullet.Damage);
                bullet.SceneMgr.SendMessage(msg);
            }
        }
    }
}
