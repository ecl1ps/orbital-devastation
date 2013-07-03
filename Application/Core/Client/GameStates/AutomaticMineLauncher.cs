using Microsoft.Xna.Framework;
using Orbit.Core.Client.Interfaces;
using Orbit.Core.Players;
using Orbit.Core.Weapons;
using Orbit.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Orbit.Core.Client.GameStates
{
    public class AutomaticMineLauncher : IGameState, IWeaponClickListener, IMouseMoveListener
    {
        public bool Enable { get; set; }

        private Player owner;
        private MineLauncher weapon;
        private UpgradeLevel lastLevel;

        private double minWidth;
        private double maxWidth;
        private Random rand;

        public float InnerCD;

        private bool targeting;
        private bool shooting;
        private int steps;
        private Vector2 point;
        private Vector2 origin;
        private Vector2 nextTarget;
        private int mine;
        private float shootingCd;

        public AutomaticMineLauncher(Player owner)
        {
            this.owner = owner;
            this.InnerCD = 0;
            Enable = true;

            Init();
        }

        private void Init()
        {
            this.weapon = owner.Mine as MineLauncher;
            this.lastLevel = weapon.UpgradeLevel;
            this.rand = owner.SceneMgr.GetRandomGenerator();
            this.targeting = false;
            this.shooting = false;
            this.steps = 0;
            this.mine = 0;
            this.shootingCd = 0;

            weapon.AddClickListener(this);
            owner.SceneMgr.AddMoveListener(this);

            nextTarget = new Vector2();
            nextTarget.Y = 1;
            nextTarget.X = (float) FastMath.LinearInterpolate(minWidth, maxWidth, rand.NextDouble());

            if (owner.GetPosition() == PlayerPosition.RIGHT)
            {
                minWidth = 0;
                maxWidth = SharedDef.VIEW_PORT_SIZE.Width * SharedDef.MINE_ACTIVE_RADIUS;
            }
            else
            {
                minWidth = SharedDef.VIEW_PORT_SIZE.Width - (SharedDef.VIEW_PORT_SIZE.Width * SharedDef.MINE_ACTIVE_RADIUS);
                maxWidth = SharedDef.VIEW_PORT_SIZE.Width;
            }
        }

        public void Update(float tpf)
        {
            if (!Enable)
                return;

            if (lastLevel != owner.Mine.UpgradeLevel)
            {
                weapon = owner.Mine as MineLauncher;
                lastLevel = weapon.UpgradeLevel;
                weapon.AddClickListener(this);
            }

            if (shooting)
            {
                proccesShooting(tpf);
                return;
            }

            if (InnerCD > 0)
            {
                InnerCD -= tpf;
                weapon.ReloadTime = weapon.Owner.Data.MineCooldown;
                if (InnerCD <= 0)
                {
                    //TODO add launch area
                }

                return;
            }

            if (owner.Mine != weapon)
                weapon = owner.Mine as MineLauncher;

            if (weapon.IsReady())
            {
                weapon.Shoot(nextTarget);

                nextTarget.X = (float) FastMath.LinearInterpolate(minWidth, maxWidth, rand.NextDouble());
            }
            else
            {
                float percentage = 1 - (weapon.ReloadTime / weapon.Owner.Data.MineCooldown);
            }
        }

        public void proccesShooting(float tpf)
        {
            weapon.ReloadTime = weapon.Owner.Data.MineCooldown;
            shootingCd -= tpf;

            if (steps <= 0)
            {
                shooting = false;
                return;
            }

            if (shootingCd <= 0)
            {
                weapon.Shoot(point, true);
                point.X += 32;

                shootingCd = SharedDef.MINE_VOLLEY_TIME;
                mine++;
                if (mine >= 7)
                {
                    steps--;
                    mine = 0;
                    point = origin;
                }
            }
        }


        public bool ProccessClickEvent(Vector2 point, MouseButton button, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Pressed && !targeting)
                PrepareToFire(point);

            if (buttonState == MouseButtonState.Released && !shooting)
                Fire(point);

            return true;
        }

        private void PrepareToFire(Vector2 point)
        {
            if (InnerCD > 0)
                return;

            targeting = true;
        }

        private void Fire(Vector2 point)
        {
            if (!targeting)
                return;

            this.point = point;
            this.point.X -= 115;
            this.origin = this.point;

            targeting = false;
            shooting = true;
            steps = getStepsByWeaponLvl();
            mine = 0;
            shootingCd = 0;
            InnerCD = SharedDef.MINE_VOLLEY_CD;
        }

        private int getStepsByWeaponLvl()
        {
            if (weapon.UpgradeLevel == UpgradeLevel.LEVEL1)
                return 1;
            if (weapon.UpgradeLevel == UpgradeLevel.LEVEL2)
                return 2;
            if (weapon.UpgradeLevel == UpgradeLevel.LEVEL3)
                return 3;

            return 0;
        }

        public void OnMouseMove(Vector2 point)
        {
        }
    }
}
