using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.SpecialActions.Spectator
{
   public enum Limit{
        EXACT,
        TOP_LIMIT,
        BOTTOM_LIMIT
    }

    public abstract class SpectatorAction : SpecialAction, ISpectatorAction
    {
        protected MiningModuleControl control;

        protected int normal;
        public int Normal { get { return normal; } }
        protected int gold;
        public int Gold { get { return gold; } }
        protected Limit limit;

        public int MissingNormal { get { return computeMissing(AsteroidType.NORMAL); } }
        public int MissingGold { get { return computeMissing(AsteroidType.GOLDEN); } }


        public SpectatorAction(MiningModuleControl control, SceneMgr mgr, Players.Player owner)
            : base(mgr, owner)
        {
            this.control = control;
        }

        public float Percentage
        {
            get { return computePercentage(); }
        }

        private float computePercentage()
        {
            List<MiningObject> list = control.currentlyMining;

            int normal = 0;
            int gold = 0;

            foreach (MiningObject obj in list)
                if (obj.Obj is Asteroid)
                {
                    if ((obj.Obj as Asteroid).AsteroidType == AsteroidType.GOLDEN)
                        gold++;
                    else if ((obj.Obj as Asteroid).AsteroidType == AsteroidType.NORMAL)
                        normal++;
                }

            switch (limit)
            {
                case Limit.EXACT:
                    return computePercentageExact(normal, gold);
                
                case Limit.BOTTOM_LIMIT:
                    return computePercentageBottom(normal, gold);

                case Limit.TOP_LIMIT:
                    return computePercentageTop(normal, gold);
                
                default:
                    throw new Exception("Unknown limit value");
            }
        }

        protected float computePercentageTop(int normal, int gold)
        {
            float normPerc = this.normal == 0 ? 1 : 0;
            float goldPerc = this.gold == 0 ? 1 : 0;

            if (normPerc != 1)
                normPerc = normal / this.normal;

            if (goldPerc != 1)
                goldPerc = gold / this.gold;

            if (goldPerc < 1)
                goldPerc = 1;
            else
                goldPerc = computeModulo(goldPerc);

            if (normPerc < 1)
                normPerc = 1;
            else
                normPerc = computeModulo(normPerc);

            return goldPerc * normPerc;
        }

        protected float computePercentageBottom(int normal, int gold)
        {
            float normPerc = this.normal == 0 ? 1 : 0;
            float goldPerc = this.gold == 0 ? 1 : 0;

            if (normPerc != 1)
                normPerc = normal / this.normal;

            if (goldPerc != 1)
                goldPerc = gold / this.gold;

            if (goldPerc > 1)
                goldPerc = 1;
            else
                goldPerc = computeModulo(goldPerc);

            if (normPerc > 1)
                normPerc = 1;
            else
                normPerc = computeModulo(normPerc);

            return goldPerc * normPerc;
        }

        protected float computePercentageExact(int normal, int gold)
        {

            float normPerc = this.normal == 0 ? 1 : 0;
            float goldPerc = this.gold == 0 ? 1 : 0;

            if (normPerc != 1)
                normPerc = normal / this.normal;

            if (goldPerc != 1)
                goldPerc = gold / this.gold;

            if (goldPerc > 1)
                goldPerc = 1 - computeModulo(goldPerc);
            else
                goldPerc = computeModulo(goldPerc);

            if (normPerc > 1)
                normPerc = 1 - computeModulo(normPerc);
            else
                normPerc = computeModulo(normPerc);

            return goldPerc * normPerc;
        }

        private float computeModulo(float perc)
        {
            return ((perc * 100) % 100) / 100;
        }

        private int computeMissing(AsteroidType type)
        {
            List<MiningObject> list = control.currentlyMining;

            int ast = 0;

            switch(type) 
            {
                case AsteroidType.NORMAL:
                    ast = Normal;
                    break;

                case AsteroidType.GOLDEN:
                    ast = Gold;
                    break;

                default:
                    throw new Exception("Undefined type " + type);
            }

            foreach (MiningObject obj in list)
                if (obj.Obj is Asteroid && (obj.Obj as Asteroid).AsteroidType == type)
                    ast--;

            return ast;
        }

    }
}
