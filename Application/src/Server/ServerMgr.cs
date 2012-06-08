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

namespace Orbit.Server
{
    public partial class ServerMgr : ISceneMgr
    {
        private bool isInitialized;
        private bool userActionsDisabled;
        private volatile bool shouldQuit;
        private List<ISceneObject> objects;
        private List<ISceneObject> objectsToRemove;
        private List<Player> players;
        private PlayerPosition mePlayer;
        private PlayerPosition firstPlayer;
        private PlayerPosition secondPlayer;
        private Rect orbitArea;
        private Random randomGenerator;
        public Size ViewPortSizeOriginal { get; set; }
        private ConcurrentQueue<Action> synchronizedQueue;
        public Gametype GameType { get; set; }
        private bool gameEnded;
        private float statisticsTimer;

        private static ServerMgr serverMgr;
        private static Object lck = new Object();


        /*public static ServerMgr GetInstance()
        {
            lock (lck)
            {
                if (serverMgr == null)
                    serverMgr = new ServerMgr();
                return serverMgr;
            }
        }*/

        public ServerMgr()
        {
            isInitialized = false;
        }

        public void Init(Gametype gameType)
        {
            GameType = gameType;
            gameEnded = false;
            isInitialized = false;
            shouldQuit = false;
            objects = new List<ISceneObject>();
            objectsToRemove = new List<ISceneObject>();
            randomGenerator = new Random(Environment.TickCount);
            players = new List<Player>(2);
            synchronizedQueue = new ConcurrentQueue<Action>();
            statisticsTimer = 0;

            if (gameType == Gametype.SERVER_GAME)
            {
                userActionsDisabled = true;
                CreatePlayers();
                mePlayer = firstPlayer;
                InitNetwork();
                CreateAsteroidField();
                isInitialized = false;
            }
            else if (gameType == Gametype.CLIENT_GAME)
            {
                userActionsDisabled = true;
                InitNetwork();
            }
            else /*Gametype.SOLO_GAME*/
            {
                userActionsDisabled = false;
                CreatePlayers();
                mePlayer = firstPlayer;
                CreateAsteroidField();
                isInitialized = true;
            }
        }

        private void SetMainInfoText(String t)
        {
            // na serveru nic
        }

        private void CreateAsteroidField()
        {
            for (int i = 0; i < SharedDef.ASTEROID_COUNT; ++i)
                AttachToScene(SceneObjectFactory.CreateNewRandomAsteroid(this, i % 2 == 0));
        }

        public void ShowStatusText(int index, string text)
        {
            // na serveru nic
        }

        private void EndGame(Player plr, GameEnd endType)
        {
            if (gameEnded)
                return;

            gameEnded = true;
            if (endType == GameEnd.WIN_GAME)
                PlayerWon(plr);
            else if (endType == GameEnd.LEFT_GAME)
                PlayerLeft(plr);

            RequestStop();

            Thread.Sleep(3000);
        }

        public void CloseGame()
        {
            RequestStop();
        }

        private void CleanUp()
        {
            if (peer != null && peer.Status != NetPeerStatus.NotRunning)
            {
                peer.Shutdown("Peer closed connection");
                Thread.Sleep(10); // networking threadu chvili trva ukonceni
            }

            objectsToRemove.Clear();
        }

        public void AttachToScene(ISceneObject obj, bool asNonInteractive = false)
        {
            if (!asNonInteractive)
                objects.Add(obj);
        }

        public void RemoveFromSceneDelayed(ISceneObject obj)
        {
            obj.Dead = true;
            objectsToRemove.Add(obj);
        }

        private void RemoveObjectsMarkedForRemoval()
        {
            foreach (ISceneObject obj in objectsToRemove)
            {
                obj.OnRemove();
                objects.Remove(obj);
            }

            objectsToRemove.Clear();
        }

        private void CreatePlayers()
        {
            firstPlayer = randomGenerator.Next(2) == 0 ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            secondPlayer = firstPlayer == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT;

            Player plr = new Player();
            plr.Data = new PlayerData(this);
            plr.Data.PlayerPosition = firstPlayer;
            plr.Data.PlayerColor = randomGenerator.Next(2) == 0 ? Colors.Red : Colors.Blue;

            Base baze = SceneObjectFactory.CreateBase(this, plr.Data);
            AttachToScene(baze);
            plr.Baze = baze;

            players.Add(plr);

            plr = new Player();
            plr.Data = new PlayerData(this);
            plr.Data.PlayerPosition = secondPlayer;
            plr.Data.PlayerColor = players[0].Data.PlayerColor == Colors.Blue ? Colors.Red : Colors.Blue;

            baze = SceneObjectFactory.CreateBase(this, plr.Data);
            AttachToScene(baze);
            plr.Baze = baze;

            players.Add(plr);
        }


        public void Run()
        {
            Stopwatch sw = new Stopwatch();
            float tpf = 0;
            sw.Start();

            while (!shouldQuit)
            {
                tpf = sw.ElapsedMilliseconds / 1000.0f;
                
                sw.Restart();

                ProcessMessages();

                if (tpf >= 0.001 && isInitialized)
                    Update(tpf);

                //Console.Out.WriteLine(tpf + ": " +sw.ElapsedMilliseconds);

		        if (sw.ElapsedMilliseconds < SharedDef.MINIMUM_UPDATE_TIME) 
                {
                    Thread.Sleep((int)(SharedDef.MINIMUM_UPDATE_TIME - sw.ElapsedMilliseconds));
		        }
            }

            sw.Stop();

            CleanUp();
        }

        private void RequestStop()
        {
            shouldQuit = true;
        }

        public void Enqueue(Action act)
        {
            if (!shouldQuit && isInitialized)
                synchronizedQueue.Enqueue(act);
        }

        public void Update(float tpf)
        {
            ShowStatistics(tpf);

            ProcessActionQueue();

            UpdateSceneObjects(tpf);
            RemoveObjectsMarkedForRemoval();

            CheckCollisions();
            RemoveObjectsMarkedForRemoval();

            GetPlayer(mePlayer).Update(tpf);

            CheckPlayerStates();
        }

        private void ShowStatistics(float tpf)
        {
            statisticsTimer += tpf;
            if (statisticsTimer < 0.5)
                return;

            statisticsTimer = 0;

            ShowStatusText(1, "TPF: " + tpf + " FPS: " + 1.0f / tpf);
            if (GameType != Gametype.SOLO_GAME)
                ShowStatusText(2, "LATENCY: " + GetOtherPlayer().Connection.AverageRoundtripTime);
        }

        private void ProcessActionQueue()
        {
            Action act = null;
            while (synchronizedQueue.TryDequeue(out act))
                act.Invoke();
        }

        public void UpdateSceneObjects(float tpf)
        {
            foreach (ISceneObject obj in objects)
            {             
                obj.Update(tpf);
                if (!obj.IsOnScreen(ViewPortSizeOriginal))
                    RemoveFromSceneDelayed(obj);
            }
        }


        public void CheckCollisions()
        {
            foreach (ISceneObject obj1 in objects)
            {
                if (!(obj1 is ICollidable))
                    continue;

                foreach (ISceneObject obj2 in objects)
                {
                    if (obj1.Id == obj2.Id)
                        continue;

                    /*if (obj2.IsDead())
                        continue;*/

                    if (!(obj2 is ICollidable))
                        continue;

                    if (((ICollidable)obj1).CollideWith((ICollidable)obj2))
                        ((ICollidable)obj1).DoCollideWith((ICollidable)obj2);
                }
            }
        }

        public Player GetPlayer(PlayerPosition pos)
        {
            foreach (Player plr in players)
            {
                if (plr.GetPosition() == pos)
                    return plr;
            }
            return null;
        }

        public void SetCanvas(Canvas canvas)
        {
            // TODO:
            //this.canvas = canvas;
            ViewPortSizeOriginal = new Size(canvas.Width, canvas.Height);
            orbitArea = new Rect(0, 0, canvas.Width, canvas.Height / 3);
        }

        public void OnViewPortChange(Size size)
        {
            
        }

        public Rect GetOrbitArea()
        {
            return orbitArea;
        }

        public Random GetRandomGenerator()
        {
            return randomGenerator;
        }

        private void CheckPlayerStates()
        {
            if (GetPlayer(firstPlayer).GetBaseIntegrity() <= 0)
                EndGame(GetPlayer(secondPlayer), GameEnd.WIN_GAME);
            else if (GetPlayer(secondPlayer).GetBaseIntegrity() <= 0)
                EndGame(GetPlayer(firstPlayer), GameEnd.WIN_GAME);
        }

        private void PlayerWon(Player winner)
        {
            if (GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = CreateNetMessage();
                msg.Write((int)PacketType.PLAYER_WON);
                msg.Write((byte)winner.GetPosition());
                SendMessage(msg);
            }
        }


        private void PlayerLeft(Player leaver)
        {
            if (leaver == null)
                return;
        }

        public Player GetOtherPlayer()
        {
            if (players.Count == 0)
                return null;

            if (GameType == Gametype.SOLO_GAME)
                return null;

            return IsServer() ? players[1] : players[0];
        }

        public Player GetMePlayer()
        {
            if (players.Count == 0)
                return null;

            if (GameType == Gametype.SOLO_GAME)
                return players[0];

            return IsServer() ? players[0] : players[1];
        }

        public void Invoke(Action a)
        {
            // zadne operace s grafikou na serveru
        }

        public void BeginInvoke(Action a)
        {
            // zadne operace s grafikou na serveru
        }
    }
}
