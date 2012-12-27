using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;

namespace Orbit.Core.SpecialActions.Spectator
{
    public class RangeGroup
    {
        public AsteroidType Type { get; set; }
        private List<Range> Ranges { get; set; }

        public RangeGroup(AsteroidType type, params Range[] ranges)
        {
            Type = type;
            Ranges = new List<Range>(ranges);
        }

        public float ComputePercentage(List<MiningObject> list)
        {
            float percents = 0;
            float temp = 0;
            foreach (Range range in Ranges)
            {
                temp = range.ComputePercentage(list, Type);
                if (temp > percents)
                    percents = temp;
            }

            return percents;
        }

        public int ComputeMissing(List<MiningObject> list)
        {
            int missing = int.MaxValue;
            int temp = 0;

            foreach (Range range in Ranges)
            {
                temp = range.ComputeMissing(list, Type);
                if (temp < missing)
                    missing = temp;
            }
            
            return missing;
        }
    }

    public class Range
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }


        public Range(int minimum = int.MinValue, int maximum = int.MaxValue) 
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public float ComputePercentage(List<MiningObject> list, AsteroidType type)
        {
            int count = getCount(list, type);

            if (count >= Minimum && count <= Maximum)
                return 1;

            if (count < Minimum)
                return count / Minimum;

            if (count > Maximum)
            {
                while (count > Maximum)
                    count -= Maximum;

                return 1 - (count / Maximum);
            }

            //nemelo by nikdy nastat
            return 0;
        }

        public int ComputeMissing(List<MiningObject> list, AsteroidType type)
        {
            int count = getCount(list, type);

            if (count >= Minimum && count <= Maximum)
                return 0;

            if (count < Minimum)
                return Minimum - count;

            if (count > Maximum)
                return Maximum - count;

            //nemelo by nikdy nastat
            return 0;
        }

        private int getCount(List<MiningObject> list, AsteroidType type)
        {
            int result = 0;
            foreach(MiningObject obj in list) 
            {
                if (obj.Obj is Asteroid)
                {
                    if ((obj.Obj as Asteroid).AsteroidType == type)
                        result++;
                }
            }

            return result;
        }

        

    }
}
