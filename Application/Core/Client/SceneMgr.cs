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
using Orbit.Gui;
using Orbit.Core.Utils;
using Orbit.Core.Weapons;

namespace Orbit.Core.Client
{
    public partial class SceneMgr
    {
        public Gametype GameType { get; set; }
        public Size ViewPortSizeOriginal { get; set; }

        private Canvas canvas;
        private bool isInitialized;
        private bool isGameInitialized;
        private bool userActionsDisabled;
        private volatile bool shouldQuit;
        private List<ISceneObject> objects;
        private List<ISceneObject> objectsToRemove;
        private List<ISceneObject> objectsToAdd;
        private List<Player> players;
        private Player currentPlayer;
        private Rect orbitArea;
        private Random randomGenerator;
        private ConcurrentQueue<Action> synchronizedQueue;
        private bool gameEnded;
        private float statisticsTimer;
        //private PlayerActionManager actionMgr;
        private GameStateManager stateMgr;

        //private static SceneMgr sceneMgr;
        //private static Object lck = new Object();

        /*public static SceneMgr GetInstance()
        {
            lock (lck)
            {
                if (sceneMgr == null)
                    sceneMgr = new SceneMgr();
                return sceneMgr;
            }
        }*/

        public SceneMgr()
        {
            isInitialized = false;
            isGameInitialized = false;
            synchronizedQueue = new ConcurrentQueue<Action>();
        }

        public void Init(Gametype gameType)
        {
            GameType = gameType;
            gameEnded = false;
            isGameInitialized = false;
            userActionsDisabled = true;
            shouldQuit = false;
            objects = new List<ISceneObject>();
            objectsToRemove = new List<ISceneObject>();
            objectsToAdd = new List<ISceneObject>();
            randomGenerator = new Random(Environment.TickCount);
            players = new List<Player>(2);
            statisticsTimer = 0;
            stateMgr = new GameStateManager();

            currentPlayer = CreatePlayer();
            players.Add(currentPlayer);
            stateMgr.AddGameState(currentPlayer);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                currentPlayer.Data.Name = (Application.Current as App).GetPlayerName();
            }));

            if (gameType != Gametype.LOBBY_GAME)
            {
                stateMgr.AddGameState(new PlayerActionManager(this));
                Invoke(new Action(() =>
                {
                    Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblEndGame");
                    if (lbl != null)
                        lbl.Content = "";

                    Label lblw = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblWaiting");
                    if (lblw != null)
                        lblw.Content = "";

                }));
            }

            if (gameType == Gametype.SERVER_GAME)
            {
                SetMainInfoText("Waiting for the other player to connect");
            }
            else if (gameType == Gametype.CLIENT_GAME)
            {
                SetMainInfoText("Waiting for the server");
            }
            else if (gameType == Gametype.SOLO_GAME)
            {
                userActionsDisabled = false;
            }

            InitNetwork();
            ConnectToServer();
            isInitialized = true;
        }

        private Player CreatePlayer()
        {
            Player plr = new Player(this);
            plr.Data = new PlayerData();
            return plr;
        }

        private void SetMainInfoText(String t)
        {
            Invoke(new Action(() =>
            {
                Label lblw = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblWaiting");
                if (lblw != null)
                    lblw.Content = t;
            }));
        }

        public void ShowStatusText(int index, string text)
        {
            Invoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "statusText" + index);
                if (lbl != null)
                    lbl.Content = text;
            }));
        }

        public void CloseGame()
        {
            RequestStop();
        }

        private void CleanUp()
        {
            if (client != null && client.Status != NetPeerStatus.NotRunning)
            {
                client.Shutdown("Peer closed connection");
                Thread.Sleep(10); // networking threadu chvili trva ukonceni
            }

            objectsToRemove.Clear();

            if (canvas != null)
                Invoke(new Action(() =>
                {
                    foreach (ISceneObject obj in objects)
                        canvas.Children.Remove(obj.GetGeometry());
                }));
        }

        /************************************************************************/
        /* manipulace s objekty                                                 */
        /************************************************************************/

        private void AddObjectsReadyToAdd()
        {
            objectsToAdd.ForEach(o => DirectAttachToScene(o));
            objectsToAdd.Clear();
        }

        /// <summary>
        /// ihned prida objekt do sceny
        /// </summary>
        private void DirectAttachToScene(ISceneObject obj)
        {
            objects.Add(obj);
            BeginInvoke(new Action(() =>
            {
                canvas.Children.Add(obj.GetGeometry());
            }));
        }

        /// <summary>
        /// bezpecne prida objekt (SceneObject i gui objekt) v dalsim updatu
        /// </summary>
        public void DelayedAttachToScene(ISceneObject obj)
        { 
            objectsToAdd.Add(obj);
        }

        /// <summary>
        /// prida GUI objekt do sceny - nikoliv SceneObject
        /// </summary>
        public void AttachGraphicalObjectToScene(UIElement obj)
        {
            BeginInvoke(new Action(() =>
            {
                canvas.Children.Add(obj);
            }));
        }

        /// <summary>
        /// odstrani jen GUI element
        /// </summary>
        public void RemoveGraphicalObjectFromScene(UIElement obj)
        {
            BeginInvoke(new Action(() =>
            {
                canvas.Children.Remove(obj);
            }));
        }

        /// <summary>
        /// bezpecne odstrani objekt (SceneObject i gui objekt) v dalsim updatu
        /// </summary>
        public void RemoveFromSceneDelayed(ISceneObject obj)
        {
            obj.Dead = true;
            objectsToRemove.Add(obj);
        }

        /// <summary>
        /// ihned odebere objekt ze sceny
        /// </summary>
        private void DirectRemoveFromScene(ISceneObject obj)
        {
            objects.Remove(obj);
            BeginInvoke(new Action(() =>
            {
                canvas.Children.Remove(obj.GetGeometry());
            }));
        }

        private void RemoveObjectsMarkedForRemoval()
        {
            foreach (ISceneObject obj in objectsToRemove)
            {
                obj.OnRemove();
                DirectRemoveFromScene(obj);
            }

            objectsToRemove.Clear();
        }

        /************************************************************************/
        /* konec manipulace s objekty                                           */
        /************************************************************************/

        public Canvas GetCanvas()
        {
            return canvas;
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

                ProcessActionQueue();

                if (tpf >= 0.001 && isGameInitialized)
                    Update(tpf);

		        if (sw.ElapsedMilliseconds < SharedDef.MINIMUM_UPDATE_TIME) 
                {
                    Thread.Sleep((int)(SharedDef.MINIMUM_UPDATE_TIME - sw.ElapsedMilliseconds));
		        }
            }

            sw.Stop();

            CleanUp();

            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    (Application.Current as App).GameEnded();
                }));
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

            AddObjectsReadyToAdd();

            stateMgr.Update(tpf);

            UpdateSceneObjects(tpf);
            RemoveObjectsMarkedForRemoval();

            CheckCollisions();
            RemoveObjectsMarkedForRemoval();

            try
            {
                UpdateGeomtricState();
            }
            catch (NullReferenceException e)
            {
                // UI is closed before game finished its Update loop
                System.Console.Error.WriteLine(e);
            }
        }

        private void ShowStatistics(float tpf)
        {
            statisticsTimer += tpf;
            if (statisticsTimer < 0.5)
                return;

            statisticsTimer = 0;

            ShowStatusText(1, "TPF: " + tpf + " FPS: " + 1.0f / tpf);
            if (GameType != Gametype.SOLO_GAME && GetCurrentPlayer().Connection != null)
                ShowStatusText(2, "LATENCY: " + GetCurrentPlayer().Connection.AverageRoundtripTime);
        }

        private void ProcessActionQueue()
        {
            Action act = null;
            while (synchronizedQueue.TryDequeue(out act))
                act.Invoke();
        }

        private void UpdateGeomtricState()
        {
            foreach (ISceneObject obj in objects)
            {
                obj.UpdateGeometric();
            }
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

        public Player GetCurrentPlayer()
        {
            return currentPlayer;
        }

        public Player GetOpponentPlayer()
        {
            return players.Find(plr => plr.Data.Active && plr.GetId() != GetCurrentPlayer().GetId());
        }

        public Player GetPlayer(int id)
        {
            return players.Find(p => p.GetId() == id);
        }

        public void SetCanvas(Canvas canvas)
        {
            this.canvas = canvas;
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

        public void OnCanvasMouseMove(Point point)
        {
            if (currentPlayer.Shooting)
                currentPlayer.TargetPoint = point;
        }

        public void OnCanvasClick(Point point, MouseButtonEventArgs e)
        {
            if (userActionsDisabled)
                return;

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    if (e.ButtonState == MouseButtonState.Pressed)
                        currentPlayer.Mine.Shoot(point);
                    break;
                case MouseButton.Right:
                    currentPlayer.Shooting = e.ButtonState == MouseButtonState.Pressed;
                    currentPlayer.TargetPoint = point;
                    break;
                case MouseButton.Middle:
                    if (e.ButtonState == MouseButtonState.Pressed)
                        currentPlayer.Hook.Shoot(point);
                    break;
            }          
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
            else if (endType == GameEnd.SERVER_DISCONNECTED)
                Disconnected();

            RequestStop();

            Thread.Sleep(3000);
        }

        private void Disconnected()
        {
            Invoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblEndGame");
                if (lbl != null)
                    lbl.Content = "Disconnected from the server";
            }));
        }

        private void PlayerWon(Player winner)
        {
            string text;
            // kdyz maji hraci stejna jmena, tak jsou rozliseni barvami
            if (players.Find(p => p.IsActivePlayer() && p.GetId() != winner.GetId()).Data.Name.Equals(winner.Data.Name))
                text = (winner.Data.PlayerColor == Colors.Red ? "Red" : "Blue") + " player wins!";
            else
                text = winner.Data.Name + " wins!";
            Invoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblEndGame");
                if (lbl != null)
                    lbl.Content = text;
            }));
        }

        private void PlayerLeft(Player leaver)
        {
            if (leaver == null)
                return;

            string text;
            // kdyz maji hraci stejna jmena, tak jsou rozliseni barvami
            if (players.Find(p => p.IsActivePlayer() && p.GetId() != leaver.GetId()).Data.Name.Equals(leaver.Data.Name))
                text = (leaver.Data.PlayerColor == Colors.Red ? "Red" : "Blue") + " player left the game!";
            else
                text = leaver.Data.Name + " left the game!";

            Invoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblEndGame");
                if (lbl != null)
                    lbl.Content = text;
            }));
        }

        public void Invoke(Action a)
        {
            canvas.Dispatcher.Invoke(a);
        }

        public void BeginInvoke(Action a)
        {
            canvas.Dispatcher.BeginInvoke(a);
        }

        private void CheckAllPlayersReady()
        {
            ShowChatMessage("checking players for ready state: " + players.Count);

            bool ready = true;
            if (players.Count < 2)
                ready = false;

            players.ForEach(p => ShowChatMessage("plr: " + p.GetId() + " " + p.Data.LobbyReady));

            if (ready)
                foreach (Player p in players)
                    if (!p.Data.LobbyReady)
                    {
                        ready = false;
                        break;
                    }

            (Application.Current as App).Dispatcher.BeginInvoke(new Action(() =>
            {
                LobbyUC wnd = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "lobbyWindow") as LobbyUC;
                if (wnd != null)
                    wnd.AllReady(ready);
            }));
        }

        public void ShowChatMessage(string message)
        {
            (Application.Current as App).Dispatcher.BeginInvoke(new Action(() =>
            {
                ListView chat = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "lvChat") as ListView;
                if (chat != null)
                {
                    chat.Items.Add(message);
                    chat.ScrollIntoView(chat.Items[chat.Items.Count - 1]);
                }
            }));
        }

        public void SendChatMessage(string message)
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.CHAT_MESSAGE);
            msg.Write(currentPlayer.Data.Name + ": " + message);
            SendMessage(msg);
        }

        private void UpdateLobbyPlayers()
        {
            List<LobbyPlayerData> data = new List<LobbyPlayerData>();
            players.ForEach(p => data.Add(new LobbyPlayerData(p.Data.Id, p.Data.Name, p.Data.Score)));

            (Application.Current as App).Dispatcher.BeginInvoke(new Action(() =>
            {
                LobbyUC lobby = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "lobbyWindow") as LobbyUC;
                if (lobby != null)
                    lobby.UpdateShownPlayers(data);
            }));
        }
    }
}
