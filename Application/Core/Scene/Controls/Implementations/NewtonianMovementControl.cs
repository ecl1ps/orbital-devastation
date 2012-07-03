using System;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class NewtonianMovementControl : Control
    {
        private IMovable meMovable;
        private float earthSurface;
        public float InitialSpeed { get; set; }
        public float Speed {
            get
            {
                return (float)meMovable.Direction.Length;
            }
        }
        public float HorizontalSpeed
        {
            get
            {
                return (float)meMovable.Direction.GetHorizontalLenght();
            }
        }
        public float TimeAlive { get; set; }

        public override void InitControl(ISceneObject obj)
        {
            if (!(obj is IMovable))
            {
                Console.Error.WriteLine("NewtonianMovementControl cannot be attached to non-movable object!");
                return;
            }

            TimeAlive = 0;
            // povrch je az pod povrchem - aby se sfery nezastavily na rozhrani
            if (me.SceneMgr != null) // hack kvuli serveru :(
                earthSurface = (float)SharedDef.VIEW_PORT_SIZE.Height * 2; 
            meMovable = obj as IMovable;
            meMovable.Direction *= InitialSpeed;
        }

        public override void UpdateControl(float tpf)
        {
            if (meMovable == null || double.IsNaN(meMovable.Direction.Length))
                return;

            Vector dirToSurf = new Vector(0, -1);

            me.Position += (meMovable.Direction * tpf) +
                (dirToSurf * 
                    (
                        (SharedDef.GRAVITY - SharedDef.GRAVITY * 
                            (1 + Math.Pow((HorizontalSpeed - SharedDef.FIRST_COSMICAL_SPEED) / SharedDef.FIRST_COSMICAL_SPEED, 2)) 
                            * 1 / (GetRealativeAltitude() / SharedDef.STABLE_ORBIT_RELATIVE)
                        )
                    )
                * 2 * TimeAlive * tpf);

            TimeAlive += tpf;

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
