using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core
{
    public class FastMath
    {

        public static float LinearInterpolate(float from, float to, float percentage)
        {
            return from + (to - from) * percentage; //xna
        }

        public static double LinearInterpolate(double from, double to, double percentage)
        {
            return (from * (1 - percentage) + to * percentage); // paul bourke
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

        /// <summary>
        /// vrati nahodnou rotaci 0 - 2PI v radianech
        /// </summary>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        public static double GetRandomRotation(Random randomGenerator)
        {
            return randomGenerator.NextDouble() * Math.PI * 2;
        }
    }
}
