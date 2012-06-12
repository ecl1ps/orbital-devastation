using System;
using Orbit.Core.Scene.Entities;
using System.Windows.Media;
using System.Diagnostics;
using Lidgren.Network;
using Orbit.Core.Weapons;

namespace Orbit.Core.Players
{
    public class Player
    {
        public PlayerData Data { get; set; }
        public NetConnection Connection { get; set; }
        public Base Baze  { get; set; }

        public void CreateWeapons()
        {
            Data.Hook = new HookLauncher(Data.SceneMgr, this);
            Data.Mine = new MineLauncher(Data.SceneMgr, this);
            Data.Canoon = new ProximityCannon(Data.SceneMgr, this);
        }

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

        public void Update(float tpf)
        {
            Data.Canoon.UpdateTimer(tpf);
            Data.Mine.UpdateTimer(tpf);
        }

        public long GetId()
        {
            return Data.Id;
        }

        public bool IsActivePlayer()
        {
            return Data.Active;
        }
    }
}
