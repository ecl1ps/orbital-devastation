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

namespace Orbit.Core.Scene.Entities.Implementations
{
    class UnstableAsteroid : Asteroid
    {
        public int Destroyer { get; set; }
        private int childsDestroyed = 0;
        private bool allDestroyedByTheSamePlayer = true;

        public UnstableAsteroid(SceneMgr mgr) : base(mgr)
        {
            Destroyer = -1;
        }

        private void SpawnSmallMeteors(int radius)
        {
            if (SceneMgr.GetPlayer(Destroyer) == null || !SceneMgr.GetPlayer(Destroyer).IsCurrentPlayerOrBot())
                return; 

            int rotation = SceneMgr.GetRandomGenerator().Next(360);
            int textureId = SceneMgr.GetRandomGenerator().Next(1, 18);

            long id1 = IdMgr.GetNewId(SceneMgr.GetCurrentPlayer().GetId());
            long id2 = IdMgr.GetNewId(SceneMgr.GetCurrentPlayer().GetId());
            long id3 = IdMgr.GetNewId(SceneMgr.GetCurrentPlayer().GetId());

            MinorAsteroid a1 = SceneObjectFactory.CreateSmallAsteroid(SceneMgr, id1, Direction, Center, rotation, textureId, radius, Math.PI / 12);
            MinorAsteroid a2 = SceneObjectFactory.CreateSmallAsteroid(SceneMgr, id2, Direction, Center, rotation, textureId, radius, 0);
            MinorAsteroid a3 = SceneObjectFactory.CreateSmallAsteroid(SceneMgr, id3, Direction, Center, rotation, textureId, radius, -Math.PI / 12);

            a1.Parent = this;
            a2.Parent = this;
            a3.Parent = this;

            SceneMgr.DelayedAttachToScene(a1);
            SceneMgr.DelayedAttachToScene(a2);
            SceneMgr.DelayedAttachToScene(a3);

            NetOutgoingMessage message = SceneMgr.CreateNetMessage();
            message.Write((int)PacketType.MINOR_ASTEROID_SPAWN);
            message.Write(radius);
            message.Write(Direction);
            message.Write(Center);
            message.Write(rotation);
            message.Write(textureId);
            message.Write(Destroyer);
            message.Write(id1);
            message.Write(id2);
            message.Write(id3);

            SceneMgr.SendMessage(message);
        }

        public override void TakeDamage(int damage, ISceneObject from)
        {
            if (from is IProjectile)
                Destroyer = (from as IProjectile).Owner.GetId();

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
                SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.CANNON_DESTROYED_ENTIRE_UNSTABLE_ASTEROID, destroyedChild.Center,
                    FloatingTextManager.TIME_LENGTH_4, FloatingTextType.SCORE, FloatingTextManager.SIZE_BIG, false, true);

                if (SceneMgr.GetPlayer(Destroyer).IsCurrentPlayerOrBot())
                    SceneMgr.GetPlayer(Destroyer).AddScoreAndShow(ScoreDefines.CANNON_DESTROYED_ENTIRE_UNSTABLE_ASTEROID);
            }
        }
    }
}
