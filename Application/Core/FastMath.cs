using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core
{
    public class FastMath
    {

        public static float LinearInterpolate(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount; //xna
        }

        public static double LinearInterpolate(double y1, double y2, double mu)
        {
            return (y1 * (1 - mu) + y2 * mu); // paul bourke
        }

        public static int Factorial(int input)
        {
            int answer = 1;

            if (input > 0)
            {
                int count = 1;
                while (count <= input)
                {
                    if (count == 1)
                    {
                        answer = 1;
                        count++;
                    }
                    else
                    {
                        answer = count * answer;
                        count++;
                    }
                }
            }
            else if (input < 0)
            {
                throw new InvalidOperationException("Factorial can by computed only from a positive integer.");
            }

            return answer;
        }
    }
}
