using System;
using Orbit.Core.Players;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using Lidgren.Network;
using Orbit.Core.Client;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class Base : Square
    {
        public Player Owner { get; set; }
        public PlayerPosition BasePosition { get; set; }
        public Color Color { get; set; }
        public int Integrity 
        { 
            get
            {
                return Owner.GetBaseIntegrity();
            }
            set
            {
                if (Owner.IsCurrentPlayer() || SceneMgr.GameType == Gametype.SOLO_GAME)
                {
                    Owner.SetBaseIntegrity(value);

                    NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                    msg.Write((int)PacketType.BASE_INTEGRITY_CHANGE);
                    msg.Write(Owner.GetId());
                    msg.Write(value);
                    SceneMgr.SendMessage(msg);
                }
            }
        }
        
        public Base(SceneMgr mgr) : base(mgr)
        {
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }

        public override void DoCollideWith(ICollidable other)
        {
            if (other is Asteroid)
            {
                int damage = (other as Asteroid).Radius / 2;

                // score
                Player otherPlayer = SceneMgr.GetOtherActivePlayer(Owner.GetId());
                Vector textPos = new Vector(otherPlayer.VectorPosition.X + (Size.Width / 2), otherPlayer.VectorPosition.Y - 20);
                SceneMgr.FloatingTextMgr.AddFloatingText(damage * ScoreDefines.DAMAGE_DEALT, textPos, FloatingTextManager.TIME_LENGTH_2,
                    FloatingTextType.SCORE, FloatingTextManager.SIZE_MEDIUM);

                if (otherPlayer.IsCurrentPlayer())
                    otherPlayer.AddScoreAndShow(damage * ScoreDefines.DAMAGE_DEALT);

                // damage
                SceneMgr.FloatingTextMgr.AddFloatingText(damage, (other as Asteroid).Center, 
                    FloatingTextManager.TIME_LENGTH_1, FloatingTextType.DAMAGE, FloatingTextManager.SIZE_MEDIUM);

                Integrity -= damage;
                if (Integrity < 0)
                    Integrity = 0;

                (other as Asteroid).DoRemoveMe();

            }
        }

        public override void UpdateGeometric()
        {
            
        }
    }

}
