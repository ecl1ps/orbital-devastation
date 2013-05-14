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
        private Player owner;
        private MineLauncher weapon;

        private double minWidth;
        private double maxWidth;
        private Random rand;

        public float InnerCD;

        private ArrowCast castArea;
        private bool targeting;
        private bool shooting;
        private int steps;
        private Point point;
        private Point origin;
        private int mine;
        private float shootingCd;

        public AutomaticMineLauncher(Player owner)
        {
            this.owner = owner;
            this.InnerCD = 0;

            Init();
            InitCastArea();
        }

        private void Init()
        {
            this.weapon = owner.Mine as MineLauncher;
            this.rand = owner.SceneMgr.GetRandomGenerator();
            this.targeting = false;
            this.shooting = false;
            this.steps = 0;
            this.mine = 0;
            this.shootingCd = 0;

            weapon.AddClickListener(this);
            owner.SceneMgr.AddMoveListener(this);

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

        private void InitCastArea()
        {
            owner.SceneMgr.BeginInvoke(new Action(() => {
                castArea = new ArrowCast();
                castArea.Visibility = Visibility.Hidden;
                owner.SceneMgr.GetCanvas().Children.Add(castArea);
                castArea.StartAnimation();
            }));
        }

        public void Update(float tpf)
        {
            if (shooting)
            {
                proccesShooting(tpf);
                return;
            }

            if (InnerCD > 0)
            {
                InnerCD -= tpf;
                weapon.ReloadTime = weapon.Owner.Data.MineCooldown;
                return;
            }

            if (owner.Mine != weapon)
                weapon = owner.Mine as MineLauncher;

            if (weapon.IsReady())
            {
                Point p = new Point();
                p.X = FastMath.LinearInterpolate(minWidth, maxWidth, rand.NextDouble());
                p.Y = 1;

                weapon.Shoot(p);
            }
        }

        public void proccesShooting(float tpf)
        {
            weapon.ReloadTime = weapon.Owner.Data.MineCooldown;
            shootingCd -= tpf;

            if (steps <= 0)
            {
                owner.SceneMgr.BeginInvoke(new Action(() =>
                {
                    castArea.Visibility = Visibility.Hidden;
                    castArea.EndAnimation();
                }));

                shooting = false;
                InnerCD = SharedDef.MINE_VOLLEY_CD;
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


        public bool ProccessClickEvent(Point point, MouseButton button, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Pressed)
                PrepareToFire(point);

            if (buttonState == MouseButtonState.Released)
                Fire(point);

            return true;
        }

        private void PrepareToFire(Point point)
        {
            if (InnerCD > 0)
                return;

            targeting = true;
            owner.SceneMgr.BeginInvoke(new Action(() => {
                castArea.Visibility = Visibility.Visible;
                Canvas.SetLeft(castArea, point.X - 115);
                Canvas.SetTop(castArea, 25);
            }));
        }

        private void Fire(Point point)
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

        public void OnMouseMove(Point point)
        {
            if (targeting)
            {
                owner.SceneMgr.BeginInvoke(new Action(() =>
                {
                    Canvas.SetLeft(castArea, point.X - 115);
                    Canvas.SetTop(castArea, 25);
                }));
            }
        }
    }
}
