using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core
{
    class FastMath
    {

        public static float LinearInterpolate(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount; //xna
        }

        public static double LinearInterpolate(double y1, double y2, double mu)
        {
            return (y1 * (1 - mu) + y2 * mu); // paul bourke
        }
    }
}
