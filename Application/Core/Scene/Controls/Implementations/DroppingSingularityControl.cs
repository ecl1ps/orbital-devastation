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
        public float Strength { get; set; }
        public float Speed { get; set; }
        private float lifeTime;
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
            lifeTime = 0;
            hitSomething = false;
        }

        protected override void UpdateControl(float tpf)
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
                    me.SceneMgr.FloatingTextMgr.AddFloatingText(GetMultihitMessage(hitObjects.Count),
                        meMine.Center, FloatingTextManager.TIME_LENGTH_4, FloatingTextType.SCORE, FloatingTextManager.SIZE_BIG, false, true);

                if (meMine.Owner.IsCurrentPlayerOrBot() && hitObjects.Count > 2)
                    meMine.Owner.AddScoreAndShow((int)Math.Pow(hitObjects.Count, ScoreDefines.MINE_HIT_MULTIPLE_EXPONENT));
                meMine.DoRemoveMe();
                return;
            }

            Grow(tpf);                
        }

        private string GetMultihitMessage(int count)
        {
            switch (count)
            {
                case 3:
                    return "Triple Hit";
                case 4:
                    return "Quadra Hit";
                case 5:
                    return "Penta Hit";
                default:
                    return "Multi Hit";
            }
        }

        public virtual void DoCollideWith(ISceneObject other, float tpf)
        {
            if ((!(other is Asteroid) && !(other is StatPowerUp)) || !(other is IMovable))
                return;

            StartDetonation();

            if (meMine.SceneMgr.GameType != Gametype.SOLO_GAME && !meMine.Owner.IsCurrentPlayer())
                return;

            if (hitObjects.Contains((other as ISceneObject).Id))
                return;

            if (meMine.Owner.IsCurrentPlayer())
                me.SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.MINE_HIT, meMine.Center, FloatingTextManager.TIME_LENGTH_1, 
                    FloatingTextType.SCORE);

            if (meMine.Owner.IsCurrentPlayerOrBot())
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

            meMine.GetGeometry().Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                meMine.BorderBrush = new SolidColorBrush(Color.FromRgb(red, 0x00, red));
                //meMine.FillBrush = new RadialGradientBrush(Colors.Black, Color.Add(Color.FromRgb(red, (byte)0, (byte)0), Color.FromRgb(0x66, 0x00, 0x80)));
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
