using System;
using Orbit;
using Orbit.Player;
using Orbit.Scene.Controls;
using Orbit.Scene.Entities;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace Orbit.Core.Scene
{
    public class SceneMgr
    {
        private bool isServer;
        //private NetServer server;
        //private NetClient client;
        private IList<ISceneObject> objects;
        private Dictionary<PlayerPosition, IPlayerData> playerData;
        private PlayerPosition me;
        private Rectangle actionArea;
        private Random randomGenerator;
        //private int isStarted;
        //http://msdn.microsoft.com/en-us/library/system.threading.interlocked.aspx

        public SceneMgr(bool isServer)
        {
            this.isServer = isServer;
            objects = new List<ISceneObject>();
            Random r = new Random();
            me = r.Next(2) == 0 ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            playerData = new Dictionary<PlayerPosition, IPlayerData>(2);
            actionArea = new Rectangle(0, 0, 800, 600);
            randomGenerator = new Random();
        }

        public void Init()
        {
            InitPlayerData();

            Sphere s;
            LinearMovementControl lc;
            for (int i = 0; i < 10; ++i)
            {
                s = new Sphere();
                s.Setid(IdMgr.GetInstance().GetNewId());
                s.SetDirection(new Vector(1, 0));
                s.Radius = (uint)randomGenerator.Next(SharedDef.MAX_SPHERE_RADIUS, SharedDef.MAX_SPHERE_RADIUS);
                s.SetPosition(new Vector(randomGenerator.Next(actionArea.X + (int)s.Radius, actionArea.Width - (int)s.Radius),
                    randomGenerator.Next(actionArea.Y + (int)s.Radius, actionArea.Height - (int)s.Radius)));
                s.Color = Color.FromArgb(randomGenerator.Next(40, 255), randomGenerator.Next(40, 255), randomGenerator.Next(40, 255));

                lc = new LinearMovementControl();
                lc.Speed = randomGenerator.Next(SharedDef.MIN_SPHERE_SPEED, SharedDef.MAX_SPHERE_SPEED);
                s.AddControl(lc);

                objects.Add(s);
            }
        }

        private void InitPlayerData()
        {
            Base myBase = new Base();
            myBase.Setid(IdMgr.GetInstance().GetNewId());
            myBase.BasePosition = me;
            myBase.Color = randomGenerator.Next(2) == 0 ? Color.Red : Color.Blue;
            objects.Add(myBase);

            PlayerData pd = new PlayerData();
            pd.SetBase(myBase);
            playerData.Add(pd.GetPosition(), pd);

            Base opponentsBase = new Base();
            opponentsBase.Setid(IdMgr.GetInstance().GetNewId());
            opponentsBase.BasePosition = myBase.BasePosition == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            opponentsBase.Color = myBase.Color == Color.Blue ? Color.Red : Color.Blue;
            objects.Add(opponentsBase);

            pd = new PlayerData();
            pd.SetBase(opponentsBase);
            playerData.Add(pd.GetPosition(), pd);
        }

        public void Update(float tpf)
        {
            UpdateSceneObjects(tpf);
            CheckCollisions();
            RenderScene();
        }

        public void UpdateSceneObjects(float tpf)
        {
            foreach (ISceneObject obj in objects)
            {
                obj.Update(tpf);
                if (!obj.IsOnScreen())
                    objects.Remove(obj);
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
                    if (!(obj2 is ICollidable))
                        continue;

                    if (((ICollidable)obj1).CollideWith((ICollidable)obj2))
                        ((ICollidable)obj1).DoCollideWith((ICollidable)obj2);
                }
            }
        }

        public void RenderScene()
        {
            foreach (ISceneObject obj in objects)
            {
                obj.Render();
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

    }

}
