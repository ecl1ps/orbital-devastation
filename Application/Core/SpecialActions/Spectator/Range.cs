using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.SpecialActions.Spectator
{
    public class RangeGroup
    {
        private List<Range> Ranges { get; set; }

        public RangeGroup(params Range[] ranges)
        {
            Ranges = new List<Range>(ranges);
        }

        public List<Asteroid> GetValidGroup(List<MiningObject> objects)
        {
            List<Asteroid> temp = new List<Asteroid>();

            foreach (Range range in Ranges)
                temp.AddRange(range.GetValidGroup(objects));

            return temp;
        }

    }

    public class Range
    {
        public int Maximum { get; set; }
        public AsteroidType? Type { get; set; }

        public Range()
        {
            Maximum = int.MaxValue;
            Type = null;
        }

        public Range(AsteroidType type, int maximum) 
        {
            Maximum = maximum;
            Type = type;
        }

        public List<Asteroid> GetValidGroup(List<MiningObject> objects)
        {
            List<Asteroid> temp = new List<Asteroid>();

            foreach (MiningObject obj in objects)
            {
                if (Type == null && obj.Obj is Asteroid)
                    temp.Add(obj.Obj as Asteroid);
                else if (obj.Obj is Asteroid && (obj.Obj as Asteroid).AsteroidType == Type)
                    temp.Add(obj.Obj as Asteroid);

                if (temp.Count > Maximum)
                    break;
            }

            return temp;
        }

    }
}
