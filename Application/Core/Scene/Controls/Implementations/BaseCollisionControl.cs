using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Players;
using Orbit.Core.Client;
using System.Windows;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.Controls.Collisions;
using Orbit.Core.Scene.Particles.Implementations;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class BaseCollisionControl : Control, ICollisionReactionControl
    {
        private Base baze;

        protected override void InitControl(ISceneObject me)
        {
            baze = me as Base;
        }
        
        public void DoCollideWith(ISceneObject other, float tpf)
        {
            if (other is Asteroid && (other as Asteroid).Enabled)
            {
                other.DoRemoveMe();

                // nebudou hitovat objekty, ktere jsou uz pod horizontem
                if (other.Center.Y > baze.Position.Y + baze.Size.Height)
                    return;

                int damage = (other as Asteroid).Radius / 2;

                // score
                Player otherPlayer = baze.SceneMgr.GetOtherActivePlayer(baze.Owner.GetId());
                if (otherPlayer != null && otherPlayer.IsCurrentPlayer())
                {
                    Vector textPos = new Vector(otherPlayer.GetBaseLocation().X + (otherPlayer.GetBaseLocation().Width / 2), otherPlayer.GetBaseLocation().Y - 20);
                    baze.SceneMgr.FloatingTextMgr.AddFloatingText(damage * ScoreDefines.DAMAGE_DEALT, textPos, FloatingTextManager.TIME_LENGTH_2,
                        FloatingTextType.SCORE, FloatingTextManager.SIZE_MEDIUM);
                }

                if (otherPlayer != null && otherPlayer.IsCurrentPlayerOrBot())
                    otherPlayer.AddScoreAndShow(damage * ScoreDefines.DAMAGE_DEALT);

                // damage
                baze.SceneMgr.FloatingTextMgr.AddFloatingText(damage, new Vector((other as Asteroid).Center.X, (other as Asteroid).Center.Y - 10),
                    FloatingTextManager.TIME_LENGTH_1, FloatingTextType.DAMAGE, FloatingTextManager.SIZE_MEDIUM);

                SoundManager.Instance.StartPlayingOnce(SharedDef.MUSIC_DAMAGE_TO_BASE);

                baze.Integrity -= damage;
                baze.Owner.Statistics.DamageTaken += damage;

                EmmitorGroup g = ParticleEmmitorFactory.CreateExplosionEmmitors(me.SceneMgr, other.Center);
                g.Attach(me.SceneMgr, false);

                if (baze.Owner.IsCurrentPlayer())
                    me.SceneMgr.ScreenShakingMgr.Start(ShakePower.WEAK);

                if (baze.Owner.IsCurrentPlayerOrBot())
                    spawnParticles(other);
            }
        }

        private void spawnParticles(ISceneObject other)
        {
            Vector direction;
            Vector collision;

            if(other is Asteroid) 
            {
                Asteroid a = other as Asteroid;
                collision = a.Center + (a.Direction * a.Radius);
            } else if (other is IMovable) 
            {
                IMovable o = other as IMovable;
                collision = o.Center;
            } else
                return;

            direction = new Vector(0, -1);

            EmmitorGroup g = ParticleEmmitorFactory.CreateBaseExplosionEmmitors(baze, collision, direction);
            g.Attach(me.SceneMgr);
        }
    }
}
