using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene;
using Orbit.Core;
using Orbit.Core.Scene.Controls;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Players;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class UnstableAsteroid : Asteroid
    {
        private Player destroyer;
        private int childsDestroyed = 0;
        private bool allDestroyedByTheSamePlayer = true;

        public UnstableAsteroid(SceneMgr mgr) : base(mgr)
        {
        }

        private void SpawnSmallMeteors(int radius)
        {
            CreateSmallAsteroid(radius, Math.PI / 12);
            CreateSmallAsteroid(radius, 0);
            CreateSmallAsteroid(radius, -Math.PI / 12);
        }

        private void CreateSmallAsteroid(int radius, double rotation)
        {
            MinorAsteroid asteroid = new MinorAsteroid(SceneMgr);
            asteroid.AsteroidType = AsteroidType.SPAWNED;
            asteroid.Id = IdMgr.GetNewId(SceneMgr.GetCurrentPlayer().GetId());
            asteroid.Rotation = SceneMgr.GetRandomGenerator().Next(360);
            asteroid.Direction = Direction.Rotate(rotation);
            asteroid.Radius = radius;
            asteroid.Position = Center;
            asteroid.Gold = radius * 2;
            asteroid.TextureId = SceneMgr.GetRandomGenerator().Next(1, 18);
            asteroid.Enabled = true;
            asteroid.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(asteroid));

            asteroid.Parent = this;

            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.InitialSpeed = 1;
            asteroid.AddControl(nmc);

            LinearRotationControl lrc = new LinearRotationControl();
            lrc.RotationSpeed = SceneMgr.GetRandomGenerator().Next(SharedDef.MIN_ASTEROID_ROTATION_SPEED, SharedDef.MAX_ASTEROID_ROTATION_SPEED) / 10.0f;
            asteroid.AddControl(lrc);

            SceneMgr.DelayedAttachToScene(asteroid);
        }

        public override void TakeDamage(int damage, ISceneObject from)
        {
            // TODO: lepsi pretypovani, je mozne, ze bude vic bulletu
            if (from is SingularityBullet)
                destroyer = (from as SingularityBullet).Owner;
            base.TakeDamage(damage, from);
            DoRemoveMe();
            SpawnSmallMeteors((int)(Radius * 0.7f));
        }

        /// <summary>
        /// kontroluje extra score za zniceni vsech potomku jednim hracem
        /// </summary>
        public void NoticeChildAsteroidDestroyedBy(Player lastHitTakenFrom, MinorAsteroid destroyedChild)
        {
            childsDestroyed++;
            if (allDestroyedByTheSamePlayer)
                if (lastHitTakenFrom == null || lastHitTakenFrom.GetId() != destroyer.GetId())
                    allDestroyedByTheSamePlayer = false;

            if (childsDestroyed == 3 && allDestroyedByTheSamePlayer)
            {
                SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.CANNON_DESTROYED_ENTIRE_UNSTABLE_ASTEROID, destroyedChild.Center,
                    FloatingTextManager.TIME_LENGTH_4, FloatingTextType.SCORE, FloatingTextManager.SIZE_BIG);
                destroyer.AddScoreAndShow(ScoreDefines.CANNON_DESTROYED_ENTIRE_UNSTABLE_ASTEROID);
            }
        }
    }
}
