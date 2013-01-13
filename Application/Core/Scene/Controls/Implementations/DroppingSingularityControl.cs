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
    public class DroppingSingularityControl : Control, ICollisionReactionControl
    {
        public bool StatsReported { get; set; }
        public float Strength { get; set; }
        public float Speed { get; set; }
        protected SingularityMine meMine;
        private IList<long> hitObjects;
        private bool hitSomething;

        private void Grow(float tpf)
        {
            meMine.Radius += (int)(Speed * tpf * 100);
        }

        protected override void InitControl(ISceneObject me)
        {
            if (!(me is SingularityMine))
                throw new InvalidOperationException("Cannot attach SingularityControl to non SingularityMine object");

            hitObjects = new List<long>();
            meMine = me as SingularityMine;
            hitSomething = false;
        }

        protected override void UpdateControl(float tpf)
        {
            AdjustColorsDueToDistance();

            // vybuch, kdyz se mina dostane moc daleko
            if (!hitSomething && GetDistToExplosionPct() <= 0)
                StartDetonation();

            if (hitSomething)
            {
                Grow(tpf);
                if (!StatsReported && meMine.Owner.IsCurrentPlayer())
                {
                    me.SceneMgr.StatisticsMgr.MineHit++;
                    StatsReported = true;
                }
            }
        }

        private void Die()
        {
            if (meMine.Owner.IsCurrentPlayer() && hitObjects.Count > 2)
                me.SceneMgr.FloatingTextMgr.AddFloatingText(GetMultihitMessage(hitObjects.Count),
                    meMine.Center, FloatingTextManager.TIME_LENGTH_4, FloatingTextType.SCORE, FloatingTextManager.SIZE_BIG, false, true);

            if (meMine.Owner.IsCurrentPlayerOrBot() && hitObjects.Count > 2)
                meMine.Owner.AddScoreAndShow((int)Math.Pow(hitObjects.Count, ScoreDefines.MINE_HIT_MULTIPLE_EXPONENT));
            meMine.DoRemoveMe();
        }

        private string GetMultihitMessage(int count)
        {
            switch (count)
            {
                case 3:
                    return "Triple Hit!";
                case 4:
                    return "Quadra Hit!";
                case 5:
                    return "Penta Hit!";
                default:
                    return "Multi Hit!";
            }
        }

        public virtual void DoCollideWith(ISceneObject other, float tpf)
        {
            if (other is SingularityMine || !(other is IMovable))
                return;

            StartDetonation();

            if (!meMine.Owner.IsCurrentPlayerOrBot())
                return;

            if (hitObjects.Contains((other as ISceneObject).Id))
                return;

            if (meMine.Owner.IsCurrentPlayer())
                me.SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.MINE_HIT, meMine.Center, FloatingTextManager.TIME_LENGTH_1, 
                    FloatingTextType.SCORE);

            meMine.Owner.AddScoreAndShow(ScoreDefines.MINE_HIT);

            hitObjects.Add(other.Id);

            float speed = 0;
            IMovementControl control = other.GetControlOfType<IMovementControl>();

            if (control != null)
            {
                Vector newDir = other.Center - me.Position;
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

        public virtual void StartDetonation()
        {
            // nevybuchne vickrat
            if (hitSomething)
                return;

            hitSomething = true;

            events.AddEvent(1, new Event(SharedDef.MINE_LIFE_TIME, EventType.ONE_TIME, new Action(() => { Die(); })));

            SoundManager.Instance.StartPlayingOnce(SharedDef.MUSIC_EXPLOSION);

            IMovementControl c = me.GetControlOfType<IMovementControl>();
            if (c != null)
                c.Speed = c.Speed / 4;

            meMine.GetGeometry().Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                meMine.FillBrush = new RadialGradientBrush(Colors.Black, Color.FromRgb(0x66, 0x00, 0x80));
            }));
        }

        private void AdjustColorsDueToDistance()
        {
            int r = (int)(255 * GetDistFromStartPct());
            byte red = (byte)(r > 254 ? 254 : r);
            meMine.Color = Color.FromRgb(red, 0x00, red);

            meMine.GetGeometry().Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                meMine.BorderBrush = new SolidColorBrush(meMine.Color);
            }));
        }

        protected float GetDistFromStartPct()
        {
            return (float)(meMine.Center.Y / (SharedDef.VIEW_PORT_SIZE.Height * 0.5));
        }

        protected float GetDistToExplosionPct()
        {
            return -1 * (GetDistFromStartPct() - 1);
        }
    }
}
