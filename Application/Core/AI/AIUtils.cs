using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Controls;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.AI
{

    public enum TargetStatus
    {
        AVAILABLE,
        LOCKED,
        HOOKONLY,
        DEAD,
       
    }

    struct Target
    {
        public TargetStatus Status;
        public Asteroid Asteroid;
        public int Priority;
        
    };


    public class AIUtils
    {



        public static Vector ComputeDestinationPositionToHitTarget(ISceneObject targetObject, double sourceSpeed, Vector sourceCenter, Random randomGenerator)
        {
            Vector randomResultDirection = new Vector(1, 0).Rotate(FastMath.GetRandomRotation(randomGenerator));

            if (targetObject == null)
                return randomResultDirection;

            // rychlost bulletu
            double v1 = sourceSpeed;
            // rychlost objektu
            IMovementControl mc = targetObject.GetControlOfType<IMovementControl>();
            double v2 = mc == null ? 0 : mc.Speed;
            // vektor od objektu k launcheru hooku
            Vector cVec = sourceCenter - targetObject.Center;
            // vektor smeru objektu
            Vector dVec = (targetObject as IMovable).Direction;

            // vzdalenost mezi nabojem a objektem
            double c = cVec.Length;
            // cosinus uhlu, ktery sviraji vektory pohybu objektu a smeru k launcheru
            double cosAlpha = (cVec.X * dVec.X + cVec.Y * dVec.Y) / (cVec.Length * dVec.Length);

            // diskriminant pro kvadratickou rovnici cosinovy vety
            double D = Math.Pow(2 * c * cosAlpha, 2) - 4 * c * c * (1 - Math.Pow(v1 / v2, 2));
            // nebyl nalezen trojuhelnik (komplexni cisla)
            if (D < 0)
                return randomResultDirection;

            double sqrtD = Math.Sqrt(D);

            // kvadraticka rovnice cosinovy vety
            // odectenim D ziskame bod pred telesem, prictenim bychom ziskali bod za telesem (ve smeru jeho pohybu)
            double x1 = (2 * c * cosAlpha - sqrtD) / (2 - 2 * Math.Pow(v1 / v2, 2));

            dVec.Normalize();

            return targetObject.Center + (dVec * x1);
        }
    }
}
