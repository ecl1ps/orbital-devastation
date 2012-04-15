using System;
using Orbit.Core.Scene.Entities;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using Lidgren.Network;

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
            meMine.Radius += (int)Math.Floor(Speed * tpf * 100);
        }

        public override void InitControl(ISceneObject me)
        {
            if (!(me is SingularityMine))
                throw new InvalidOperationException("Cannot attach SingularityControl to non SingularityMine object");

            hitObjects = new List<long>();
            meMine = me as SingularityMine;
            meMine.IsVisible = true;
            lifeTime = 0;
            hitSomething = false;
        }

        public override void UpdateControl(float tpf)
        {
            if (meMine.Position.Y > meMine.SceneMgr.ViewPortSizeOriginal.Height * 0.4)
            {
                meMine.DoRemoveMe();
                return;
            }

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
            hitSomething = true;

            if (meMine.SceneMgr.GameType != Gametype.SOLO_GAME && 
                meMine.Owner.GetPosition() == meMine.SceneMgr.GetOtherPlayer().GetPosition())
                return;

            if (hitObjects.Contains((movable as ISceneObject).Id))
                return;

            hitObjects.Add((movable as ISceneObject).Id);

            Vector newDir = (movable as ISceneObject).Position - me.Position;
            newDir.Normalize();
            newDir *= Strength;
            movable.Direction += newDir;

            if (meMine.SceneMgr.GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                msg.Write((int)PacketType.SINGULARITY_MINE_HIT);
                msg.Write((movable as ISceneObject).Id);
                msg.Write((movable as ISceneObject).Position);
                msg.Write(newDir);
                SceneMgr.SendMessage(msg);
            }
        }

    }
}
