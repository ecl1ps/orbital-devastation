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
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.Concurrent;
using Lidgren.Network;
using System.Windows.Input;
using Orbit.Core;
using Orbit.Core.Scene;
using Orbit.Core.Server.Match;

namespace Orbit.Core.Server
{
    public partial class ServerMgr
    {
        private bool isInitialized;
        private volatile bool shouldQuit;
        private List<Player> players;
        private Random randomGenerator;
        public Size ViewPortSizeOriginal { get; set; }
        private ConcurrentQueue<Action> synchronizedQueue;
        public Gametype GameType { get; set; }
        private bool gameEnded;
        private GameManager gameSession;

        public ServerMgr()
        {
            isInitialized = false;
            shouldQuit = false;
        }

        public void Init(Gametype gameType)
        {
            GameType = gameType;
            gameEnded = false;
            isInitialized = false;
            shouldQuit = false;
            randomGenerator = new Random(Environment.TickCount);
            players = new List<Player>(2);
            synchronizedQueue = new ConcurrentQueue<Action>();

            InitNetwork();
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
                msg.Write((int)PacketType.PLAYER_SCORE);
                msg.Write(p.GetId());
                msg.Write(p.Data.Score);
                BroadcastMessage(msg, p);
            }

            gameEnded = true;
            if (endType == GameEnd.WIN_GAME)
                PlayerWon(plr);
            else if (endType == GameEnd.LEFT_GAME)
                PlayerLeft(plr);

            if (GameType != Gametype.TOURNAMENT_GAME)
                RequestStop();
            else
                TournamentGameEnded(plr, endType);
        }

        private void TournamentGameEnded(Player plr, GameEnd endType)
        {
            isInitialized = false;
            gameEnded = false;

            if (gameSession.CheckTournamentFinished(true))
            {
                RequestStop();
                return;
            }

            players.ForEach(p => p.Data.LobbyReady = false);
            players.ForEach(p => p.Data.BaseIntegrity = SharedDef.BASE_MAX_INGERITY);
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

            if (server != null && server.Status != NetPeerStatus.NotRunning)
            {
                server.Shutdown("Peer closed connection");
                Thread.Sleep(10); // networking threadu chvili trva ukonceni
            }
        }

        public Player CreateAndAddPlayer(String name, String hash)
        {
            Player plr = new Player(null);
            plr.Data = new PlayerData();
            plr.Data.Id = IdMgr.GetNewPlayerId();
            if (players.Exists(p => p.Data.Name.Equals(name)))
                plr.Data.Name = name + " " + plr.GetId();
            else
                plr.Data.Name = name;
            plr.Data.HashId = hash;
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

                if (tpf >= 0.001 && isInitialized)
                    Update(tpf);

                //Console.Out.WriteLine(Server tpf + ": " +sw.ElapsedMilliseconds);
                elapsedMs = sw.ElapsedMilliseconds;
                if (elapsedMs < SharedDef.MINIMUM_UPDATE_TIME) 
                {
                    Thread.Sleep((int)(SharedDef.MINIMUM_UPDATE_TIME - elapsedMs));
		        }
            }

            sw.Stop();

            CleanUp();
        }

        private void RequestStop()
        {
            shouldQuit = true;
            Thread.Sleep(500);
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

        private Player GetPlayer(NetConnection netConnection)
        {
            return players.Find(plr => plr.Connection != null && plr.Connection.RemoteUniqueIdentifier == netConnection.RemoteUniqueIdentifier);
        }

        public Random GetRandomGenerator()
        {
            return randomGenerator;
        }

        private void CheckPlayerStates()
        {
            foreach (Player plr in players)
                if (plr.IsActivePlayer() && plr.GetBaseIntegrity() <= 0)
                    foreach (Player winner in players)
                        if (winner.IsActivePlayer() && winner.GetId() != plr.GetId())
                        {
                            EndGame(winner, GameEnd.WIN_GAME);
                            return;
                        }
        }

        private void PlayerWon(Player winner)
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_WON);
            msg.Write(winner.GetId());
            msg.Write(winner.Data.WonMatches);
            BroadcastMessage(msg);
        }

        private void PlayerLeft(Player leaver)
        {
            if (leaver == null)
                return;
        }

        public Rect GetOrbitArea()
        {
            // TODO: prozatim hack - defaultne je canvas 800*600
            return new Rect(0, 0, 800, 600 / 3);
        }

        public void SendChatMessage(string message)
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.CHAT_MESSAGE);
            msg.Write("Server: " + message);
            BroadcastMessage(msg);
        }
    }
}
