using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.SpecialActions.Spectator
{
    public class Range
    {
        public int Maximum { get; set; }
        public int Minimum { get; set; }

        public Range()
        {
            Maximum = int.MaxValue;
            Minimum = 0;
        }

        public Range(int maximum, int minimum = 0) 
        {
            Maximum = maximum;
        }

        public List<Asteroid> GetValidGroup(List<MiningObject> objects)
        {
            List<Asteroid> temp = new List<Asteroid>();

            int count = Maximum < objects.Count ? Maximum : objects.Count;
            for (int i = Minimum; i < count; i++)
                if(objects[i].Obj != null)
                    temp.Add(objects[i].Obj as Asteroid);

            return temp;
        }

    }
}
