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
using Microsoft.Xna.Framework;
using System.Windows.Threading;
using System.Collections.Concurrent;
using Lidgren.Network;
using Orbit.Gui;
using Orbit.Core.Utils;
using Orbit.Core.Weapons;
using System.Windows.Media.Imaging;
using Orbit.Core.Players.Input;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.Controls.Collisions;
using Orbit.Gui.Visuals;
using Orbit.Core.Helpers;
using Orbit.Gui.ActionControllers;
using System.Globalization;
using Orbit.Core.Client.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Client
{
    public partial class SceneMgr : XNAControl.AbstractGame, IUpdatable, Invoker
    {
        private SpriteBatch background;
        private SpriteBatch spriteBatch;
        private int frames;
    
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Gametype GameType { get; set; }
        public FloatingTextManager FloatingTextMgr { get; set; }
        public StatsMgr StatsMgr { get; set; }
        public GameStateManager StateMgr { get; set; }
        public LevelEnvironment LevelEnv { get; set; }
        public SpectatorActionsManager SpectatorActionMgr { get; set; }
        public ScreenShakingManager ScreenShakingMgr { get; set; }
        public InputManager InputMgr { get; set; }
        public AbstractInputMgr PlayerInputManager { get; set; }
        public bool UserActionsDisabled { get { return userActionsDisabled; } }
        public bool IsGameInitalized { get { return isGameInitialized; } }
        public WindowState GameWindowState { get { return gameWindowState; } set { gameWindowState = value; } }

        private volatile WindowState gameWindowState;

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
        private bool playerQuit;
        private TournamentSettings lastTournamentSettings;
        private float totalTime;
        private bool stopUpdating = false;

        public SceneMgr(IntPtr handle)
            : base(handle, (int)SharedDef.VIEW_PORT_SIZE.Width, (int)SharedDef.VIEW_PORT_SIZE.Height)
        {
            StatsMgr = new StatsMgr(this);
            isGameInitialized = false;
            shouldQuit = false;
            synchronizedQueue = new ConcurrentQueue<Action>();
            GameWindowState = WindowState.IN_MAIN_MENU;
            Mouse.WindowHandle = handle;
        }
 
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.IsMouseVisible = true;
            background = new SpriteBatch(GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            InputMgr = new InputManager();
        }
 
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SceneGeometryFactory.Load(Content);
 
            // TODO: use this.Content to load your game content here
        }
 
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            CleanUp();
        }
 
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float tpf = (float) gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            ProcessMessages();

            ProcessActionQueue();

            InputMgr.Update(tpf);

            if (tpf >= 0.001 && isGameInitialized)
            {
                AddObjectsReadyToAdd();
                RemoveObjectsMarkedForRemoval();

                Update(tpf);
            }

            base.Update(gameTime);
        }
 
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Red);

            background.Begin(SpriteSortMode.Texture, BlendState.Opaque);
            background.Draw(SceneGeometryFactory.Background, SharedDef.CANVAS_SIZE, Color.White);
            background.End();

            if (IsGameInitalized)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

                UpdateGeomtricState();
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public void Init(Gametype gameType)
        {
            GameType = gameType;
            stopUpdating = false;
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
            totalTime = 0;
            StateMgr = new GameStateManager();

            currentPlayer = CreatePlayer();
            currentPlayer.Data.PlayerColor = Player.GetChosenColor();
            players.Add(currentPlayer);
            AttachStateManagers();

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                currentPlayer.Data.Name = App.Instance.PlayerName;
                currentPlayer.Data.HashId = App.Instance.PlayerHashId;
            }));

            if (gameType == Gametype.MULTIPLAYER_GAME)
                SetMainInfoText(Strings.networking_waiting);

            InitNetwork();
            ConnectToServer();
        }

        private void AttachStateManagers()
        {
            StateMgr.AddGameState(currentPlayer);
            FloatingTextMgr = new FloatingTextManager(this);
            StateMgr.AddGameState(FloatingTextMgr);
            LevelEnv = new LevelEnvironment();
            StateMgr.AddGameState(LevelEnv);
            SpectatorActionMgr = new SpectatorActionsManager();
            StateMgr.AddGameState(SpectatorActionMgr);
            ScreenShakingMgr = new ScreenShakingManager(this);
            StateMgr.AddGameState(ScreenShakingMgr);
        }

        public void InitStaticMouse()
        {
            StaticMouse.Init(this);
            StaticMouse.Enable(true);
            StateMgr.AddGameState(StaticMouse.Instance);
        }

        public void InitAutomaticMineLauncher()
        {
            StateMgr.AddGameState(new AutomaticMineLauncher(GetCurrentPlayer()));
        }

        private Player CreatePlayer()
        {
            Player plr = new Player(this);
            plr.Data = new PlayerData();
            return plr;
        }

        private void SetMainInfoText(String t)
        {
            /* TODO přehodit přes text managera
            Invoke(new Action(() =>
            {
                Label lblw = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "lblWaiting");
                if (lblw != null && area.Parent != null)
                    lblw.Content = t;
            }));
             */
        }

        public void ShowStatusText(int index, string text)
        {
            /* TODO poresit jinak
            Invoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "statusText" + index);
                if (lbl != null && area.Parent != null)
                    lbl.Content = text;
            }));
             */ 
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
                client.Disconnect(Strings.networking_client_connection_close);
                // bussy wait for shutdown
                while (client.ConnectionStatus != NetConnectionStatus.Disconnected && client.ConnectionStatus != NetConnectionStatus.None)
                    Thread.Sleep(1);
            }

            CleanObjects();
            InputMgr.Clear();
        }

        private void CleanObjects() 
        {
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

            objects.Add(obj);
        }

        /// <summary>
        /// bezpecne prida objekt (SceneObject i gui objekt) v dalsim updatu
        /// </summary>
        public void DelayedAttachToScene(ISceneObject obj)
        {
                objectsToAdd.Add(obj);
                obj.OnAttach();
        }

        /// <summary>
        /// bezpecne odstrani objekt (SceneObject i gui objekt) v dalsim updatu
        /// </summary>
        public void RemoveFromSceneDelayed(ISceneObject obj)
        {
            obj.Dead = true;
            obj.OnRemove();
            objectsToRemove.Add(obj);
        }

        /// <summary>
        /// ihned odebere objekt ze sceny
        /// </summary>
        private void DirectRemoveFromScene(ISceneObject obj)
        {
            objects.Remove(obj);
        }

        private void RemoveObjectsMarkedForRemoval()
        {
            ISceneObject obj;
            for (int i = 0; i < objectsToRemove.Count; ++i)
            {
                obj = objectsToRemove[i];
                DirectRemoveFromScene(obj);
            }

            objectsToRemove.Clear();
        }

        /************************************************************************/
        /* konec manipulace s objekty                                           */
        /************************************************************************/

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

            totalTime += tpf;

            ShowStatistics(tpf);

            StateMgr.Update(tpf);

            if (!stopUpdating)
            {
                AddObjectsReadyToAdd();

                UpdateSceneObjects(tpf);
                RemoveObjectsMarkedForRemoval();

                CheckCollisions(tpf);
                RemoveObjectsMarkedForRemoval();
            }
        }

        private void ShowStatistics(float tpf)
        {
            statisticsTimer += tpf;
            if (statisticsTimer < 0.5)
                return;

            statisticsTimer = 0;

            ShowStatusText(1, String.Format(Strings.Culture, Strings.misc_fps, (int)(1.0f / tpf)));
            if (GameType != Gametype.SOLO_GAME && serverConnection != null)
                ShowStatusText(2, String.Format(Strings.Culture, Strings.misc_latency, (int)(serverConnection.AverageRoundtripTime * 1000 / 2)));
        }

        private void ProcessActionQueue()
        {
            Action act = null;
            while (synchronizedQueue.TryDequeue(out act))
                act.Invoke();
        }

        private void UpdateGeomtricState()
        {
            for (int i = 0; i < objects.Count; ++i)
                objects[i].UpdateGeometric(spriteBatch);
        }

        public void UpdateSceneObjects(float tpf)
        {
            ISceneObject obj;
            for (int i = 0; i < objects.Count; ++i)
            {
                obj = objects[i];
                obj.Update(tpf);
            }

            for (int i = 0; i < objects.Count; ++i)
            {
                obj = objects[i];
                obj.AfterUpdate(tpf);
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

                ISceneObject obj2 = null;
                for (int i = 0; i < objects.Count; ++i)
                {
                    obj2 = objects[i];

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

        public List<Player> GetPlayers()
        {
            return players;
        }

        public Random GetRandomGenerator()
        {
            return randomGenerator;
        }

        private void ProcessStaticMouseActionBarClick(Vector2 point)
        {
        }

        public static bool IsPointInViewPort(Vector2 point)
        {
            if (point.X > SharedDef.VIEW_PORT_SIZE.Width || point.X < 0)
                return false;

            if (point.Y > SharedDef.VIEW_PORT_SIZE.Height || point.Y < 0)
                return false;

            return true;
        }

        /// <summary>
        /// vola se pri leavnuti hrace, pri vyhre, pri disconnectu od serveru, pri ukonceni turnaje;
        /// vypne uzivatelsky vstup a hru, zkontroluje highscore, ohlasi vysledek hry, zobrazi statistiky a naplanuje zavreni okna hry 
        /// </summary>
        /// <param name="plr">hrac ktery leavnul nebo vyhral</param>
        /// <param name="endType">typ s jakym byly hra ukoncena</param>
        private void EndGame(Player plr, GameEnd endType)
        {
            if (gameEnded && endType != GameEnd.TOURNAMENT_FINISHED)
                return;

            if (Application.Current != null)
                Application.Current.Dispatcher.Invoke(new Action(() => App.Instance.SetGameStarted(false)));

            userActionsDisabled = true;
            gameEnded = true;

            switch (GameType)
            {
                case Gametype.SOLO_GAME:
                    {
                        if (endType == GameEnd.WIN_GAME)
                            CheckHighScore(plr);
                    }
                    break;
                case Gametype.MULTIPLAYER_GAME:
                    {
                        if (endType == GameEnd.WIN_GAME)
                            CheckHighScore(plr);
                    }
                    break;
                case Gametype.TOURNAMENT_GAME:
                    {

                    }
                    break;
            }

            players.ForEach(p => p.Statistics.GameEnded = true);

            if (endType == GameEnd.LEFT_GAME)
            {
                PlayerLeft(plr);
                if (!plr.IsActivePlayer())
                    return;
            }
            else if (endType == GameEnd.SERVER_DISCONNECTED)
                Disconnected();

            if (GameWindowState == WindowState.IN_LOBBY && endType != GameEnd.TOURNAMENT_FINISHED || !IsGameInitalized)
                CloseGameWindowAndCleanup(endType, true);
            else
                ShowEndGameStats(endType);
        }

        /// <summary>
        /// Vytvori a zobrazi okno se statistikami za posledni hru a nastavi akci, ktera se ma provest po zavreni statistik
        /// </summary>
        /// <param name="endType">typ konce hry</param>
        private void ShowEndGameStats(GameEnd endType)
        {
            //zrusime static mouse a zabranime dalsimu update - hrac pozna ze je konec
            StaticMouse.Enable(false);
            stopUpdating = true;

            if (endType != GameEnd.TOURNAMENT_FINISHED)
                CloseGameWindowAndCleanup(endType);
            else
                TournamentFinished();
        }

        public void CloseGameWindowAndCleanup(GameEnd endType, bool forceQuit = false)
        {
            if (GameType != Gametype.TOURNAMENT_GAME || endType == GameEnd.SERVER_DISCONNECTED || endType == GameEnd.TOURNAMENT_FINISHED)
                RequestStop();

            gameRunning = false;

            StateMgr.Clear();

            if (Application.Current == null)
                return;

            if (forceQuit)
                NormalGameEnded();
            else if (GameType == Gametype.TOURNAMENT_GAME && endType != GameEnd.SERVER_DISCONNECTED && endType != GameEnd.TOURNAMENT_FINISHED)
                TournamentGameEnded();
            else if (endType != GameEnd.TOURNAMENT_FINISHED)
                NormalGameEnded();
        }

        public void PlayerQuitGame()
        {
            playerQuit = true;
            SendChatMessage(String.Format(Strings.Culture, Strings.lobby_left, GetCurrentPlayer().Data.Name), true);
            serverConnection.Disconnect(Strings.networking_server_quit);
        }

        /// <summary>
        /// zkontroluje, jestli dal hrac highscore a bud zobrazi hlasku s novym highscore nebo jen hlasku o vyhre/prohre;
        /// kontroluje hisghscore jen pro solo a quick game
        /// </summary>
        /// <param name="winner">hrac, ktery vyhral</param>
        private void CheckHighScore(Player winner)
        {
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

            int hs = int.Parse(GameProperties.Props.Get(key), CultureInfo.InvariantCulture);
            if (hs < currentPlayer.Data.MatchPoints)
            {
                hs = currentPlayer.Data.MatchPoints;
                GameProperties.Props.SetAndSave(key, hs);
            }
        }

        public void TournamentFinished()
        {
            if (Application.Current == null)
                return;

            StaticMouse.Enable(false);

            List<LobbyPlayerData> data = CreateLobbyPlayerData();

            RequestStop();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                App.Instance.CreateScoreboardGui(data);
            }));
        }

        private void TournamentGameEnded()
        {
            gameEnded = false;
            isGameInitialized = false;
            userActionsDisabled = true;

            CleanObjects();

            foreach (Player p in players)
            {
                p.ClearActions();
                p.ClearObjects();
            }

            AttachStateManagers();
            stopUpdating = false;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                App.Instance.CreateLobbyGui(currentPlayer.Data.LobbyLeader);
                LobbyUC lobby = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "lobbyWindow") as LobbyUC;
                if (lobby != null)
                    lobby.UpdateTournamentSettings(lastTournamentSettings);
            }));

            SendChatMessage(String.Format(Strings.Culture, Strings.lobby_joined, GetCurrentPlayer().Data.Name), true);
            SendPlayerDataRequestMessage();
            
            if (currentPlayer.Data.LobbyLeader)
            {
                currentPlayer.Data.LobbyReady = true;
                SendPlayerReadyMessage(true);
            }
        }

        private void NormalGameEnded()
        {
            RequestStop();
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                App.Instance.GameEnded();
            }));
        }

        private void Disconnected()
        {
            if (stopUpdating)
                return;

            string msg;
            if (!playerQuit)
                msg = Strings.networking_disconnected;
            else
                msg = Strings.game_end;

        }

        private void PlayerLeft(Player leaver)
        {
            if (leaver == null)
                return;

            if (GameWindowState == WindowState.IN_LOBBY)
                ShowChatMessage(String.Format(Strings.Culture, Strings.game_left, leaver.Data.Name));
        }

        public void Invoke(Action a)
        {
            Dispatcher.CurrentDispatcher.Invoke(a);
        }

        public void BeginInvoke(Action a)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(a);
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

            foreach (ISceneObject obj in objects) 
            {
                if (obj.GetType().IsAssignableFrom(clazz))
                    temp.Add(obj);
            }

            return temp;
        }

        public void SendChatMessage(string message, bool withoutPlayerName = false)
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.CHAT_MESSAGE);
            if (!withoutPlayerName)
                message = currentPlayer.Data.Name + ": " + message;
            msg.Write(message);
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
                {
                    lobby.UpdateShownPlayers(data);
                    lobby.SetIsLeader(currentPlayer.Data.LobbyLeader);
                }
            }));
        }

        private List<LobbyPlayerData> CreateLobbyPlayerData()
        {
            List<LobbyPlayerData> data = new List<LobbyPlayerData>();
            players.ForEach(p => data.Add(new LobbyPlayerData(p.Data.Id, p.Data.Name, p.Data.Score, p.GetId() == GetCurrentPlayer().GetId(), 
                p.Data.LobbyLeader, p.Data.LobbyReady, p.Data.PlayedMatches, p.Data.WonMatches, p.Data.PlayerColor.ToWindowsColor())));
            return data;
        }

        public Player GetOtherActivePlayer(int firstPlayerId)
        {
            return players.Find(p => p.IsActivePlayer() && p.GetId() != firstPlayerId);
        }

        /* TODO predelat
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
            keyListeners.ForEach(l => l.OnKeyEvent(e));
        }
         */

        private List<PlayerOverviewData> GetPlayerOverviewData()
        {
            List<PlayerOverviewData> data = new List<PlayerOverviewData>(players.Count);
            foreach (Player p in players)
            {
                if (p.IsActivePlayer())
                    data.Add(new PlayerOverviewData(p.Data.Name, p.Data.MatchPoints, p.Data.Gold, p.Data.Active, p.Data.PlayedMatches, p.Data.WonMatches,
                        p.Mine.UpgradeLevel, p.Canoon.UpgradeLevel, p.Hook.UpgradeLevel));
                else
                    data.Add(new PlayerOverviewData(p.Data.Name, p.Data.MatchPoints, p.Data.Gold, p.Data.Active, p.Data.PlayedMatches, p.Data.WonMatches,
                        UpgradeLevel.LEVEL_NONE, UpgradeLevel.LEVEL_NONE, UpgradeLevel.LEVEL_NONE));
            }
            return data;
        }

        public void PlayerColorChanged()
        {
            /* TODO doresit
            System.Windows.Media.Color newColor = Player.GetChosenColor();
            if (players == null || players.Exists(p => p.GetPlayerColor() == newColor))
                return;

            GetCurrentPlayer().Data.PlayerColor = Player.GetChosenColor();

            if (GameWindowState == WindowState.IN_LOBBY)
            {
                SendPlayerColorChanged();
                UpdateLobbyPlayers();
            }
             */
        }

        public void ShowPlayerOverview()
        {
            List<PlayerOverviewData> data = GetPlayerOverviewData();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                App.Instance.ShowGameOverview(data);
            }));
        }

        public List<ISceneObject> GetSceneObjectsInDist<T>(Vector2 position, double radius)
        {
            List<ISceneObject> found = new List<ISceneObject>();
            objects.ForEach(o => { if (o is T && (o.Center - position).Length() <= radius) found.Add(o); });
            return found;
        }
    }
}

