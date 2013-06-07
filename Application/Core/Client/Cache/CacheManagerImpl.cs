using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Client.Cache
{
    public class CacheManagerImpl : CacheManager
    {
        public CacheManagerImpl(SceneMgr SceneMgr, LoadingScreen screen)
            : base(SceneMgr, screen)
        {
        }

        public void RegisterPermGolden()
        {
            LoadAsteroids(AsteroidType.UNSTABLE, 5);
        }

        public void RegisterPermUnstable()
        {
            LoadAsteroids(AsteroidType.GOLDEN, 5);
        }

        public void RegisterPermNormal()
        {
            LoadAsteroids(AsteroidType.NORMAL, 17);
        }

        private void LoadAsteroids(AsteroidType type, int num)
        {
            SceneMgr.Invoke(new Action(() =>
            {
                for (int i = 1; i <= num; i++)
                    Register(SceneGeometryFactory.CreateAsteroidImage(type, i), true, type.ToString() + i);
            }));
        }
    }
}
