using System;
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
using System.Windows;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class SingularityBullet : Sphere, ISendable, IDamageable
    {

        public Player Owner { get; set; } // neposilan
        public int Damage { get; set; }

        private bool hit;

        public SingularityBullet(SceneMgr mgr) : base(mgr)
        {
            hit = false;
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

        public void HitAsteroid(IDestroyable asteroid)
        {
            if (hit)
                return;
            else
                hit = true;

            if (Owner.IsCurrentPlayer())
            {
                Owner.AddScoreAndShow(ScoreDefines.CANNON_HIT);
                SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.CANNON_HIT, Center, FloatingTextManager.TIME_LENGTH_1, 
                    FloatingTextType.SCORE);

                if (asteroid is UnstableAsteroid)
                {
                    Rect opponentLoc = PlayerBaseLocation.GetBaseLocation(Owner.GetPosition() == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT);
                    double xMin = opponentLoc.X;
                    double xMax = opponentLoc.X + opponentLoc.Width;

                    if (asteroid.Position.Y > SharedDef.VIEW_PORT_SIZE.Height * 0.4 &&
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
