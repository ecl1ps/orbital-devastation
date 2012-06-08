using System;
using Orbit.Core.Scene.Entities;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using Lidgren.Network;
using System.Windows.Media;
using System.Windows.Threading;

namespace Orbit.Core.Scene.Controls
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
                meMine.DoRemoveMe();
                return;
            }

            Grow(tpf);                
        }

        public void CollidedWith(IMovable movable)
        {
            StartDetonation();

            if (meMine.SceneMgr.GameType != Gametype.SOLO_GAME && 
                meMine.Owner.GetPosition() == meMine.SceneMgr.GetOtherPlayer().GetPosition())
                return;

            if (hitObjects.Contains((movable as ISceneObject).Id))
                return;

            hitObjects.Add((movable as ISceneObject).Id);

            if (!(movable is Sphere))
                throw new Exception("Dropping singularity mine collided with other object than Sphere");

            Vector newDir = (movable as Sphere).Center - me.Position;
            newDir.Normalize();
            newDir *= Strength;
            movable.Direction += newDir;

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

        public void StartDetonation()
        {
            // nevybuchne vickrat
            if (hitSomething)
                return;

            hitSomething = true;

            meMine.GetGeometry().Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                meMine.FillBrush = new RadialGradientBrush(Colors.Black, Color.FromRgb(0x66, 0x00, 0x80));
            }));
        }

        private void AdjustColorsDueToDistance()
        {
            byte red = (byte)(255 * GetDistFromStartPct());

            meMine.GetGeometry().Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                meMine.BorderBrush = new SolidColorBrush(Color.FromRgb(red, 0x00, red));
                //meMine.FillBrush = new RadialGradientBrush(Colors.Black, Color.Add(Color.FromRgb(red, (byte)0, (byte)0), Color.FromRgb(0x66, 0x00, 0x80)));
            }));
        }

        private float GetDistFromStartPct()
        {
            return (float)(meMine.Center.Y / (meMine.SceneMgr.ViewPortSizeOriginal.Height * 0.5));
        }

        private float GetDistToExplosionPct()
        {
            return -1 * (GetDistFromStartPct() - 1);
        }
    }
}
