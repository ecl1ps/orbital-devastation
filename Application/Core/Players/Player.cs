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
using System.Globalization;

namespace Orbit.Core.Players
{
    public class Player : IGameState
    {
        public SceneMgr SceneMgr { get; set; }
        public PlayerData Data { get; set; }
        public StatisticsManager Statistics { get; set; }
        /**
        *  Na serveru je Connection pripojeni k hracovi, na klientovi je to pripojeni k serveru                                                                    
        */
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
            Statistics = new StatisticsManager();
            Statistics.Owner = this;
        }

        public void SetGoldAndShow(int gold)
        {
            Data.Gold = gold;

            ShowGold();
        }

        public void AddGoldAndShow(int gold)
        {
            if (gold > 0)
            {
                AddScoreAndShow((int)(gold * ScoreDefines.GOLD_TAKEN));
                Statistics.GoldEarned += gold;
            }

            Data.Gold += gold;

            ShowGold();
        }

        public void ShowGold()
        {
            if (Data.Gold < 0)
                return;

            if (IsCurrentPlayer() && IsActivePlayer())
                SceneMgr.ShowStatusText(4, String.Format(Strings.Culture, Strings.ui_gold, Data.Gold));
        }

        public void AddScoreAndShow(int score)
        {
            Data.MatchPoints += score;

            if (Data.MatchPoints <= 0)
                return;

            if (IsCurrentPlayer() && IsActivePlayer())
                SceneMgr.ShowStatusText(5, String.Format(Strings.Culture, Strings.ui_match_points, Data.MatchPoints));
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
                
            if (diff > 0)
                Statistics.Healed += diff;

            if (Baze != null)
                Baze.OnIntegrityChange();

            if (SceneMgr.GetCanvas() == null)
                return;

            if (showHeal)
            {
                Vector textPos = new Vector(GetBaseLocation().X + (GetBaseLocation().Width / 2), GetBaseLocation().Y - 20);
                SceneMgr.FloatingTextMgr.AddFloatingText(String.Format(Strings.Culture, Strings.char_plus_and_val, diff), textPos, FloatingTextManager.TIME_LENGTH_3,
                    FloatingTextType.HEAL, FloatingTextManager.SIZE_BIGGER, true);
            }
        }

        public void UpdateScore(int amount)
        {
            Data.MatchPoints += amount;
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
            String name = Strings.game_player_nobody;
            if (p != null)
                name = p.Data.Name;

            SceneMgr.AlertMessageMgr.Show(String.Format(Strings.Culture, Strings.game_protecting, name), 3);
        }

        public void Update(float tpf)
        {
            Statistics.Update(tpf);
            if (!informedProtecting)
            {
                informedProtecting = true;
                if (IsActivePlayer())
                {
                    SceneMgr.AlertMessageMgr.Show(GetPosition() == PlayerPosition.LEFT ? Strings.game_protect_base_left : Strings.game_protect_base_right, 
                        AlertMessageManager.TIME_NORMAL);
                }
                else
                    ShowProtecting();
            }

            Player p = null;
            if (IsActivePlayer())
                p = this;
            else if (Data.FriendlyPlayerId != 0)
                p = SceneMgr.GetPlayer(Data.FriendlyPlayerId);

            if (p != null)
            {
                if (!informedLowHealth && HasLowHp(p))
                {
                    informedLowHealth = true;
                    SceneMgr.AlertMessageMgr.Show(IsActivePlayer() ? Strings.game_warning_integrity : Strings.game_warning_integrity_ally, AlertMessageManager.TIME_NORMAL);
                    SoundManager.Instance.StartPlayingOnce(SharedDef.MUSIC_ALERT);
                }

                if (informedLowHealth && !HasLowHp(p))
                    informedLowHealth = false;
            }

            if (!IsActivePlayer())
                return;

            scoreUpdateTimer += tpf;
            if (scoreUpdateTimer > 0.3)
            {
                if (lastScoreValue != Data.MatchPoints || lastGoldValue != Data.Gold)
                {
                    lastScoreValue = Data.MatchPoints;
                    lastGoldValue = (int) Data.Gold;
                    SendScoreAndGoldUpdate();
                }
                scoreUpdateTimer = 0;
            }
        }

        private bool HasLowHp(Player p)
        {
            return p.Data.BaseIntegrity <= p.Data.MaxBaseIntegrity * 0.25;
        }

        private void SendScoreAndGoldUpdate()
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_SCORE_AND_GOLD);
            msg.Write(GetId());
            msg.Write(Data.MatchPoints);
            msg.Write(Data.Gold);
            SceneMgr.SendMessage(msg);
        }

        public int GetId()
        {
            return Data.Id;
        }

        public static string GenerateNewHashId(string name)
        {
            string value = name + DateTime.Now;
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(value);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2", CultureInfo.InvariantCulture));

            return sb.ToString();
        }

        public Rect GetBaseLocation()
        {
            return PlayerBaseLocation.GetBaseLocation(Data.PlayerPosition);
        }

        public bool IsActivePlayer()
        {
            return Data.Active;
        }

        public bool IsCurrentPlayer()
        {
            return GetId() == SceneMgr.GetCurrentPlayer().GetId();
        }

        public bool IsBot()
        {
            return Data.PlayerType == PlayerType.BOT;
        }

        public bool IsCurrentPlayerOrBot()
        {
            return IsCurrentPlayer() || IsBot();
        }

        public bool IsFriendOf(Player p)
        {
            return Data.FriendlyPlayerId == p.GetId();
        }

        /** 
         * na klientovi bude pro ostatni hrace udavat ze jsou offline, jelikoz nebudou mit nastavene connection (vse jede pres server)
         */
        public bool IsOnlineOrBot()
        {
            return IsBot() || (Connection != null && (Connection.Status == NetConnectionStatus.Connected || Connection.Status == NetConnectionStatus.RespondedAwaitingApproval || Connection.Status == NetConnectionStatus.RespondedConnect));
        }

        public List<ISpecialAction> GetActions<T>()
        {
            if (actions == null)
                InitActions();

            List<ISpecialAction> temp = new List<ISpecialAction>();
            foreach (ISpecialAction action in actions)
            {
                if (typeof(T).IsAssignableFrom(action.GetType()))
                    temp.Add((ISpecialAction) action);
            }

            return temp;
        }

        public List<T> GetActionsTyped<T>()
        {
            if (actions == null)
                InitActions();

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

        private void InitActions()
        {
            if (IsActivePlayer())
                actions = GeneratePlayerActions();
            else
                actions = GenerateSpectatorActions();
        }

        private List<ISpecialAction> GeneratePlayerActions()
        {
            actions = new List<ISpecialAction>();
            actions.Add(new HealAction(HealingKit, SceneMgr, this));
            actions.Add(new WeaponUpgrade(Hook));
            actions.Add(new WeaponUpgrade(Mine));
            actions.Add(new WeaponUpgrade(Canoon));

            return actions;
        }

        private List<ISpecialAction> GenerateSpectatorActions()
        {
            actions = new List<ISpecialAction>();

            actions.Add(new AsteroidThrow(SceneMgr, this));
            actions.Add(new AsteroidDamage(SceneMgr, this));
            actions.Add(new AsteroidGrowth(SceneMgr, this));
            actions.Add(new AsteroidSlow(SceneMgr, this));
            actions.Add(new StaticField(SceneMgr, this));

            return actions;
        }

        public static Color GetChosenColor()
        {
            string col = GameProperties.Props.Get(PropertyKey.CHOSEN_COLOR);
            try
            {
                return (Color)ColorConverter.ConvertFromString(col);
            }
            catch (Exception) {}

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
