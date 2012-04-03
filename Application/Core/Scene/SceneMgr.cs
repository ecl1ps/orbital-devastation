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
        private PlayerPosition me;
        private Rect actionArea;
        private Random randomGenerator;
        public Size ViewPortSizeOriginal { get; set; }
        public Size ViewPortSize { get; set; }
        private const long MINIMUM_UPDATE_TIME = 30;
        //private int isStarted;

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
            shouldQuit = false;
            objects = new List<ISceneObject>();
            objectsToRemove = new List<ISceneObject>();
            Random r = new Random();
            me = r.Next(2) == 0 ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            playerData = new Dictionary<PlayerPosition, IPlayerData>(2);
            randomGenerator = new Random();
        }

        public void Init()
        {
            InitPlayerData();

            Sphere s;
            for (int i = 0; i < SharedDef.SPHERE_COUNT; ++i)
            {
                s = SceneObjectFactory.CreateNewRandomSphere(i % 2 == 0);
                AttachToScene(s);
            }
        }

        public void AttachToScene(ISceneObject obj)
        {
            objects.Add(obj);
            canvas.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
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
            Base myBase = SceneObjectFactory.CreateBase(me, randomGenerator.Next(2) == 0 ? Colors.Red : Colors.Blue);
            AttachToScene(myBase);

            PlayerData pd = new PlayerData();
            pd.SetBase(myBase);
            playerData.Add(pd.GetPosition(), pd);

            Base opponentsBase = SceneObjectFactory.CreateBase(myBase.BasePosition == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT,
                                            myBase.Color == Colors.Blue ? Colors.Red : Colors.Blue);
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
                Update(tpf);

                //Console.Out.WriteLine(sw.ElapsedMilliseconds);

		        if (sw.ElapsedMilliseconds < MINIMUM_UPDATE_TIME) 
                {
				    Thread.Sleep((int)(MINIMUM_UPDATE_TIME - sw.ElapsedMilliseconds));
		        }
            }

            sw.Stop();                
        }

        public void RequestStop()
        {
            shouldQuit = true;
        }

        public void Update(float tpf)
        {
            UpdateSceneObjects(tpf);
            RemoveObjectsMarkedForRemoval();

            CheckCollisions();
            RemoveObjectsMarkedForRemoval();

            UpdateGeomtricState();
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
            catch (ArgumentNullException e)
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

        public Dispatcher GetCanvasDispatcher()
        {
            return canvas.Dispatcher;
        }

        public void OnViewPortChange(Size size)
        {
            ViewPortSize = size;
            canvas.RenderTransform = new ScaleTransform(size.Width / ViewPortSizeOriginal.Width, size.Height / ViewPortSizeOriginal.Height);
        }

        public void SetAsServer(bool isServer)
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
    }

}
