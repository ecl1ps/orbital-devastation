using System;
using Orbit;
using Orbit.Core.Player;
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

namespace Orbit.Core.Scene
{
    public class SceneMgr
    {
        private Canvas canvas;
        private bool isServer;
        private volatile bool shouldQuit;
        //private NetServer server;
        //private NetClient client;
        private IList<ISceneObject> objects;
        private IList<ISceneObject> objectsToRemove;
        private Dictionary<PlayerPosition, IPlayerData> playerData;
        private PlayerPosition firstPlayer;
        private PlayerPosition secondPlayer;
        private Rect actionArea;
        private Random randomGenerator;
        public Size ViewPortSizeOriginal { get; set; }
        public Size ViewPortSize { get; set; }
        //private int isStarted;
        private ConcurrentQueue<Action> synchronizedQueue;

        private static SceneMgr sceneMgr;
        private static Object lck = new Object();

        public static SceneMgr GetInstance()
        {
            lock (lck)
            {
                if (sceneMgr == null)
                    sceneMgr = new SceneMgr();
                return sceneMgr;
            }
        }

        private SceneMgr()
        {
        }

        public void Init()
        {
            shouldQuit = false;
            objects = new List<ISceneObject>();
            objectsToRemove = new List<ISceneObject>();
            randomGenerator = new Random(Environment.TickCount);
            firstPlayer = randomGenerator.Next(2) == 0 ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            secondPlayer = firstPlayer == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            playerData = new Dictionary<PlayerPosition, IPlayerData>(2);
            synchronizedQueue = new ConcurrentQueue<Action>();

            GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblEndGame");
                if (lbl != null)
                    lbl.Content = "";

                Label lbl1 = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblIntegrityLeft");
                if (lbl1 != null)
                    lbl1.Content = 100 + "%";

                Label lbl2 = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblIntegrityRight");
                if (lbl2 != null)
                    lbl2.Content = 100 + "%";
            }));

            InitPlayerData();

            Sphere s;
            for (int i = 0; i < SharedDef.SPHERE_COUNT; ++i)
            {
                s = SceneObjectFactory.CreateNewRandomSphere(i % 2 == 0);
                AttachToScene(s);
            }
        }

        private void EndGame(IPlayerData winner)
        {
            PlayerWon(winner);
            RequestStop();

            Thread.Sleep(3000);

            CleanUp();
        }

        private void CleanUp()
        {
            objectsToRemove.Clear();

            GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                foreach (ISceneObject obj in objects)
                    canvas.Children.Remove(obj.GetGeometry());
            }));
        }

        public void AttachToScene(ISceneObject obj)
        {
            objects.Add(obj);
            GetUIDispatcher().BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                canvas.Children.Add(obj.GetGeometry());
            }));
        }

        public void RemoveFromSceneDelayed(ISceneObject obj)
        {
            obj.SetDead(true);
            objectsToRemove.Add(obj);
        }

        private void DirectRemoveFromScene(ISceneObject obj)
        {
            objects.Remove(obj);
            canvas.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(delegate
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

        private void InitPlayerData()
        {
            Base myBase = SceneObjectFactory.CreateBase(firstPlayer, randomGenerator.Next(2) == 0 ? Colors.Red : Colors.Blue);
            AttachToScene(myBase);

            PlayerData pd = new PlayerData();
            pd.SetBase(myBase);
            playerData.Add(pd.GetPosition(), pd);

            Base opponentsBase = SceneObjectFactory.CreateBase(secondPlayer, myBase.Color == Colors.Blue ? Colors.Red : Colors.Blue);
            AttachToScene(opponentsBase);

            pd = new PlayerData();
            pd.SetBase(opponentsBase);
            playerData.Add(pd.GetPosition(), pd);
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
                if (tpf > 0.001)
                    Update(tpf);

                //Console.Out.WriteLine(tpf + ": " +sw.ElapsedMilliseconds);

		        if (sw.ElapsedMilliseconds < SharedDef.MINIMUM_UPDATE_TIME) 
                {
                    Thread.Sleep((int)(SharedDef.MINIMUM_UPDATE_TIME - sw.ElapsedMilliseconds));
		        }
            }

            sw.Stop();
            if (Application.Current != null)
                (Application.Current as App).GameEnded();    
        }

        public void RequestStop()
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
            ProcessActionQueue();

            UpdateSceneObjects(tpf);
            RemoveObjectsMarkedForRemoval();

            CheckCollisions();
            RemoveObjectsMarkedForRemoval();

            UpdateGeomtricState();

            CheckPlayerStates();
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
                    if (obj1.GetId() == obj2.GetId())
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

        public IPlayerData GetPlayerData(PlayerPosition pos)
        {
            IPlayerData data = null;
            try
            {
                playerData.TryGetValue(pos, out data);
            }
            catch (ArgumentNullException /*e*/)
            {
                Console.Error.WriteLine("GetPlayerData() - position cannot be null");
            }
            return data;
        }

        public void SetCanvas(Canvas canvas)
        {
            this.canvas = canvas;
            ViewPortSizeOriginal = new Size(canvas.Width, canvas.Height);
            actionArea = new Rect(0, 0, canvas.Width, canvas.Height / 3);
        }

        public Canvas GetCanvas()
        {
            return canvas;
        }

        public Dispatcher GetUIDispatcher()
        {
            return canvas.Dispatcher;
        }

        public void OnViewPortChange(Size size)
        {
            ViewPortSize = size;
            GetUIDispatcher().BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                canvas.RenderTransform = new ScaleTransform(size.Width / ViewPortSizeOriginal.Width, size.Height / ViewPortSizeOriginal.Height);
            }));
        }

        public void SetIsServer(bool isServer)
        {
            this.isServer = isServer;
        }

        public Rect GetActionArea()
        {
            return actionArea;
        }

        public Random GetRandomGenerator()
        {
            return randomGenerator;
        }

        public void OnCanvasClick(Point point)
        {
            AttachToScene(SceneObjectFactory.CreateSingularityMine(point, GetPlayerData(firstPlayer)));
        }

        private void CheckPlayerStates()
        {
            if (GetPlayerData(firstPlayer).GetBaseIntegrity() <= 0)
                EndGame(GetPlayerData(secondPlayer));
            else if (GetPlayerData(secondPlayer).GetBaseIntegrity() <= 0)
                EndGame(GetPlayerData(firstPlayer));
        }

        private void PlayerWon(IPlayerData winner)
        {
            GetUIDispatcher().Invoke(DispatcherPriority.Render, new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblEndGame");
                if (lbl != null)
                    lbl.Content = (winner.GetPlayerColor() == Colors.Red ? "Red" : "Blue") + " player won!";
            }));
        }

    }

}
