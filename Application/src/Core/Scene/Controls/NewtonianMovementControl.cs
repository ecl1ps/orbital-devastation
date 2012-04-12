using System;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    public class NewtonianMovementControl : Control
    {
        private IMovable meMovable;
        private float earthSurface;
        public float Speed { get; set; }

        public override void InitControl(ISceneObject obj)
        {
            if (!(obj is IMovable))
            {
                Console.Error.WriteLine("NewtonianMovementControl cannot be attached to non-movable object!");
                return;
            }

            earthSurface = (float)SceneMgr.GetInstance().ViewPortSizeOriginal.Height;
            meMovable = obj as IMovable;
        }

        public override void UpdateControl(float tpf)
        {
            if (meMovable == null)
                return;

            // jsme na orbite - gravitace se zanedbava
            if (me.GetPosition().Y < SceneMgr.GetInstance().GetOrbitArea().Height)
            {
                me.SetPosition(me.GetPosition() + (meMovable.GetDirection() * Speed * tpf));
                return;
            }

            Vector dirToSurf = new Vector(me.GetPosition().X, earthSurface) - me.GetPosition();
            float distToSurf = (float)dirToSurf.Length;
            dirToSurf.Normalize();

            // pokud jsme uz pod povrchem zeme, pokracujme dal (ne zpet nahoru k nemu)
            if (me.GetPosition().Y > earthSurface)
                dirToSurf.Negate();

            // soucasne pozici se pricte vektor dopredneho smeru a pote gravitacni vektor smerujici k povrchu zeme
            // rychlost roste s klesajici vzdalenosti k povrchu
            me.SetPosition(me.GetPosition() + (meMovable.GetDirection() * Speed * tpf) + dirToSurf * (earthSurface / distToSurf) * tpf * 30);
        }
    }
}
