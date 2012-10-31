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
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class SingularityBullet : Sphere, ISendable, IProjectile
    {

        public Player Owner { get; set; } // neposilan
        public int Damage { get; set; }

        private bool hit;

        public SingularityBullet(SceneMgr mgr) : base(mgr)
        {
            hit = false;
        }

        public override void DoCollideWith(ICollidable other, float tpf)
        {
            if (other is Asteroid)
            {
                if (!Owner.IsCurrentPlayerOrBot())
                    return;
               
                HitAsteroid(other as IDestroyable);
                DoRemoveMe();
            }
            else if (other is IDestroyable)
            {
                (other as IDestroyable).TakeDamage(Damage, this);
                Owner.AddScoreAndShow(ScoreDefines.CANNON_HIT);
                DoRemoveMe();
            }
        }

        public virtual void HitAsteroid(IDestroyable asteroid)
        {
            if (hit)
                return;
            else
                hit = true;

            if (Owner.IsCurrentPlayerOrBot())
            {
                Owner.AddScoreAndShow(ScoreDefines.CANNON_HIT);
                if (Owner.IsCurrentPlayer())
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
                            FloatingTextManager.TIME_LENGTH_4, FloatingTextType.SCORE, FloatingTextManager.SIZE_BIG, false, true);
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

        public virtual void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_SINGULARITY_BULLET);
            msg.WriteObjectSingularityBullet(this);
            msg.WriteControls(GetControlsCopy());
        }

        public virtual void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectSingularityBullet(this);
            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}
