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
using Microsoft.Xna.Framework;

namespace Orbit.Core.AI
{
    /// <summary>
    /// bot urovne 1 - healuje se, strili nahodne z kanonu a pousti nahodne miny nad nepritelem
    /// na obou zbranich ma zpomaleni 2.5*
    /// </summary>
    public class SimpleBot : IGameState
    {
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
            if (me.Data.BaseIntegrity + me.HealingKit.GetHealAmount() <= me.Data.MaxBaseIntegrity &&
                me.Data.Gold >= me.HealingKit.Cost)
            {
                me.Data.Gold -= me.HealingKit.Cost;
                me.HealingKit.Heal();
            }
        }

        private void MineDrop()
        {
            Rectangle opponentLoc = PlayerBaseLocation.GetBaseLocation(me.GetPosition() == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT);
            double xMin = opponentLoc.X;
            double xMax = opponentLoc.X + opponentLoc.Width;
            me.Mine.Shoot(new Vector2(sceneMgr.GetRandomGenerator().Next((int)xMin, (int)xMax), 1));
 
        }

        private void CannonShoot()
        {
            me.Canoon.Shoot(new Vector2(sceneMgr.GetRandomGenerator().Next(0, (int)SharedDef.VIEW_PORT_SIZE.Width), 0));
        }
    }
}
