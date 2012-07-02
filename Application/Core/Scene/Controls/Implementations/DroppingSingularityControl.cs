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

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class DroppingSingularityControl : Control
    {
        public float Strength { get; set; }
        public float Speed { get; set; }
        private float lifeTime;
        private SingularityMine meMine;
        private IList<long> hitObjects;
        private bool hitSomething;

        private void Grow(float tpf)
        {
            meMine.Radius += (int)(Speed * tpf * 100);
        }

        public override void InitControl(ISceneObject me)
        {
            if (!(me is SingularityMine))
                throw new InvalidOperationException("Cannot attach SingularityControl to non SingularityMine object");

            hitObjects = new List<long>();
            meMine = me as SingularityMine;
            lifeTime = 0;
            hitSomething = false;
        }

        public override void UpdateControl(float tpf)
        {
            AdjustColorsDueToDistance();

            // vybuch, kdyz se mina dostane moc daleko
            if (!hitSomething && GetDistToExplosionPct() <= 0)
                StartDetonation();

            if (!hitSomething)
                return;

            lifeTime += tpf;

            if (lifeTime >= SharedDef.MINE_LIFE_TIME)
            {
                if (meMine.Owner.IsCurrentPlayer() && hitObjects.Count > 2)
                {
                    me.SceneMgr.FloatingTextMgr.AddFloatingText((int)Math.Pow(hitObjects.Count, ScoreDefines.MINE_HIT_MULTIPLE_EXPONENT),
                        meMine.Center, FloatingTextManager.TIME_LENGTH_4, FloatingTextType.SCORE, FloatingTextManager.SIZE_BIG);
                    meMine.Owner.AddScoreAndShow((int)Math.Pow(hitObjects.Count, ScoreDefines.MINE_HIT_MULTIPLE_EXPONENT));
                }
                meMine.DoRemoveMe();
                return;
            }

            Grow(tpf);                
        }

        public void CollidedWith(IMovable movable)
        {
            if (!(movable is Asteroid) && !(movable is StatPowerUp))
                return;

            StartDetonation();

            if (meMine.SceneMgr.GameType != Gametype.SOLO_GAME && !meMine.Owner.IsCurrentPlayer())
                return;

            if (hitObjects.Contains((movable as ISceneObject).Id))
                return;

            if (meMine.Owner.IsCurrentPlayer())
            {
                me.SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.MINE_HIT, meMine.Center, FloatingTextManager.TIME_LENGTH_1, 
                    FloatingTextType.SCORE);
                meMine.Owner.AddScoreAndShow(ScoreDefines.MINE_HIT);
            }

            hitObjects.Add((movable as ISceneObject).Id);

            Vector newDir = new Vector();
            if (movable is Sphere)
                newDir = CollideWithSphere(movable as Sphere);
            else if (movable is Square)
                newDir = CollideWithSquare(movable as Square);

            if (meMine.SceneMgr.GameType != Gametype.SOLO_GAME)
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

        private Vector CollideWithSquare(Square square)
        {
            Vector newDir = square.Center - me.Position;
            newDir.Normalize();
            newDir *= Strength;
            (square as IMovable).Direction += newDir;

            return newDir;
        }

        private Vector CollideWithSphere(Sphere sphere)
        {
            Vector newDir = sphere.Center - me.Position;
            newDir.Normalize();
            newDir *= Strength;
            sphere.Direction += newDir;

            return newDir;
        }

        public void StartDetonation()
        {
            // nevybuchne vickrat
            if (hitSomething)
                return;

            hitSomething = true;

            double speed = meMine.Direction.Length;
            meMine.Direction = meMine.Direction.NormalizeV();
            meMine.Direction *= (speed / 4);

            meMine.GetGeometry().Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                meMine.FillBrush = new RadialGradientBrush(Colors.Black, Color.FromRgb(0x66, 0x00, 0x80));
            }));
        }

        private void AdjustColorsDueToDistance()
        {
            int r = (int)(255 * GetDistFromStartPct());
            byte red = (byte)(r > 254 ? 254 : r);

            meMine.GetGeometry().Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                meMine.BorderBrush = new SolidColorBrush(Color.FromRgb(red, 0x00, red));
                //meMine.FillBrush = new RadialGradientBrush(Colors.Black, Color.Add(Color.FromRgb(red, (byte)0, (byte)0), Color.FromRgb(0x66, 0x00, 0x80)));
            }));
        }

        private float GetDistFromStartPct()
        {
            return (float)(meMine.Center.Y / (SharedDef.VIEW_PORT_SIZE.Height * 0.5));
        }

        private float GetDistToExplosionPct()
        {
            return -1 * (GetDistFromStartPct() - 1);
        }
    }
}
