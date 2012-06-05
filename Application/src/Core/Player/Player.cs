using System;
using Orbit.Core.Scene.Entities;
using System.Windows.Media;
using System.Diagnostics;
using Lidgren.Network;
using Orbit.src.Core.weapons;
using Orbit.src.Core.utils;

namespace Orbit.Core.Players
{
    public class Player
    {
        public PlayerData Data { get; set; }
        public NetConnection Connection { get; set; }
        public Base Baze  { get; set; }

        public int GetBaseIntegrity()
        {
            return Baze.Integrity;
        }

        public void UpdateBaseIntegrity(int amount)
        {
            Baze.Integrity += amount;
        }

        public void UpdateScore(int amount)
        {
            Data.Score += amount;
        }

        public PlayerPosition GetPosition()
        {
            return Data.PlayerPosition;
        }

        public Color GetPlayerColor()
        {
            return Data.PlayerColor;
        }
    }
}
