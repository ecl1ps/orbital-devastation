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
using System.Collections.Generic;
using Orbit.Core.SpecialActions;
using Orbit.Core.SpecialActions.Gamer;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.SpecialActions.Spectator;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.Players
{
    public class Player : IGameState
    {
        public SceneMgr SceneMgr { get; set; }
        public PlayerData Data { get; set; }
        public NetConnection Connection { get; set; }
        public Base Baze  { get; set; }
        public MiningModule Device { get; set; }

        private IWeapon hook;
        public IWeapon Hook {
            get
            {
                return hook;
            }
            set
            {
                value.TriggerUpgrade(hook);
                hook = value;
            }
        }
        private IWeapon mine;
        public IWeapon Mine {
            get
            {
                return mine;
            }
            set
            {
                value.TriggerUpgrade(mine);
                mine = value;
            }
        }
        private IWeapon canoon;
        public IWeapon Canoon {
            get
            {
                return canoon;
            }
            set
            {
                value.TriggerUpgrade(canoon);
                canoon = value;
            }
        }

        public bool Shooting { get; set; }
        public Point TargetPoint { get; set; }

        public IHealingKit HealingKit { get; set; }

        private float scoreUpdateTimer = 0;
        private int lastScoreValue = 0;
        private int lastGoldValue = 0;

        private bool informedProtecting = false;
        private bool informedLowHealth = false;

        private List<ISpecialAction> actions = null;

        public Player(SceneMgr mgr)
        {
            SceneMgr = mgr;
            Shooting = false;
        }

        public void SetGoldAndShow(int gold)
        {
            Data.Gold = gold;

            ShowGold();
        }

        public void AddGoldAndShow(int gold)
        {
            if (gold > 0)
                AddScoreAndShow((int)(gold * ScoreDefines.GOLD_TAKEN));

            Data.Gold += gold;

            ShowGold();
        }

        public void ShowGold()
        {
            if (Data.Gold < 0)
                return;

            if (IsCurrentPlayer() && IsActivePlayer())
                SceneMgr.ShowStatusText(4, "Gold: " + Data.Gold);
        }

        public void AddScoreAndShow(int score)
        {
            Data.Score += score;

            if (Data.Score <= 0)
                return;

            if (IsCurrentPlayer() && IsActivePlayer())
                SceneMgr.ShowStatusText(5, "Score: " + Data.Score);
        }

        public void CreateWeapons()
        {
            Hook = new HookLauncher(SceneMgr, this);
            Mine = new MineLauncher(SceneMgr, this);
            Canoon = new ProximityCannon(SceneMgr, this);
            HealingKit = new HealingKit(SceneMgr, this);
        }

        public int GetBaseIntegrity()
        {
            return Data.BaseIntegrity;
        }

        public float GetBaseIntegrityPct()
        {
            return (float)Data.BaseIntegrity / Data.MaxBaseIntegrity;
        }

        public void ChangeBaseIntegrity(int amount, bool showHeal = false)
        {
            SetBaseIntegrity(Data.BaseIntegrity + amount, showHeal);
        }

        public void SetBaseIntegrity(int amount, bool showHeal = false)
        {
            if (amount > Data.MaxBaseIntegrity)
                amount = Data.MaxBaseIntegrity;

            int diff = amount - Data.BaseIntegrity;

            if (amount < 0)
                amount = 0;

            Data.BaseIntegrity = amount;
            if (Baze != null)
                Baze.OnIntegrityChange();

            if (SceneMgr.GetCanvas() == null)
                return;

            if (showHeal)
            {
                Vector textPos = new Vector(GetBaseLocation().X + (GetBaseLocation().Width / 2), GetBaseLocation().Y - 20);
                SceneMgr.FloatingTextMgr.AddFloatingText("+ " + diff, textPos, FloatingTextManager.TIME_LENGTH_3,
                    FloatingTextType.HEAL, FloatingTextManager.SIZE_BIGGER, true);
            }
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

        public void ShowProtecting()
        {
            Player p = SceneMgr.GetPlayer(Data.FriendlyPlayerId);
            String name = "noone";
            if (p != null)
                name = p.Data.Name;

            SceneMgr.AlertMessageMgr.Show("You are protecting: " + name, 3);
        }

        public void Update(float tpf)
        {
            if (!informedProtecting && !IsActivePlayer())
            {
                informedProtecting = true;
                ShowProtecting();
            }
            

            // zatim ne pro spectatory
            if (!IsActivePlayer())
                return;

            if (!IsActivePlayer())

            scoreUpdateTimer += tpf;
            if (scoreUpdateTimer > 0.3)
            {
                if (lastScoreValue != Data.Score || lastGoldValue != Data.Gold)
                {
                    lastScoreValue = Data.Score;
                    lastGoldValue = (int) Data.Gold;
                    SendScoreAndGoldUpdate();
                }
                scoreUpdateTimer = 0;
            }

            if (!informedLowHealth && Data.BaseIntegrity <= Data.MaxBaseIntegrity * 0.25)
            {
                informedLowHealth = true;
                SceneMgr.AlertMessageMgr.Show("WARNING! LOW BASE INTEGRITY!", AlertMessageManager.TIME_NORMAL);
                SoundManager.Instance.StartPlayingOnce(SharedDef.MUSIC_ALERT);
            }

            if (informedLowHealth && Data.BaseIntegrity > Data.MaxBaseIntegrity * 0.25)
                informedLowHealth = false;
        }

        private void SendScoreAndGoldUpdate()
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_SCORE_AND_GOLD);
            msg.Write(GetId());
            msg.Write(Data.Score);
            msg.Write(Data.Gold);
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
            string value = name + DateTime.Now;
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(value);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }

        public bool IsOnlineOrBot()
        {
            return (Data.PlayerType == PlayerType.BOT) || (Connection != null && (Connection.Status == NetConnectionStatus.Connected || Connection.Status == NetConnectionStatus.RespondedAwaitingApproval));
        }

        public Rect GetBaseLocation()
        {
            return PlayerBaseLocation.GetBaseLocation(Data.PlayerPosition);
        }

        public bool IsBot()
        {
            return Data.PlayerType == PlayerType.BOT;
        }

        public bool IsCurrentPlayerOrBot()
        {
            return GetId() == SceneMgr.GetCurrentPlayer().GetId() || Data.PlayerType == PlayerType.BOT;
        }

        public List<ISpecialAction> GetActions<T>(SceneMgr mgr)
        {
            if (actions == null)
                InitActions(mgr);

            List<ISpecialAction> temp = new List<ISpecialAction>();
            foreach (ISpecialAction action in actions)
            {
                if (typeof(T).IsAssignableFrom(action.GetType()))
                    temp.Add((ISpecialAction) action);
            }

            return temp;
        }

        public List<T> GetActionsTyped<T>(SceneMgr mgr)
        {
            if (actions == null)
                InitActions(mgr);

            List<T> temp = new List<T>();
            foreach (ISpecialAction action in actions)
            {
                if (typeof(T).IsAssignableFrom(action.GetType()))
                    temp.Add((T)action);
            }

            return temp;
        }

        public void ClearActions()
        {
            actions = null;
        }

        private void InitActions(SceneMgr mgr)
        {
            if (IsActivePlayer())
                actions = GeneratePlayerActions(mgr);
            else
                actions = GenerateSpectatorActions(mgr);
        }

        private List<ISpecialAction> GeneratePlayerActions(SceneMgr mgr)
        {
            actions = new List<ISpecialAction>();
            actions.Add(new HealAction(HealingKit, mgr, this));
            actions.Add(new WeaponUpgrade(Hook));
            actions.Add(new WeaponUpgrade(Mine));
            actions.Add(new WeaponUpgrade(Canoon));

            return actions;
        }

        private List<ISpecialAction> GenerateSpectatorActions(SceneMgr mgr)
        {
            actions = new List<ISpecialAction>();

            actions.Add(new AsteroidThrow(mgr, this));
            actions.Add(new AsteroidDamage(mgr, this));
            actions.Add(new AsteroidGrowth(mgr, this));
            actions.Add(new AsteroidSlow(mgr, this));
            actions.Add(new StaticField(mgr, this));

            return actions;
        }

        public static Color GetChosenColor()
        {
            string col = GameProperties.Props.Get(PropertyKey.CHOSEN_COLOR);
            try
            {
                return (Color)ColorConverter.ConvertFromString(col);
            }
            catch (Exception /*e*/) {}

            return Colors.Pink;
        }
    }

    public enum PlayerPosition
    {
        LEFT,
        RIGHT,
        INVALID
    }

    public enum PlayerType
    {
        HUMAN,
        BOT
    }

    public enum BotType
    {
        NONE,
        LEVEL1,
        LEVEL2,
        LEVEL3,
        LEVEL4,
        LEVEL5
    }
}
