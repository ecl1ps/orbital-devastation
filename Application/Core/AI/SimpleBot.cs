using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Weapons;
using System.Windows;
using Orbit.Core.Players;

namespace Orbit.Core.AI
{
    /// <summary>
    /// bot urovne 1 - healuje se, strili nahodne z kanonu a pousti nahodne miny nad nepritelem
    /// na obou zbranich ma zpomaleni 2.5*
    /// </summary>
    public class SimpleBot : IGameState
    {
        public const string NAME = "SimpleBot";

        private SceneMgr sceneMgr;
        private Player me;

        public SimpleBot(SceneMgr mgr, List<ISceneObject> objects, Player bot)
        {
            sceneMgr = mgr;
            me = bot;

            me.Data.MineCooldown = me.Data.MineCooldown * 2.5f;
            me.Data.BulletCooldown = me.Data.BulletCooldown * 2.5f;
        }

        public void Update(float tpf)
        {
            if (me.Canoon.IsReady())
                CannonShoot();

            if (me.Mine.IsReady())
                MineDrop();

            CheckHeal();
        }

        private void CheckHeal()
        {
            if (me.Data.BaseIntegrity <= SharedDef.BASE_MAX_INGERITY - ((SharedDef.BASE_MAX_INGERITY * SharedDef.HEAL_AMOUNT) + SharedDef.BONUS_HEAL) &&
                me.Data.Gold >= me.HealingKit.Cost)
            {
                me.Data.Gold -= me.HealingKit.Cost;
                me.HealingKit.Heal();
            }
        }

        private void MineDrop()
        {
            Rect opponentLoc = PlayerBaseLocation.GetBaseLocation(me.GetPosition() == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT);
            double xMin = opponentLoc.X;
            double xMax = opponentLoc.X + opponentLoc.Width;
            me.Mine.Shoot(new Point(sceneMgr.GetRandomGenerator().Next((int)xMin, (int)xMax), 1));
 
        }

        private void CannonShoot()
        {
            double xMin = 0, xMax = 0;
            if (me.GetPosition() == PlayerPosition.LEFT)
            {
                xMin = SharedDef.VIEW_PORT_SIZE.Width * 0.25;
                xMax = SharedDef.VIEW_PORT_SIZE.Width;
            }
            else
            {
                xMin = 0;
                xMax = SharedDef.VIEW_PORT_SIZE.Width * 0.75;
            }
            me.Canoon.Shoot(new Point(sceneMgr.GetRandomGenerator().Next((int)xMin, (int)xMax), 0));
        }
    }
}
