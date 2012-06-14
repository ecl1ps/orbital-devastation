using System;
using Orbit.Core.Players;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using Lidgren.Network;

namespace Orbit.Core.Scene.Entities
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
                    msg.Write(value);
                    msg.Write(Owner.GetId());
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
                Integrity -= (other as Asteroid).Radius / 2;
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
