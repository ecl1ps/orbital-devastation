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
        private IList<Sphere> removedSpheres;
        private Dictionary<PlayerPosition, IPlayerData> playerData;
        private PlayerPosition me;
        private Rect actionArea;
        private Random randomGenerator;
        public Size ViewPortSize { get; set; }
        private const long MINIMUM_UPDATE_TIME = 30;
        //private int isStarted;

        public SceneMgr(bool isServer)
        {
            shouldQuit = false;
            this.isServer = isServer;
            objects = new List<ISceneObject>();
            removedSpheres = new List<Sphere>();
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
                s = CreateNewRandomSphere(i % 2 == 0);
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

        private Sphere CreateNewRandomSphere(bool headingRight)
        {
            Sphere s = new Sphere();
            s.Setid(IdMgr.GetNewId());
            s.IsHeadingRight = headingRight;
            s.SetDirection(headingRight ? new Vector(1, 0) : new Vector(-1, 0));

            s.Radius = (uint)randomGenerator.Next(SharedDef.MIN_SPHERE_RADIUS, SharedDef.MAX_SPHERE_RADIUS);
            s.SetPosition(new Vector(randomGenerator.Next((int)(actionArea.X + s.Radius), (int)(actionArea.Width - s.Radius)),
                randomGenerator.Next((int)(actionArea.Y + s.Radius), (int)(actionArea.Height - s.Radius))));
            s.Color = Color.FromRgb((byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255), (byte)randomGenerator.Next(40, 255));


            LinearMovementControl lc = new LinearMovementControl();
            lc.Speed = randomGenerator.Next(SharedDef.MIN_SPHERE_SPEED * 10, SharedDef.MAX_SPHERE_SPEED * 10) / 10.0f;
            s.AddControl(lc);

            canvas.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
            {
                EllipseGeometry geom = new EllipseGeometry(s.GetPosition().ToPoint(), s.Radius, s.Radius);
                Path path = new Path();
                path.Data = geom;
                path.Fill = new RadialGradientBrush(s.Color, Color.FromArgb(0x80, s.Color.R, s.Color.G, s.Color.B));
                path.Stroke = Brushes.Black;
                s.SetGeometry(path);
            }));

            return s;
        }


        private Sphere CreateNewSphereOnEdge(Sphere oldSphere)
        {
            Sphere s = CreateNewRandomSphere(oldSphere.IsHeadingRight);

            s.SetPosition(new Vector(s.IsHeadingRight ? (int)(-s.Radius) : (int)(actionArea.Width + s.Radius),
                randomGenerator.Next((int)(actionArea.Y + s.Radius), (int)(actionArea.Height - s.Radius))));

            return s;
        }

        private void InitPlayerData()
        {
            Base myBase = new Base();
            myBase.Setid(IdMgr.GetNewId());
            myBase.BasePosition = me;
            myBase.Color = randomGenerator.Next(2) == 0 ? Colors.Red : Colors.Blue;
            objects.Add(myBase);

            PlayerData pd = new PlayerData();
            pd.SetBase(myBase);
            playerData.Add(pd.GetPosition(), pd);

            Base opponentsBase = new Base();
            opponentsBase.Setid(IdMgr.GetNewId());
            opponentsBase.BasePosition = myBase.BasePosition == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            opponentsBase.Color = myBase.Color == Colors.Blue ? Colors.Red : Colors.Blue;
            objects.Add(opponentsBase);

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

                Console.Out.WriteLine(sw.ElapsedMilliseconds);

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
            //CheckCollisions();
            UpdateGeomtricState();
            //canvas.Refresh();
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
            removedSpheres.Clear();
            for (int i = 0; i < objects.Count; i++)
            {             
                objects[i].Update(tpf);
                if (!objects[i].IsOnScreen(ViewPortSize))
                {
                    RemoveObject(objects[i]);
                    i--;
                }
            }

            foreach (Sphere s in removedSpheres)
                AttachToScene(CreateNewSphereOnEdge(s));
        }

        private void RemoveObject(ISceneObject obj)
        {
            objects.Remove(obj);
            canvas.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(delegate
            {
                canvas.Children.Remove(obj.GetGeometry());
            }));

            if (obj.GetType() == typeof(Sphere))
                removedSpheres.Add(obj as Sphere);
        }

        public void CheckCollisions()
        {
            foreach (ISceneObject obj1 in objects)
            {
                if (!(obj1 is ICollidable))
                    continue;

                foreach (ISceneObject obj2 in objects)
                {
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

        public void ProcessUserInput()
        {
            throw new Exception("Not implemented");
        }

        public void ProcessMessages()
        {
            throw new Exception("Not implemented");
        }

        internal void SetCanvas(Canvas canvas)
        {
            this.canvas = canvas;
            ViewPortSize = new Size(canvas.Width, canvas.Height);
            actionArea = new Rect(0, 0, canvas.Width, canvas.Height / 3);
        }

        internal void OnViewPortChange(Size size)
        {
            ViewPortSize = size;
            actionArea = new Rect(0, 0, size.Width, size.Height / 3);
        }


    }

}
