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
        private SceneMgr sceneMgr;
        private List<ISceneObject> objects;
        private Player me;

        public SimpleBot(SceneMgr mgr, List<ISceneObject> objects, Player bot)
        {
            sceneMgr = mgr;
            this.objects = objects;
            me = bot;

            me.Data.MineCooldown = me.Data.MineCooldown * 2.5f;
            me.Data.BulletCooldown = me.Data.BulletCooldown * 2.5f;
        }

        public void Update(float tpf)
        {
            me.Canoon.UpdateTimer(tpf);
            me.Mine.UpdateTimer(tpf);

            if (me.Canoon.IsReady())
                CannonShoot();

            if (me.Mine.IsReady())
                MineDrop();

            CheckHeal();
        }

        private void CheckHeal()
        {
            if (me.Data.BaseIntegrity <= SharedDef.BASE_MAX_INGERITY - SharedDef.HEAL_AMOUNT)
                me.HealingKit.Heal();
        }

        private void MineDrop()
        {
            double xMin = 0, xMax = 0;
            if (me.GetPosition() == PlayerPosition.RIGHT)
            {
                xMin = sceneMgr.ViewPortSize.Width * 0.1;
                xMax = sceneMgr.ViewPortSize.Width * 0.4;
            }
            else
            {
                xMin = sceneMgr.ViewPortSize.Width * 0.6;
                xMax = sceneMgr.ViewPortSize.Width * 0.9;
            }
            me.Mine.Shoot(new Point(sceneMgr.GetRandomGenerator().Next((int)xMin, (int)xMax), 1));
 
        }

        private void CannonShoot()
        {
            double xMin = 0, xMax = 0;
            if (me.GetPosition() == PlayerPosition.LEFT)
            {
                xMin = sceneMgr.ViewPortSize.Width * 0.25;
                xMax = sceneMgr.ViewPortSize.Width;
            }
            else
            {
                xMin = 0;
                xMax = sceneMgr.ViewPortSize.Width * 0.75;
            }
            me.Canoon.Shoot(new Point(sceneMgr.GetRandomGenerator().Next((int)xMin, (int)xMax), 0));
        }
    }
}
