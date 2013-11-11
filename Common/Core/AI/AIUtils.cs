using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Controls;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Microsoft.Xna.Framework;

namespace Orbit.Core.AI
{

    public enum TargetStatus
    {
        AVAILABLE,
        UNAVAILABLE,
        HOOKONLY
        
       
    }

    struct Target
    {
        public TargetStatus Status;
        public Asteroid Asteroid;
        public int Priority;
        
    };

   struct HookTarget {

       
        public long target;
        public HookControl hook;
        public HookTarget(HookControl hook, long target)
        {
            this.hook = hook;
            this.target = target;
        
        
        }
    
    }


    public class AIUtils
    {



        public static Vector2 ComputeDestinationPositionToHitTarget(ISceneObject targetObject, float sourceSpeed, Vector2 sourceCenter, Random randomGenerator)
        {
            Vector2 randomResultDirection = new Vector2(1, 0).Rotate(FastMath.GetRandomRotation(randomGenerator));

            if (targetObject == null)
                return randomResultDirection;

            // rychlost bulletu
            float v1 = sourceSpeed;
            // rychlost objektu
            IMovementControl mc = targetObject.GetControlOfType<IMovementControl>();
            float v2 = mc == null ? 0 : mc.Speed;
            // vektor od objektu k launcheru hooku
            Vector2 cVec = sourceCenter - targetObject.Center;
            // vektor smeru objektu
            Vector2 dVec = (targetObject as IMovable).Direction;

            // vzdalenost mezi nabojem a objektem
            float c = cVec.Length();
            // cosinus uhlu, ktery sviraji vektory pohybu objektu a smeru k launcheru
            float cosAlpha = (cVec.X * dVec.X + cVec.Y * dVec.Y) / (cVec.Length() * dVec.Length());

            // diskriminant pro kvadratickou rovnici cosinovy vety
            double D = Math.Pow(2 * c * cosAlpha, 2) - 4 * c * c * (1 - Math.Pow(v1 / v2, 2));
            // nebyl nalezen trojuhelnik (komplexni cisla)
            if (D < 0)
                return randomResultDirection;

            double sqrtD = Math.Sqrt(D);

            // kvadraticka rovnice cosinovy vety
            // odectenim D ziskame bod pred telesem, prictenim bychom ziskali bod za telesem (ve smeru jeho pohybu)
            float x1 = (float) ((2 * c * cosAlpha - sqrtD) / (2 - 2 * Math.Pow(v1 / v2, 2)));

            dVec.Normalize();

            return targetObject.Center + (dVec * x1);
        }
    }
}
