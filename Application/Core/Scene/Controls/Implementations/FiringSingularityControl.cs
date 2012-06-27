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

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class FiringSingularityControl : Control
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
        public override void InitControl(ISceneObject me)
        {
            if (!(me is SingularityExplodingBullet))
                throw new InvalidOperationException("Cannot attach SingularityControl to non SingularityExploadingBullet object");

            hitObjects = new List<long>();
            meBullet = me as SingularityExplodingBullet;
            lifeTime = 0;
            hitSomething = false;
        }

        public override void UpdateControl(float tpf)
        {
            if (!hitSomething)
            {
                lifeTime += tpf;

                if (lifeTime >= SharedDef.BULLET_LIFE_TIME)
                {
                    meBullet.SpawnMinions();
                    meBullet.DoRemoveMe();
                }
            }
            else
                Grow(tpf);
        }

        public void CollidedWith(IMovable movable)
        {
            if (!(movable is Asteroid))
                return;

            StartDetonation();

            if (meBullet.SceneMgr.GameType != Gametype.SOLO_GAME && !meBullet.Owner.IsCurrentPlayer())
                return;

            if (hitObjects.Contains((movable as ISceneObject).Id))
                return;

            if (meBullet.Owner.IsCurrentPlayer())
            {
                me.SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.MINE_HIT, meBullet.Center, FloatingTextManager.TIME_LENGTH_1,
                    FloatingTextType.SCORE);
                meBullet.Owner.AddScoreAndShow(ScoreDefines.MINE_HIT);
            }

            hitObjects.Add((movable as ISceneObject).Id);

            if (!(movable is UnstableAsteroid))
            {
                Vector newDir = (movable as Sphere).Center - me.Position;
                newDir.Normalize();
                newDir *= Strength;
                movable.Direction += newDir;

                if (meBullet.SceneMgr.GameType != Gametype.SOLO_GAME)
                {
                    NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
                    msg.Write((int)PacketType.SINGULARITY_MINE_HIT);
                    msg.Write(me.Id);
                    msg.Write((movable as ISceneObject).Id);
                    msg.Write((movable as ISceneObject).Position);
                    msg.Write(newDir);
                    me.SceneMgr.SendMessage(msg);
                }
            }

            if (movable is IDestroyable)
            {
                if (me.SceneMgr.GameType != Gametype.SOLO_GAME && !(me as SingularityBullet).Owner.IsCurrentPlayer())
                    return;

                (me as SingularityBullet).HitAsteroid(movable as IDestroyable);
            }
        }

        public void StartDetonation()
        {
            // nevybuchne vickrat
            if (hitSomething)
                return;

            hitSomething = true;
            me.RemoveControl(typeof(LinearMovementControl));

            me.GetGeometry().Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                (meBullet.GetGeometry() as Path).Fill = new RadialGradientBrush(Colors.Black, Color.Add(Color.FromRgb(0xff, 0x0, 0x0), Color.FromRgb(0x66, 0x00, 0x30)));
            }));
        }
    }
}
