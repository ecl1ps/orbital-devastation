using System;
using Orbit.Core.Scene.Entities;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using Lidgren.Network;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Controls.Collisions;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class SingularityControl : Control, ICollisionReactionControl
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

        protected override void InitControl(ISceneObject me)
        {
            if (!(me is SingularityMine))
                throw new InvalidOperationException("Cannot attach SingularityControl to non SingularityMine object");

            hitObjects = new List<long>();
            meMine = me as SingularityMine;
            lifeTime = 0;
        }

        protected override void UpdateControl(float tpf)
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

        public void DoCollideWith(ISceneObject other, float tpf)
        {
            if (!(other is IMovable))
                return;

            if (!meMine.Owner.IsCurrentPlayerOrBot())
                return;

            if (hitObjects.Contains(other.Id))
                return;

            hitObjects.Add((other as ISceneObject).Id);

            Vector newDir = (other as ISceneObject).Position - me.Position;
            newDir.Normalize();
            newDir *= Strength;
            (other as IMovable).Direction += newDir;

            NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.SINGULARITY_MINE_HIT);
            msg.Write(other.Id);
            msg.Write(other.Position);
            msg.Write(newDir);
            me.SceneMgr.SendMessage(msg);
        }
    }
}
