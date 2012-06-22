﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using Lidgren.Network;
using Orbit.Core;
using Orbit.Core.Client;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class SingularityBullet : Sphere, ISendable
    {

        public Player Owner { get; set; } // neposilan
        public int Damage { get; set; }

        public SingularityBullet(SceneMgr mgr) : base(mgr)
        {
        }

        protected override void UpdateGeometricState()
        {
        }

        public override void DoCollideWith(ICollidable other)
        {
            if (other is IDestroyable)
            {
                if (SceneMgr.GameType != Gametype.SOLO_GAME && !Owner.IsCurrentPlayer())
                    return;

                HitAsteroid(other as IDestroyable);
                DoRemoveMe();
            }
        }

        private void HitAsteroid(IDestroyable asteroid)
        {
            if (Owner.IsCurrentPlayer())
            {
                Owner.AddScoreAndShow(ScoreDefines.CANNON_HIT);
                SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.CANNON_HIT, Center, FloatingTextManager.TIME_LENGTH_1, 
                    FloatingTextType.SCORE);

                if (asteroid is UnstableAsteroid)
                {
                    double xMin = 0, xMax = 0;
                    if (Owner.GetPosition() == PlayerPosition.RIGHT)
                    {
                        xMin = SceneMgr.ViewPortSizeOriginal.Width * 0.1;
                        xMax = SceneMgr.ViewPortSizeOriginal.Width * 0.4;
                    }
                    else
                    {
                        xMin = SceneMgr.ViewPortSizeOriginal.Width * 0.6;
                        xMax = SceneMgr.ViewPortSizeOriginal.Width * 0.9;
                    }

                    if (asteroid.Position.Y > SceneMgr.ViewPortSizeOriginal.Height * 0.4 &&
                        asteroid.Position.X >= xMin && asteroid.Position.X <= xMax)
                    {
                        SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.CANNON_DESTROYED_UNSTABLE_ASTEROID_ABOVE_ENEMY, Center, 
                            FloatingTextManager.TIME_LENGTH_4, FloatingTextType.SCORE, FloatingTextManager.SIZE_BIG);
                        Owner.AddScoreAndShow(ScoreDefines.CANNON_DESTROYED_UNSTABLE_ASTEROID_ABOVE_ENEMY);
                    }
                }
            }

            asteroid.TakeDamage(Damage, this);

            if (SceneMgr.GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                msg.Write((int)PacketType.BULLET_HIT);
                msg.Write(Id);
                msg.Write(asteroid.Id);
                msg.Write(Damage);
                SceneMgr.SendMessage(msg);
            }
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_SINGULARITY_BULLET);
            msg.WriteObjectSingularityBullet(this);
            msg.WriteControls(GetControlsCopy());
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectSingularityBullet(this);
            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}
