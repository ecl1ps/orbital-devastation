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

namespace Orbit.Core.Scene
{
    public partial class SceneMgr
    {
        public Gametype GameType { get; set; }
        public Size ViewPortSizeOriginal { get; set; }

        private Canvas canvas;
        private bool isInitialized;
        private bool userActionsDisabled;
        private volatile bool shouldQuit;
        private List<ISceneObject> objects;
        private List<ISceneObject> objectsToRemove;
        private List<Player> players;
        private Player currentPlayer;
        private Rect orbitArea;
        private Random randomGenerator;
        private ConcurrentQueue<Action> synchronizedQueue;
        private bool gameEnded;
        private float statisticsTimer;

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

            Invoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblEndGame");
                if (lbl != null)
                    lbl.Content = "";

                if (gameType != Gametype.CLIENT_GAME)
                {
                    Label lbl1 = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblIntegrityLeft");
                    if (lbl1 != null)
                        lbl1.Content = 100 + "%";

                    Label lbl2 = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblIntegrityRight");
                    if (lbl2 != null)
                        lbl2.Content = 100 + "%";
                }

                Label lblw = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblWaiting");
                if (lblw != null)
                    lblw.Content = "";
                
            }));

            if (gameType == Gametype.SERVER_GAME)
            {
                userActionsDisabled = true;
                SetMainInfoText("Waiting for the other player to connect");
                ShowStatusText(3, "You are " + (players[0].GetPlayerColor() == Colors.Red ? "Red" : "Blue"));
                InitNetwork();
                isInitialized = false;
            }
            else if (gameType == Gametype.CLIENT_GAME)
            {
                userActionsDisabled = true;
                SetMainInfoText("Waiting for the server");
                InitNetwork();
            }
            else /*Gametype.SOLO_GAME*/
            {
                userActionsDisabled = false;
                isInitialized = true;
            }
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
            if (client != null && client.Status != NetPeerStatus.NotRunning)
            {
                client.Shutdown("Peer closed connection");
                Thread.Sleep(10); // networking threadu chvili trva ukonceni
            }

            objectsToRemove.Clear();

            Invoke(new Action(() =>
            {
                foreach (ISceneObject obj in objects)
                    canvas.Children.Remove(obj.GetGeometry());
            }));
        }

        public void AttachToScene(ISceneObject obj, bool asNonInteractive = false)
        {
            if (!asNonInteractive)
                objects.Add(obj);

            BeginInvoke(new Action(() =>
            {
                canvas.Children.Add(obj.GetGeometry());
            }));
        }

        public void AttachGraphicalObjectToScene(UIElement obj)
        {
            BeginInvoke(new Action(() =>
            {
                canvas.Children.Add(obj);
            }));
        }

        public void RemoveGraphicalObjectFromScene(UIElement obj)
        {
            BeginInvoke(new Action(() =>
            {
                canvas.Children.Remove(obj);
            }));
        }

        public Canvas GetCanvas()
        {
            return canvas;
        }

        public void RemoveFromSceneDelayed(ISceneObject obj)
        {
            obj.Dead = true;
            objectsToRemove.Add(obj);
        }

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

            ProcessActionQueue();

            UpdateSceneObjects(tpf);
            RemoveObjectsMarkedForRemoval();

            CheckCollisions();
            RemoveObjectsMarkedForRemoval();

            GetCurrentPlayer().Update(tpf);

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
            if (GameType != Gametype.SOLO_GAME)
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

        public void OnCanvasClick(Point point, MouseButtonEventArgs e)
        {
            if (userActionsDisabled)
                return;

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    GetCurrentPlayer().Data.Mine.Shoot(point);
                    break;
                case MouseButton.Right:
                    GetCurrentPlayer().Data.Canoon.Shoot(point);
                    break;
                case MouseButton.Middle:
                    GetCurrentPlayer().Data.Hook.Shoot(point);
                    break;
            }          
        }

        private void PlayerWon(Player winner)
        {
            Invoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblEndGame");
                if (lbl != null)
                    lbl.Content = (winner.Data.PlayerColor == Colors.Red ? "Red" : "Blue") + " player win!";
            }));
        }


        private void PlayerLeft(Player leaver)
        {
            if (leaver == null)
                return;

            Invoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblEndGame");
                if (lbl != null)
                    lbl.Content = (leaver.Data.PlayerColor == Colors.Red ? "Red" : "Blue") + " player left the game!";
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
    }
}
