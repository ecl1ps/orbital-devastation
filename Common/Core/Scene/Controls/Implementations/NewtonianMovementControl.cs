using System;
using System.Windows;
using Orbit.Core.Scene.Entities;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class NewtonianMovementControl : Control, IMovementControl
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IMovable meMovable;
        private float earthSurface;
        public float Speed { get; set; }
        public float HorizontalSpeed
        {
            get
            {
                return (meMovable.Direction * Speed).GetHorizontalLenght();
            }
        }
        private float verticalSpeed;
        public float VerticalSpeed { get { return verticalSpeed; } }
        public float TimeAlive { get; set; }
        private Vector2 move;
        public Vector2 RealDirection { get { return move.NormalizeV(); } }
        public float RealSpeed { get { return (float) move.Length(); } }

        protected override void InitControl(ISceneObject obj)
        {
            if (!(obj is IMovable))
            {
                Logger.Error("NewtonianMovementControl cannot be attached to non-movable object!");
                return;
            }

            TimeAlive = 0;
            verticalSpeed = 0;
            // povrch je az pod povrchem - aby se sfery nezastavily na rozhrani
            if (me.SceneMgr != null) // hack kvuli serveru :(
                earthSurface = (float)SharedDef.VIEW_PORT_SIZE.Height * 2; 

            meMovable = obj as IMovable;
            move = meMovable.Direction;
        }

        protected override void UpdateControl(float tpf)
        {
            if (meMovable == null || double.IsNaN(meMovable.Direction.Length()))
                return;

            Vector2 dirToSurf = new Vector2(0, -1);
            float vSpeed = (float) (me.SceneMgr.LevelEnv.CurrentGravity - me.SceneMgr.LevelEnv.CurrentGravity *
                            (1 + Math.Pow((HorizontalSpeed - SharedDef.FIRST_COSMICAL_SPEED) / SharedDef.FIRST_COSMICAL_SPEED, 2))
                            * 1 / (GetRealativeAltitude() / me.SceneMgr.LevelEnv.StableOrbitRelative));

            move = (meMovable.Direction * Speed * tpf) +
                (dirToSurf * vSpeed * 2 * TimeAlive * tpf);

            me.Position += move;

            TimeAlive += tpf;
            verticalSpeed = (float) vSpeed;

            /*SceneMgr.GetInstance().ShowStatusText(1, "ORBITA: " + SharedDef.STABLE_ORBIT_HEIGHT);
            SceneMgr.GetInstance().ShowStatusText(2, "S: " + Speed + "H: " + HorizontalSpeed);
            SceneMgr.GetInstance().ShowStatusText(3, "ALT: " + GetRealativeAltitude());
            SceneMgr.GetInstance().ShowStatusText(4, "" + (SharedDef.GRAVITY * (1 + Math.Pow((HorizontalSpeed - SharedDef.FIRST_COSMICAL_SPEED) / SharedDef.FIRST_COSMICAL_SPEED, 2)) * (GetRealativeAltitude() / SharedDef.STABLE_ORBIT_RELATIVE)));
            SceneMgr.GetInstance().ShowStatusText(5, "GRAV: " + SharedDef.GRAVITY);*/
        }

        private float GetRealativeAltitude()
        {
            return 1.0f - (float)me.Position.Y / earthSurface;
        }
    }
}
