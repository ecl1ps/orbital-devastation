﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Lidgren.Network;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls.Collisions;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class LaserDamageControl : Control, ICollisionReactionControl
    {
        public class HitData
        {
            public IDestroyable Obj { get; set; }
            public float Time {get; set;}

            public HitData(IDestroyable obj, float time)
            {
                Obj = obj;
                Time = time;
            }
        }

        private Player owner;
        private List<HitData> dataList;

        protected override void InitControl(ISceneObject me)
        {
            if (me is Laser)
            {
                this.me = me;
                this.owner = (me as Laser).Owner;
            }
            else
                throw new Exception("LaserDamageControl must be attached to Laser class");

            dataList = new List<HitData>();
        }

        public void DoCollideWith(ISceneObject other, float tpf)
        {
            if (other is IDestroyable)
                HitObject(other as IDestroyable);
        }

        public void HitObject(IDestroyable enemy) 
        {
            if (owner != me.SceneMgr.GetCurrentPlayer())
                return;

            if (IsValidTarget(enemy)) 
            {
                enemy.TakeDamage(owner.Data.LaserDamage, me);
                dataList.Add(new HitData(enemy, owner.Data.LaserDamageInterval));

                if (me.SceneMgr.GameType != Gametype.SOLO_GAME)
                {
                    NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
                    msg.Write((int)PacketType.BULLET_HIT);
                    msg.Write(me.Id);
                    msg.Write(enemy.Id);
                    msg.Write(owner.Data.LaserDamage);
                    me.SceneMgr.SendMessage(msg);
                }
            }
        }

        private bool IsValidTarget(IDestroyable enemy)
        {
            foreach (HitData data in dataList)
            {
                if (data.Obj == enemy)
                    return false;
            }

            return true;
        }

        protected override void UpdateControl(float tpf)
        {
            UpdateHitData(tpf);
        }

        private void UpdateHitData(float tpf)
        {
            HitData data;
            for (int i = dataList.Count - 1; i >= 0; i--)
            {
                data = dataList[i];
                data.Time -= tpf;

                if (data.Time <= 0)
                    dataList.RemoveAt(i);
            }
        }
       
    }
}
