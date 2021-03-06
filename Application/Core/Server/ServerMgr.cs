using System;
using Orbit;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using Orbit.Core.Scene.Entities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Concurrent;
using Lidgren.Network;
using System.Windows.Input;
using Orbit.Core;
using Orbit.Core.Scene;
using Orbit.Core.Server.Match;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.Server
{
    public partial class ServerMgr : IUpdatable
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool isInitialized;
        private volatile bool shouldQuit;
        private List<Player> players;
        private Random randomGenerator;
        private ConcurrentQueue<Action> synchronizedQueue;
        private bool gameEnded;
        private GameManager gameSession;
        private Action savedEndGameAction;
        private List<int> playersRespondedScore;
        private StatsMgr statsMgr;

        public int Id { get; set; }
        public Gametype GameType { get; set; }
        public GameStateManager StateMgr { get; set; }
        public TournamentSettings TournamentSettings { get; set; }
        public float Time { get; set; }

        public delegate void ManagerClosed(ServerMgr mgr);
        public ManagerClosed CloseCallback { get; set; }

        public ServerMgr()
        {
            statsMgr = new StatsMgr(null);
            isInitialized = false;
            shouldQuit = false;
            Time = 0;
            Id = IdMgr.GetNewServerId();
        }

        public void Init(Gametype gameType, NetServer netServer)
        {
            GameType = gameType;
            server = netServer;
            gameEnded = false;
            isInitialized = false;
            shouldQuit = false;
            randomGenerator = new Random(Environment.TickCount);
            players = new List<Player>(2);
            synchronizedQueue = new ConcurrentQueue<Action>();
            StateMgr = new GameStateManager();
        }

        private void EndGame(Player plr, GameEnd endType)
        {
            if (gameEnded)
                return;

            gameSession.IsRunning = false;
            gameSession.GameEnded(plr, endType);

            foreach (Player p in players)
            {
                NetOutgoingMessage msg = CreateNetMessage();
                msg.Write((int)PacketType.PLAYER_SCORE_AND_GOLD);
                msg.Write(p.GetId());
                msg.Write(p.Data.MatchPoints);
                msg.Write(p.Data.Gold);
                BroadcastMessage(msg, p);
            }

            gameEnded = true;
            if (endType == GameEnd.WIN_GAME)
                PlayerWon(plr);

            if (GameType != Gametype.TOURNAMENT_GAME)
                RequestStop();
            else
                TournamentGameEnded(plr, endType);
        }

        private void TournamentGameEnded(Player plr, GameEnd endType)
        {
            isInitialized = false;

            if (gameSession.CheckTournamentFinished(true))
            {
                RequestStop();
                return;
            }

            gameEnded = false;

            players.ForEach(p => p.Data.Reset());
        }

        public void Shutdown()
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.SERVER_SHUTDOWN);
            BroadcastMessage(msg);

            RequestStop();
        }

        private void CleanUp()
        {
            shouldQuit = false;
        }

        public Player CreateAndAddPlayer(String name, String hash, Color clr)
        {
            Player plr = new Player(null);
            plr.Data = new PlayerData();
            plr.Data.Id = IdMgr.GetNewPlayerId();
            if (players.Exists(p => p.Data.Name.Equals(name)))
                plr.Data.Name = name + " " + plr.GetId();
            else
                plr.Data.Name = name;
            plr.Data.HashId = hash;
            plr.Data.PlayerColor = clr;
            players.Add(plr);
            return plr;
        }

        public void Run()
        {
            Stopwatch sw = new Stopwatch();
            float tpf = 0;
            sw.Start();

            long elapsedMs = 0;
            while (!shouldQuit)
            {
                tpf = sw.ElapsedMilliseconds / 1000.0f;
                
                sw.Restart();

                ProcessMessages();

                ProcessActionQueue();

                StateMgr.Update(tpf);

                if (tpf >= 0.001 && isInitialized)
                    Update(tpf);

                elapsedMs = sw.ElapsedMilliseconds;
                if (elapsedMs < SharedDef.MINIMUM_UPDATE_TIME) 
                {
                    Thread.Sleep((int)(SharedDef.MINIMUM_UPDATE_TIME - elapsedMs));
		        }

                Time += tpf;
            }

            sw.Stop();

            CleanUp();
        }

        private void RequestStop()
        {
            if (CloseCallback != null)
                CloseCallback(this);

            shouldQuit = true;
            Thread.Sleep(10);
        }

        public void Enqueue(Action act)
        {
            if (!shouldQuit)
                synchronizedQueue.Enqueue(act);
        }

        private void ProcessActionQueue()
        {
            Action act = null;
            while (synchronizedQueue.TryDequeue(out act))
                act.Invoke();
        }

        public void Update(float tpf)
        {
            CheckPlayerStates();
        }

        public Player GetPlayer(int id)
        {
            return players.Find(p => p.GetId() == id);
        }

        private Player GetPlayer(long netConnectionId)
        {
            return players.Find(plr => plr.Connection != null && plr.Connection.RemoteUniqueIdentifier == netConnectionId);
        }

        public Random GetRandomGenerator()
        {
            return randomGenerator;
        }

        private void CheckPlayerStates()
        {
            foreach (Player plr in players)
            {
                if (plr.IsActivePlayer() && plr.GetBaseIntegrity() <= 0)
                {
                    foreach (Player winner in players)
                    {
                        if (winner.IsActivePlayer() && winner.GetId() != plr.GetId())
                        {
                            if (GameType == Gametype.TOURNAMENT_GAME)
                                StopAndRequestScores(new Action(() => EndGame(winner, GameEnd.WIN_GAME)));
                            else
                                EndGame(winner, GameEnd.WIN_GAME);
                            return;
                        }
                    }
                }
            }
        }

        public int GetPlayerCount()
        {
            return players.Count;
        }

        private void StopAndRequestScores(Action action)
        {
            playersRespondedScore = new List<int>();
            savedEndGameAction = action;
            isInitialized = false;
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.SCORE_QUERY);
            BroadcastMessage(msg);
            StateMgr.AddGameState(new DelayedActionInvoker(1, new Action(() => SkipWaitingForScoreQueryResponse())));
        }

        private void PlayerWon(Player winner)
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_WON);
            msg.Write(winner.GetId());
            msg.Write(winner.Data.WonMatches);
            BroadcastMessage(msg);
        }

        public void SendChatMessage(string message)
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.CHAT_MESSAGE);
            msg.Write("Server: " + message);
            BroadcastMessage(msg);
        }

        public bool IsGameRunning()
        {
            return (gameSession == null) ? false : gameSession.IsRunning;
        }

        public bool PlayerExists(string hashId)
        {
            return players.Exists(p => p.Data.HashId == hashId);
        }
    }
}
