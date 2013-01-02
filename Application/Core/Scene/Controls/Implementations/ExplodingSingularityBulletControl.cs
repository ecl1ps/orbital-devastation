using System;
using Orbit.Core.Scene.Entities;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using Lidgren.Network;
using System.Windows.Media;
using System.Windows.Threading;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Players;
using Orbit.Core.Client;
using System.Windows.Shapes;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.Controls.Collisions;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class ExplodingSingularityBulletControl : SingularityBulletCollisionReactionControl
    {
        public float Strength { get; set; }
        public float Speed { get; set; }
        private float lifeTime;
        private float explodeTime = 0;
        private SingularityExplodingBullet meBullet;
        private IList<long> hitObjects;
        private bool hitSomething;

        private void Grow(float tpf)
        {
            if (explodeTime < SharedDef.BULLET_EXPLODE_DURATION)
            {
                meBullet.Radius += (int)(Speed * tpf * 100);
                explodeTime += tpf;
            }
            else
                meBullet.DoRemoveMe();
        }

        protected override void InitControl(ISceneObject me)
        {
            base.InitControl(me);
            if (!(me is SingularityExplodingBullet))
                throw new InvalidOperationException("Cannot attach SingularityControl to non SingularityExploadingBullet object");

            hitObjects = new List<long>();
            meBullet = me as SingularityExplodingBullet;
            lifeTime = 0;
            hitSomething = false;
        }

        protected override void UpdateControl(float tpf)
        {
            if (!hitSomething)
            {
                lifeTime += tpf;

                if (lifeTime >= SharedDef.BULLET_LIFE_TIME)
                {
                    meBullet.SpawnSmallBullets();
                    meBullet.DoRemoveMe();
                }
            }
            else
                Grow(tpf);
        }

        public void StartDetonation()
        {
            // nevybuchne vickrat
            if (hitSomething)
                return;

            hitSomething = true;
            me.GetControlOfType<LinearMovementControl>().Enabled = false;

            me.GetGeometry().Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                //(meBullet.GetGeometry() as Path).Fill = new RadialGradientBrush(meBullet.Color, Colors.Black);
            }));
        }

        public override void DoCollideWith(ISceneObject other, float tpf)
        {
            if (!(other is Asteroid))
                return;

            StartDetonation();

            if (meBullet.SceneMgr.GameType != Gametype.SOLO_GAME && !meBullet.Owner.IsCurrentPlayer())
                return;

            if (hitObjects.Contains(other.Id))
                return;

            if (meBullet.Owner.IsCurrentPlayer())
                me.SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.MINE_HIT, meBullet.Center, FloatingTextManager.TIME_LENGTH_1,
                    FloatingTextType.SCORE);

            if (meBullet.Owner.IsCurrentPlayerOrBot())
                meBullet.Owner.AddScoreAndShow(ScoreDefines.MINE_HIT);

            hitObjects.Add(other.Id);

            if (!(other is UnstableAsteroid))
            {
                float speed = 0;
                IMovementControl control = other.GetControlOfType<IMovementControl>();

                if (control != null)
                {
                    Vector newDir = (other as Sphere).Center - me.Position;
                    newDir.Normalize();
                    newDir *= Strength;
                    newDir = newDir + ((other as IMovable).Direction * control.Speed);

                    speed = (float)newDir.Length;
                    control.Speed = speed;
                    newDir.Normalize();
                    (other as IMovable).Direction = newDir;
                }

                NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
                msg.Write((int)PacketType.SINGULARITY_MINE_HIT);
                msg.Write(me.Id);
                msg.Write(other.Id);
                msg.Write(other.Position);
                msg.Write((other as IMovable).Direction);
                msg.Write(speed);
                me.SceneMgr.SendMessage(msg);
            }

            if (other is IDestroyable)
            {
                if (!(me as SingularityBullet).Owner.IsCurrentPlayerOrBot())
                    return;

                HitAsteroid(other as IDestroyable);
            }
        }
    }
}
