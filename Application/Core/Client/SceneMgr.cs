﻿using System;
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

namespace Orbit.Core.Client
{
    public partial class SceneMgr
    {
        public Gametype GameType { get; set; }
        public FloatingTextManager FloatingTextMgr { get; set; }

        /// <summary>
        /// canvas je velky 1000*700 - pres cele okno
        /// </summary>
        private Canvas canvas;
        private bool isGameInitialized;
        private bool userActionsDisabled;
        private volatile bool shouldQuit;
        private List<ISceneObject> objects;
        private List<ISceneObject> objectsToRemove;
        private List<ISceneObject> objectsToAdd;
        private List<Player> players;
        private Player currentPlayer;
        private Random randomGenerator;
        private ConcurrentQueue<Action> synchronizedQueue;
        private bool gameEnded;
        private float statisticsTimer;
        private PlayerActionManager actionMgr;
        public GameStateManager StateMgr { get; set; }

        public SceneMgr()
        {
            isGameInitialized = false;
            shouldQuit = false;
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
            StateMgr = new GameStateManager();

            currentPlayer = CreatePlayer();
            players.Add(currentPlayer);
            StateMgr.AddGameState(currentPlayer);
            FloatingTextMgr = new FloatingTextManager(this);
            StateMgr.AddGameState(FloatingTextMgr);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                currentPlayer.Data.Name = (Application.Current as App).PlayerName;
                currentPlayer.Data.HashId = (Application.Current as App).PlayerHashId;
            }));

            if (gameType != Gametype.TOURNAMENT_GAME)
            {
                actionMgr = new PlayerActionManager(this);
                StateMgr.AddGameState(actionMgr);
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
            shouldQuit = false;

            if (client != null && client.Status != NetPeerStatus.NotRunning)
            {
                client.Shutdown("Peer closed connection");
                Thread.Sleep(10); // networking threadu chvili trva ukonceni
            }

            objectsToRemove.Clear();
            objects.Clear();
            objectsToAdd.Clear();

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

            long elapsedMs = 0;
            while (!shouldQuit)
            {
                tpf = sw.ElapsedMilliseconds / 1000.0f;
                
                sw.Restart();

                ProcessMessages();

                if (Application.Current == null)
                    return;

                ProcessActionQueue();

                if (tpf >= 0.001 && isGameInitialized)
                    Update(tpf);

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
            ShowStatistics(tpf);

            AddObjectsReadyToAdd();

            StateMgr.Update(tpf);

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
                if (!obj.IsOnScreen(SharedDef.VIEW_PORT_SIZE))
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
        }

        public void OnViewPortChange(Size size)
        {
            
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
            if (userActionsDisabled)
                return;


            if (StaticMouse.Instance != null && StaticMouse.ALLOWED)
                point = StaticMouse.GetPosition();

            if((e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed) &&
                (!IsPointInViewPort(point) && StaticMouse.Instance != null && StaticMouse.ALLOWED))
            {
                ProcessStaticMouseActionBarClick(point);
                return;
            }
            
            currentPlayer.Mine.ProccessClickEvent(point, e.ChangedButton, e.ButtonState);
            currentPlayer.Hook.ProccessClickEvent(point, e.ChangedButton, e.ButtonState);
            currentPlayer.Canoon.ProccessClickEvent(point, e.ChangedButton, e.ButtonState);
        }

        private void ProcessStaticMouseActionBarClick(Point point)
        {
            Invoke(new Action(() =>
            {
                actionMgr.ActionBar.OnClick(canvas.PointToScreen(point));
            }));
        }

        private bool IsPointInViewPort(Point point)
        {
            if (point.X > SharedDef.VIEW_PORT_SIZE.Width || point.X < 0)
                return false;

            if (point.Y > SharedDef.VIEW_PORT_SIZE.Height || point.Y < 0)
                return false;

            return true;
        }

        private void EndGame(Player plr, GameEnd endType)
        {
            if (gameEnded)
                return;

            if (Application.Current != null)
                (Application.Current as App).setGameStarted(false);

            isGameInitialized = true;
            gameEnded = true;
            if (endType == GameEnd.WIN_GAME)
                PlayerWon(plr);
            else if (endType == GameEnd.LEFT_GAME)
                PlayerLeft(plr);
            else if (endType == GameEnd.SERVER_DISCONNECTED)
                Disconnected();
            else if (endType == GameEnd.TOURNAMENT_FINISHED)
                TournamenFinished(plr);

            if (GameType != Gametype.TOURNAMENT_GAME || endType == GameEnd.SERVER_DISCONNECTED || endType == GameEnd.TOURNAMENT_FINISHED)
                RequestStop();

            StaticMouse.Enable(false);

            Thread.Sleep(3000);

            if (Application.Current == null)
                return;

            if (GameType == Gametype.TOURNAMENT_GAME && endType != GameEnd.SERVER_DISCONNECTED && endType != GameEnd.TOURNAMENT_FINISHED)
                TournamentGameEnded();
            else if (endType != GameEnd.TOURNAMENT_FINISHED)
                NormalGameEnded();
        }

        private void TournamenFinished(Player winner)
        {
            if (Application.Current == null)
                return;

            List<LobbyPlayerData> data = new List<LobbyPlayerData>();
            players.ForEach(p => data.Add(new LobbyPlayerData(p.Data.Id, p.Data.Name, p.Data.Score, p.Data.LobbyLeader, p.Data.PlayedMatches, p.Data.WonMatches)));

            LobbyPlayerData winnerData = data.Find(d => d.Id == winner.Data.Id);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                (Application.Current as App).CreateScoreboardGui(winnerData, data);
            }));
        }

        private void TournamentGameEnded()
        {
            gameEnded = false;
            isGameInitialized = false;
            userActionsDisabled = true;
            objects.Clear();
            objectsToRemove.Clear();
            objectsToAdd.Clear();

            players.ForEach(p => p.Data.LobbyReady = false);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                (Application.Current as App).CreateLobbyGui(currentPlayer.Data.LobbyLeader);
            }));

            SendPlayerDataRequestMessage();

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
                (Application.Current as App).GameEnded();
                //(Application.Current as App).ExitGame();
            }));
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

            List<LobbyPlayerData> data = new List<LobbyPlayerData>();
            players.ForEach(p => data.Add(new LobbyPlayerData(p.Data.Id, p.Data.Name, p.Data.Score, p.Data.LobbyLeader, p.Data.PlayedMatches, p.Data.WonMatches)));

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                LobbyUC lobby = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "lobbyWindow") as LobbyUC;
                if (lobby != null)
                    lobby.UpdateShownPlayers(data);
            }));
        }

        public Player GetOtherActivePlayer(int firstPlayerId)
        {
            return players.Find(p => p.IsActivePlayer() && p.GetId() != firstPlayerId);
        }
    }
}
