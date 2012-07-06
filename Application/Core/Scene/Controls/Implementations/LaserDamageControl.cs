using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Lidgren.Network;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class LaserDamageControl : Control
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

        private List<HitData> datas;

        public override void InitControl(ISceneObject me)
        {
            if (me is Laser)
            {
                this.me = me;
                (me as Laser).initControl(this);
            }
            else
                throw new Exception("LaserDamageControl must be attached to Laser class");

            datas = new List<HitData>();
        }

        public void HitObject(IDestroyable enemy) 
        {
            if (IsValidTarget(enemy)) 
            {
                enemy.TakeDamage(SharedDef.LASER_DMG, me);
                datas.Add(new HitData(enemy, SharedDef.LASER_DMG_INTERVAL));

                if (me.SceneMgr.GameType != Gametype.SOLO_GAME)
                {
                    NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
                    msg.Write((int)PacketType.BULLET_HIT);
                    msg.Write(me.Id);
                    msg.Write(enemy.Id);
                    msg.Write(SharedDef.LASER_DMG);
                    me.SceneMgr.SendMessage(msg);
                }
            }
        }

        private bool IsValidTarget(IDestroyable enemy)
        {
            foreach (HitData data in datas)
            {
                if (data.Obj == enemy)
                    return false;
            }

            return true;
        }

        public override void UpdateControl(float tpf)
        {
            UpdateHitData(tpf);
        }

        private void UpdateHitData(float tpf)
        {
            HitData data;
            for (int i = datas.Count - 1; i >= 0; i--)
            {
                data = datas[i];
                data.Time -= tpf;

                if (data.Time <= 0)
                    datas.RemoveAt(i);
            }
        }
    }
}
