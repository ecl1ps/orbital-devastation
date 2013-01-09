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
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.Controls.Collisions;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class ExplodingSingularityBulletControl : SingularityBulletCollisionReactionControl
    {
        public float Strength { get; set; }
        public float Speed { get; set; }
        private float explodeTime = 0;
        private SingularityExplodingBullet meBullet;
        private IList<long> hitObjects;
        private bool hitSomething;

        private enum Events
        {
            DEATH_AND_BULLET_SPAWN
        }

        protected override void InitControl(ISceneObject me)
        {
            base.InitControl(me);
            if (!(me is SingularityExplodingBullet))
                throw new InvalidOperationException("Cannot attach SingularityControl to non SingularityExploadingBullet object");

            hitObjects = new List<long>();
            meBullet = me as SingularityExplodingBullet;
            hitSomething = false;

            events.AddEvent((int)Events.DEATH_AND_BULLET_SPAWN, new Event(SharedDef.BULLET_LIFE_TIME, EventType.ONE_TIME, new Action(() => { DieAndSpawnBullets(); })));
        }

        protected override void UpdateControl(float tpf)
        {
            if (hitSomething)
                Grow(tpf);
        }

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

        private void DieAndSpawnBullets()
        {
            if (me.SceneMgr.GetCurrentPlayer().IsActivePlayer())
                meBullet.SpawnSmallBullets();
            meBullet.DoRemoveMe();
        }

        public void StartDetonation()
        {
            // nevybuchne vickrat
            if (hitSomething)
                return;

            events.RemoveEvent((int)Events.DEATH_AND_BULLET_SPAWN);

            hitSomething = true;
            me.GetControlOfType<LinearMovementControl>().Enabled = false;

            me.GetGeometry().Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                (me.GetGeometry().Children[0] as GeometryDrawing).Brush = new RadialGradientBrush(meBullet.Color, Colors.Black);
                (me.GetGeometry().Children[0] as GeometryDrawing).Pen = new Pen(Brushes.Black, 2);
            }));
        }

        public override void DoCollideWith(ISceneObject other, float tpf)
        {
            if (!CanCollideWithObject(other))
                return;

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
                if (other.GetControlOfType<ModuleDamageControl>() != null && !other.GetControlOfType<ModuleDamageControl>().Vulnerable)
                    return;

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

                if (other.GetControlOfType<ModuleDamageControl>() != null && !other.GetControlOfType<ModuleDamageControl>().Vulnerable)
                    return;

                HitAsteroid(other as IDestroyable);
                me.SceneMgr.StatisticsMgr.BulletHit++;
            }

            StartDetonation();
        }
    }
}
