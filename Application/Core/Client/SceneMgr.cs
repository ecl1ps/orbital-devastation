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
using System.Windows.Media.Imaging;
using Orbit.Core.Players.Input;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.Controls.Collisions;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Client
{
    public partial class SceneMgr
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Gametype GameType { get; set; }
        public FloatingTextManager FloatingTextMgr { get; set; }
        public StatsMgr StatsMgr { get; set; }
        public GameStateManager StateMgr { get; set; }
        public LevelEnvironment LevelEnv { get; set; }
        public WindowState GameWindowState { get { return gameWindowState; } set { gameWindowState = value; } }

        private volatile WindowState gameWindowState;

        private GameVisualArea area;
        private bool isGameInitialized;
        private bool userActionsDisabled;
        private volatile bool shouldQuit;
        private List<ISceneObject> objects;
        private List<ISceneObject> objectsToRemove;
        private List<ISceneObject> objectsToAdd;
        private List<long> idsToRemove;
        private List<Player> players;
        private Player currentPlayer;
        private Random randomGenerator;
        private ConcurrentQueue<Action> synchronizedQueue;
        private bool gameEnded;
        private float statisticsTimer;
        private ActionBarMgr actionBarMgr;
        private IInputMgr inputMgr;
        private bool playerQuit;
        private GameEnd lastGameEnd;

        public SceneMgr()
        {
            StatsMgr = new StatsMgr(this);
            isGameInitialized = false;
            shouldQuit = false;
            synchronizedQueue = new ConcurrentQueue<Action>();
            GameWindowState = WindowState.IN_MAIN_MENU;
        }

        public void Init(Gametype gameType)
        {
            GameType = gameType;
            gameEnded = false;
            isGameInitialized = false;
            userActionsDisabled = true;
            shouldQuit = false;
            playerQuit = false;
            objects = new List<ISceneObject>();
            objectsToRemove = new List<ISceneObject>();
            objectsToAdd = new List<ISceneObject>();
            idsToRemove = new List<long>();
            randomGenerator = new Random(Environment.TickCount);
            players = new List<Player>(2);
            statisticsTimer = 0;
            StateMgr = new GameStateManager();

            currentPlayer = CreatePlayer();
            currentPlayer.Data.PlayerColor = Player.GetChosenColor();
            players.Add(currentPlayer);
            StateMgr.AddGameState(currentPlayer);
            FloatingTextMgr = new FloatingTextManager(this);
            StateMgr.AddGameState(FloatingTextMgr);
            LevelEnv = new LevelEnvironment();
            StateMgr.AddGameState(LevelEnv);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                currentPlayer.Data.Name = App.Instance.PlayerName;
                currentPlayer.Data.HashId = App.Instance.PlayerHashId;
            }));

            if (gameType != Gametype.TOURNAMENT_GAME)
            {
                Invoke(new Action(() =>
                {
                    Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "lblEndGame");
                    if (lbl != null)
                        lbl.Content = "";

                    Label lblw = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "lblWaiting");
                    if (lblw != null)
                        lblw.Content = "";
                }));
            }

            if (gameType == Gametype.MULTIPLAYER_GAME)
                SetMainInfoText("Establishing connection...");

            InitNetwork();
            ConnectToServer();
        }

        public void InitStaticMouse()
        {
            StaticMouse.Init(this);
            StaticMouse.Enable(true);
            StateMgr.AddGameState(StaticMouse.Instance);
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
                Label lblw = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "lblWaiting");
                if (lblw != null && area.Parent != null)
                    lblw.Content = t;
            }));
        }

        public void ShowStatusText(int index, string text)
        {
            Invoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "statusText" + index);
                if (lbl != null && area.Parent != null)
                    lbl.Content = text;
            }));
        }

        public void CloseGame()
        {
            RequestStop();
        }

        private void CleanUp()
        {
            shouldQuit = false;
            currentPlayer.Data.StartReady = false;

            if (client != null && client.Status != NetPeerStatus.NotRunning)
            {
                NetOutgoingMessage msg = CreateNetMessage();
                msg.Write((int)PacketType.PLAYER_DISCONNECTED);
                msg.Write(currentPlayer.GetId());
                SendMessage(msg);

                client.FlushSendQueue();
                client.Disconnect("Client closed connection");
                // bussy wait for shutdown
                while (client.ConnectionStatus != NetConnectionStatus.Disconnected && client.ConnectionStatus != NetConnectionStatus.None)
                    ;
            }

            if (area != null)
            {
                Invoke(new Action(() =>
                {
                    area.Clear();
                }));
            }

            objectsToRemove.Clear();
            objects.Clear();
            objectsToAdd.Clear();
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
            if (obj.Id == -1)
            {
                Logger.Error("Trying to add object " + obj.GetType().Name + " to scene, but it has uninitialized id -> skipped!");
                return;
            }

            for (int i = idsToRemove.Count - 1; i >= 0; i--)
            {
                if (obj.Id == idsToRemove[i])
                {
                    idsToRemove.RemoveAt(i);
                    return;
                }
            }

            if (obj.GetGeometry() == null)
            {
                Logger.Warn("Trying to add geometry object to scene, but it is null -> skipped!");
                return;
            }

            obj.OnAttach();
            objects.Add(obj);

            BeginInvoke(new Action(() =>
            {
                area.Add(obj.GetGeometry());
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
        public void AttachGraphicalObjectToScene(Drawing obj)
        {
            BeginInvoke(new Action(() =>
            {
                area.Add(obj);
            }));
        }

        /// <summary>
        /// odstrani jen GUI element
        /// </summary>
        public void RemoveGraphicalObjectFromScene(Drawing obj)
        {
            BeginInvoke(new Action(() =>
            {
                area.Remove(obj);
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
                area.Remove(obj.GetGeometry());
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

        // TODO odstranit
        public Canvas GetCanvas()
        {
            return area.Parent as Canvas;
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

                if (Application.Current == null)
                    continue;

                if (tpf >= 0.001 && isGameInitialized)
                    Update(tpf);

                area.RunRender();

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
        }

        public void Enqueue(Action act)
        {
            if (!shouldQuit)
                synchronizedQueue.Enqueue(act);
        }

        public void Update(float tpf)
        {
            if (GameWindowState != WindowState.IN_GAME)
                return;

            ShowStatistics(tpf);

            AddObjectsReadyToAdd();

            StateMgr.Update(tpf);

            UpdateSceneObjects(tpf);
            RemoveObjectsMarkedForRemoval();

            CheckCollisions(tpf);
            RemoveObjectsMarkedForRemoval();

            UpdateGeomtricState();
        }

        private void ShowStatistics(float tpf)
        {
            statisticsTimer += tpf;
            if (statisticsTimer < 0.5)
                return;

            statisticsTimer = 0;

            ShowStatusText(1, "TPF: " + tpf + " FPS: " + (int)(1.0f / tpf));
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
            Invoke(new Action(() =>
            {
                foreach (ISceneObject obj in objects)
                    obj.UpdateGeometric();
            }));
        }

        public void UpdateSceneObjects(float tpf)
        {
            foreach (ISceneObject obj in objects)
            {             
                obj.Update(tpf);
                if (!obj.IsOnScreen(SharedDef.VIEW_PORT_SIZE))
                    RemoveFromSceneDelayed(obj);
            }
        }

        public void CheckCollisions(float tpf)
        {
            foreach (ISceneObject obj1 in objects)
            {
                if (obj1.CollisionShape == null)
                    continue;

                foreach (ISceneObject obj2 in objects)
                {
                    if (obj1.Id == obj2.Id)
                        continue;

                    if (obj2.CollisionShape == null)
                        continue;

                    if (obj1.CollisionShape.CollideWith(obj2.CollisionShape))
                        obj1.GetControlsOfType<ICollisionReactionControl>().ForEach(c => c.DoCollideWith(obj2, tpf));
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

        public void SetGameVisualArea(GameVisualArea gva)
        {
            area = gva;
        }

        public Random GetRandomGenerator()
        {
            return randomGenerator;
        }

        public void OnCanvasMouseMove(Point point)
        {
            if (currentPlayer.Shooting)
                currentPlayer.TargetPoint = (StaticMouse.Instance != null && StaticMouse.ALLOWED) ? StaticMouse.GetPosition() : point;
        }

        public void OnCanvasClick(Point point, MouseButtonEventArgs e)
        {
            if (userActionsDisabled || !isGameInitialized)
                return;

            if (StaticMouse.Instance != null && StaticMouse.ALLOWED)
                point = StaticMouse.GetPosition();

            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed &&
                !IsPointInViewPort(point) && StaticMouse.Instance != null && StaticMouse.ALLOWED)
            {
                ProcessStaticMouseActionBarClick(point);
                return;
            }

            if (IsPointInViewPort(point))
                inputMgr.OnCanvasClick(point, e);
            else
                inputMgr.OnActionBarClick(point, e);
        }

        private void ProcessStaticMouseActionBarClick(Point point)
        {
            Invoke(new Action(() =>
            {
                actionBarMgr.ActionBar.OnClick(area.PointToScreen(point));
            }));
        }

        public static bool IsPointInViewPort(Point point)
        {
            if (point.X > SharedDef.VIEW_PORT_SIZE.Width || point.X < 0)
                return false;

            if (point.Y > SharedDef.VIEW_PORT_SIZE.Height || point.Y < 0)
                return false;

            return true;
        }

        private void EndGame(Player plr, GameEnd endType)
        {
            if (endType == GameEnd.TOURNAMENT_FINISHED)
                TournamenFinished(plr);

            if (gameEnded)
                return;

            if (Application.Current != null)
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    App.Instance.SetGameStarted(false);
                }));

            if (endType == GameEnd.WIN_GAME)
                CheckHighScore();

            isGameInitialized = true;
            gameEnded = true;
            userActionsDisabled = true;

            if (endType == GameEnd.WIN_GAME)
                PlayerWon(plr);
            else if (endType == GameEnd.LEFT_GAME)
                PlayerLeft(plr);
            else if (endType == GameEnd.SERVER_DISCONNECTED)
                Disconnected();

            lastGameEnd = endType;

            // po urcitem case zavola metodu CloseGameWindowAndCleanup()
            if (GameWindowState == WindowState.IN_GAME)
                StateMgr.AddGameState(new DelayedActionInvoker(3.0f, new Action(() => { CloseGameWindowAndCleanup(); })));
            else
                CloseGameWindowAndCleanup();
        }

        public void CloseGameWindowAndCleanup()
        {
            if (GameType != Gametype.TOURNAMENT_GAME || lastGameEnd == GameEnd.SERVER_DISCONNECTED || lastGameEnd == GameEnd.TOURNAMENT_FINISHED)
                RequestStop();

            StateMgr.Clear();
            StaticMouse.Enable(false);

            if (Application.Current == null)
                return;

            if (GameType == Gametype.TOURNAMENT_GAME && lastGameEnd != GameEnd.SERVER_DISCONNECTED && lastGameEnd != GameEnd.TOURNAMENT_FINISHED)
                TournamentGameEnded();
            else if (lastGameEnd != GameEnd.TOURNAMENT_FINISHED)
                NormalGameEnded();
        }

        public void PlayerQuitGame()
        {
            playerQuit = true;
            serverConnection.Disconnect("Quit");
        }

        private void CheckHighScore()
        {
            // zatim jen pro solo a 1v1 hry
            if (GameType != Gametype.SOLO_GAME && GameType != Gametype.MULTIPLAYER_GAME)
                return;

            PropertyKey key = PropertyKey.PLAYER_HIGHSCORE_SOLO1;

            if (GameType == Gametype.MULTIPLAYER_GAME)
                key = PropertyKey.PLAYER_HIGHSCORE_QUICK_GAME;
            else if (GameType == Gametype.SOLO_GAME)
            {
                switch (GetOpponentPlayer().Data.BotType)
                {
                    case BotType.LEVEL1:
                        key = PropertyKey.PLAYER_HIGHSCORE_SOLO1;
                        break;
                    case BotType.LEVEL2:
                        key = PropertyKey.PLAYER_HIGHSCORE_SOLO2;
                        break;
                    case BotType.LEVEL3:
                        key = PropertyKey.PLAYER_HIGHSCORE_SOLO3;
                        break;
                    case BotType.LEVEL4:
                        key = PropertyKey.PLAYER_HIGHSCORE_SOLO4;
                        break;
                    case BotType.LEVEL5:
                        key = PropertyKey.PLAYER_HIGHSCORE_SOLO5;
                        break;
                    default:
                        break;
                }
            }

            int hs = int.Parse(GameProperties.Props.Get(key));
            if (hs < currentPlayer.Data.Score)
            {
                hs = currentPlayer.Data.Score;
                GameProperties.Props.SetAndSave(key, hs);
                Invoke(new Action(() =>
                {
                    Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "lblHighScore");
                    if (lbl != null)
                        lbl.Content = "New HighScore " + hs + "!";
                }));
            }

        }

        private void TournamenFinished(Player winner)
        {
            if (Application.Current == null)
                return;

            lastGameEnd = GameEnd.TOURNAMENT_FINISHED;

            if (winner == null)
                return;

            List<LobbyPlayerData> data = CreateLobbyPlayerData();

            LobbyPlayerData winnerData = data.Find(d => d.Id == winner.Data.Id);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                App.Instance.CreateScoreboardGui(winnerData, data);
            }));
        }

        private void TournamentGameEnded()
        {
            gameEnded = false;
            isGameInitialized = false;
            userActionsDisabled = true;

            if (area != null)
            {
                Invoke(new Action(() =>
                {
                    area.Clear();
                }));
            }

            objects.Clear();
            objectsToRemove.Clear();
            objectsToAdd.Clear();

            players.ForEach(p => p.Data.LobbyReady = false);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                App.Instance.CreateLobbyGui(currentPlayer.Data.LobbyLeader, true);
            }));

            SendPlayerDataRequestMessage();
            SendTournamentSettingsRequest();

            if (currentPlayer.Data.LobbyLeader)
            {
                currentPlayer.Data.LobbyReady = true;
                SendPlayerReadyMessage();
            }
        }

        private void NormalGameEnded()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                App.Instance.GameEnded();
            }));
        }

        private void Disconnected()
        {
            if (area == null)
                return;

            string msg;
            if (!playerQuit)
                msg = "Disconnected from the host";
            else
                msg = "End of Game";
            Invoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "lblEndGame");
                if (lbl != null)
                    lbl.Content = msg;
            }));
        }

        private void PlayerWon(Player winner)
        {
            string text = winner.Data.Name + " wins!";
            Invoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "lblEndGame");
                if (lbl != null)
                    lbl.Content = text;
            }));
        }

        private void PlayerLeft(Player leaver)
        {
            if (leaver == null)
                return;

            string text = leaver.Data.Name + " left the game!";
            Invoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "lblEndGame");
                if (lbl != null)
                    lbl.Content = text;
            }));
        }

        public void Invoke(Action a)
        {
            area.Dispatcher.Invoke(a);
        }

        public void BeginInvoke(Action a)
        {
            area.Dispatcher.BeginInvoke(a);
        }

        private void CheckAllPlayersReady()
        {

            bool ready = true;
            if (players.Count < 2)
                ready = false;

            if (ready)
                foreach (Player p in players)
                    if (!p.Data.LobbyReady)
                    {
                        ready = false;
                        break;
                    }

            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    LobbyUC wnd = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "lobbyWindow") as LobbyUC;
                    if (wnd != null)
                        wnd.AllReady(ready);
                }));
        }

        public void ShowChatMessage(string message)
        {
            if (Application.Current == null)
                return;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ListView chat = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "lvChat") as ListView;
                if (chat != null)
                {
                    chat.Items.Add(message);
                    chat.ScrollIntoView(chat.Items[chat.Items.Count - 1]);
                }
            }));
        }

        public ISceneObject GetSceneObject(long id)
        {
            foreach (ISceneObject obj in objects)
            {
                if (obj.Id == id)
                    return obj;
            }

            return null;
        }

        public List<ISceneObject> GetSceneObjects()
        {
            List<ISceneObject> temp = new List<ISceneObject>(objects);

            objectsToAdd.ForEach(obj => temp.Add(obj));

            return temp;
        }

        public List<ISceneObject> GetSceneObjects(Type clazz)
        {
            List<ISceneObject> temp = new List<ISceneObject>();

            //TODO zkusit otocit podminku
            foreach (ISceneObject obj in objects) 
            {
                if (obj.GetType().IsAssignableFrom(clazz))
                    temp.Add(obj);
            }

            return temp;
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
            if (Application.Current == null)
                return;

            List<LobbyPlayerData> data = CreateLobbyPlayerData();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                LobbyUC lobby = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "lobbyWindow") as LobbyUC;
                if (lobby != null)
                    lobby.UpdateShownPlayers(data);
            }));
        }

        private List<LobbyPlayerData> CreateLobbyPlayerData()
        {
            List<LobbyPlayerData> data = new List<LobbyPlayerData>();
            players.ForEach(p => data.Add(new LobbyPlayerData(p.Data.Id, p.Data.Name, p.Data.Score, p.Data.LobbyLeader,
                p.Data.LobbyReady, p.Data.PlayedMatches, p.Data.WonMatches, p.Data.PlayerColor)));
            return data;
        }

        public Player GetOtherActivePlayer(int firstPlayerId)
        {
            return players.Find(p => p.IsActivePlayer() && p.GetId() != firstPlayerId);
        }

        public void OnKeyEvent(KeyEventArgs e)
        {
            if (!isGameInitialized || inputMgr == null)
                return;

            switch (e.Key)
            {
                case Key.Tab:
                    {
                        if (!e.IsDown)
                            break;

                        ShowPlayerOverview();
                    }
                    break;
            }

            inputMgr.OnKeyEvent(e);
        }

        private List<PlayerOverviewData> GetPlayerOverviewData()
        {
            List<PlayerOverviewData> data = new List<PlayerOverviewData>(players.Count);
            foreach (Player p in players)
            {
                if (p.IsActivePlayer())
                    data.Add(new PlayerOverviewData(p.Data.Name, p.Data.Score, p.Data.Gold, p.Data.Active, p.Data.PlayedMatches, p.Data.WonMatches,
                        p.Mine.UpgradeLevel, p.Canoon.UpgradeLevel, p.Hook.UpgradeLevel));
                else
                    data.Add(new PlayerOverviewData(p.Data.Name, p.Data.Score, p.Data.Gold, p.Data.Active, p.Data.PlayedMatches, p.Data.WonMatches,
                        UpgradeLevel.LEVEL_NONE, UpgradeLevel.LEVEL_NONE, UpgradeLevel.LEVEL_NONE));
            }
            return data;
        }

        internal void PlayerColorChanged()
        {
            Color newColor = Player.GetChosenColor();
            if (players == null || players.Exists(p => p.GetPlayerColor() == newColor))
                return;

            GetCurrentPlayer().Data.PlayerColor = Player.GetChosenColor();

            if (GameWindowState == WindowState.IN_LOBBY)
            {
                SendPlayerColorChanged();
                UpdateLobbyPlayers();
            }
        }

        internal void ShowPlayerOverview()
        {
            List<PlayerOverviewData> data = GetPlayerOverviewData();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                App.Instance.ShowGameOverview(data);
            }));
        }
    }
}
