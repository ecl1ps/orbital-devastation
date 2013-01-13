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
using Lidgren.Network;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class UnstableAsteroid : Asteroid
    {
        public int Destroyer { get; set; }
        private int childsDestroyed = 0;
        private bool allDestroyedByTheSamePlayer = true;

        public UnstableAsteroid(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Destroyer = -1;
        }

        private void SpawnSmallMeteors(int radius)
        {
            if (SceneMgr.GetPlayer(Destroyer) == null || !SceneMgr.GetPlayer(Destroyer).IsCurrentPlayerOrBot())
                return; 

            int rotation = SceneMgr.GetRandomGenerator().Next(360);
            int textureId = SceneMgr.GetRandomGenerator().Next(1, 18);
            float speed = GetControlOfType<NewtonianMovementControl>().Speed / 2;

            MinorAsteroid a1 = SceneObjectFactory.CreateSmallAsteroid(SceneMgr, Direction, Center, rotation, textureId, radius, speed, Math.PI / 12);
            MinorAsteroid a2 = SceneObjectFactory.CreateSmallAsteroid(SceneMgr, Direction, Center, rotation, textureId, radius, speed, 0);
            MinorAsteroid a3 = SceneObjectFactory.CreateSmallAsteroid(SceneMgr, Direction, Center, rotation, textureId, radius, speed, -Math.PI / 12);

            a1.Parent = this;
            a2.Parent = this;
            a3.Parent = this;

            SceneMgr.DelayedAttachToScene(a1);
            SceneMgr.DelayedAttachToScene(a2);
            SceneMgr.DelayedAttachToScene(a3);

            NetOutgoingMessage message = SceneMgr.CreateNetMessage();
            message.Write((int)PacketType.MINOR_ASTEROID_SPAWN);
            message.Write(speed);
            message.Write(radius);
            message.Write(Direction);
            message.Write(Center);
            message.Write(rotation);
            message.Write(textureId);
            message.Write(Destroyer);
            message.Write(a1.Id);
            message.Write(a2.Id);
            message.Write(a3.Id);
            message.Write(Id);

            SceneMgr.SendMessage(message);
        }

        public override void TakeDamage(int damage, ISceneObject from)
        {
            if (from is IProjectile)
                Destroyer = (from as IProjectile).Owner.GetId();
            else if (from is MiningModule)
                Destroyer = (from as MiningModule).Owner.GetId();

            base.TakeDamage(damage, from);
            DoRemoveMe();
            SpawnSmallMeteors((int)(Radius * 0.7f));
        }

        /// <summary>
        /// kontroluje extra score za zniceni vsech potomku jednim hracem
        /// </summary>
        public void NoticeChildAsteroidDestroyedBy(Player lastHitTakenFrom, MinorAsteroid destroyedChild)
        {
            if (Destroyer == -1)
                return;

            childsDestroyed++;
            if (allDestroyedByTheSamePlayer)
                if (lastHitTakenFrom == null || lastHitTakenFrom.GetId() != Destroyer)
                    allDestroyedByTheSamePlayer = false;

            if (childsDestroyed == 3 && allDestroyedByTheSamePlayer)
            {
                if (SceneMgr.GetPlayer(Destroyer).IsCurrentPlayer())
                    SceneMgr.FloatingTextMgr.AddFloatingText("Tripple wiped!", destroyedChild.Center,
                        FloatingTextManager.TIME_LENGTH_4, FloatingTextType.SCORE, FloatingTextManager.SIZE_BIG, false, true);

                if (SceneMgr.GetPlayer(Destroyer).IsCurrentPlayerOrBot())
                    SceneMgr.GetPlayer(Destroyer).AddScoreAndShow(ScoreDefines.CANNON_DESTROYED_ENTIRE_UNSTABLE_ASTEROID);
            }
        }
    }
}
