using System;
using Orbit.Core.Scene.Entities;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using Lidgren.Network;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class SingularityControl : Control
    {
        public float Strength { get; set; }
        public float Speed { get; set; }
        private float lifeTime;
        private SingularityMine meMine;
        private IList<long> hitObjects;

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
            lifeTime = 0;
        }

        public override void UpdateControl(float tpf)
        {
            lifeTime += tpf;

            if (lifeTime >= SharedDef.MINE_INVISIBILITY_TIME && !meMine.IsVisible)
            {
                meMine.IsVisible = true;
                return;
            }

            if (lifeTime >= SharedDef.MINE_INVISIBILITY_TIME + SharedDef.MINE_LIFE_TIME && meMine.IsVisible)
            {
                meMine.IsVisible = false;
                meMine.DoRemoveMe();
                return;
            }

            if (meMine.IsVisible)
                Grow(tpf);
        }

        public void CollidedWith(IMovable movable)
        {
            if (me.SceneMgr.GameType != Gametype.SOLO_GAME && 
                meMine.Owner.GetPosition() == me.SceneMgr.GetOpponentPlayer().GetPosition())
                return;

            if (hitObjects.Contains((movable as ISceneObject).Id))
                return;

            hitObjects.Add((movable as ISceneObject).Id);

            Vector newDir = (movable as ISceneObject).Position - me.Position;
            newDir.Normalize();
            newDir *= Strength;
            movable.Direction += newDir;

            if (me.SceneMgr.GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
                msg.Write((int)PacketType.SINGULARITY_MINE_HIT);
                msg.Write((movable as ISceneObject).Id);
                msg.Write((movable as ISceneObject).Position);
                msg.Write(newDir);
                me.SceneMgr.SendMessage(msg);
            }
        }

    }
}