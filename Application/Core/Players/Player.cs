using System;
using Orbit.Core.Scene.Entities;
using System.Windows.Media;
using System.Diagnostics;
using Lidgren.Network;
using Orbit.Core.Weapons;
using Orbit.Core.Utils;
using System.Windows;
using Orbit.Core.Scene;
using System.Windows.Controls;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Orbit.Core.Players
{
    public class Player : IGameState
    {
        public SceneMgr SceneMgr { get; set; }
        public PlayerData Data { get; set; }
        public NetConnection Connection { get; set; }
        public Base Baze  { get; set; }

        public IWeapon Hook { get; set; }
        public IWeapon Mine { get; set; }
        public IWeapon Canoon { get; set; }

        public bool Shooting { get; set; }
        public Point TargetPoint { get; set; }

        public IHealingKit HealingKit { get; set; }

        private float scoreUpdateTimer = 0;
        private int lastScoreValue = 0;

        public Vector VectorPosition
        {
            get
            {
                Vector vector = new Vector(SceneMgr.ViewPortSizeOriginal.Width, SceneMgr.ViewPortSizeOriginal.Height * 0.85);

                switch (Data.PlayerPosition)
                {
                    case PlayerPosition.LEFT:
                        vector.X *= 0.1;
                        break;
                    case PlayerPosition.RIGHT:
                        vector.X *= 0.6;
                        break;
                    default:
                        return new Vector();
                }

                return vector;
            }
        }

        public Player(SceneMgr mgr)
        {
            SceneMgr = mgr;
            Shooting = false;
            HealingKit = new HealingKit(SceneMgr, this);
        }

        public void SetGoldAndShow(int gold)
        {
            Data.Gold = gold;

            ShowGold();
        }

        public void AddGoldAndShow(int gold)
        {
            AddScoreAndShow((int)(gold * ScoreDefines.GOLD_TAKEN));

            Data.Gold += gold;

            ShowGold();
        }

        public void ShowGold()
        {
            if (Data.Gold <= 0)
                return;

            if (IsCurrentPlayer())
                SceneMgr.ShowStatusText(4, "Gold: " + Data.Gold);
        }

        public void AddScoreAndShow(int score)
        {
            Data.Score += score;

            if (Data.Score <= 0)
                return;

            if (IsCurrentPlayer())
                SceneMgr.ShowStatusText(5, "Score: " + Data.Score);
        }

        public void CreateWeapons()
        {
            Hook = new HookLauncher(SceneMgr, this);
            Mine = new MineLauncher(SceneMgr, this);
            Canoon = new ProximityCannon(SceneMgr, this);
        }

        public int GetBaseIntegrity()
        {
            return Data.BaseIntegrity;
        }

        public void ChangeBaseIntegrity(int amount)
        {
            SetBaseIntegrity(Data.BaseIntegrity + amount);
        }

        public void SetBaseIntegrity(int amount)
        {
            // tohle je mozna trochu sporne - hrac bude dostavat body i kdyz se protihrac bude poskozovat sam
            int diff = Data.BaseIntegrity - amount;
            if (diff < 0 && SceneMgr.GetCurrentPlayer().GetId() != GetId())
                SceneMgr.GetCurrentPlayer().AddScoreAndShow((-diff) * ScoreDefines.DAMAGE_DEALT);

            Data.BaseIntegrity = amount;

            if (SceneMgr.GetCanvas() == null)
                return;

            SceneMgr.BeginInvoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(SceneMgr.GetCanvas(),
                    Data.PlayerPosition == PlayerPosition.LEFT ? "lblIntegrityLeft" : "lblIntegrityRight");
                if (lbl != null)
                    lbl.Content = (float)Data.BaseIntegrity / (float)SharedDef.BASE_MAX_INGERITY * 100.0f + "%";
            }));
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
            // zatim ne pro spectatory
            if (!IsActivePlayer())
                return;

            Canoon.UpdateTimer(tpf);
            Mine.UpdateTimer(tpf);

            if (Shooting)
                Canoon.Shoot(TargetPoint);

            scoreUpdateTimer += tpf;
            if (scoreUpdateTimer > 1)
            {
                if (lastScoreValue != Data.Score)
                {
                    lastScoreValue = Data.Score;
                    SendScoreUpdate();
                }
                scoreUpdateTimer = 0;
            }
        }

        private void SendScoreUpdate()
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_SCORE);
            msg.Write(GetId());
            msg.Write(Data.Score);
            SceneMgr.SendMessage(msg);
        }

        public int GetId()
        {
            return Data.Id;
        }

        public bool IsActivePlayer()
        {
            return Data.Active;
        }

        public bool IsCurrentPlayer()
        {
            return GetId() == SceneMgr.GetCurrentPlayer().GetId();
        }

        public static string GenerateNewHashId(string name)
        {
            //string value = name + Environment.CurrentDirectory + Environment.TickCount;
            string value = name + Environment.CurrentDirectory;
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(value);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }
    }
}
