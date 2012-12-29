using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Controls.Health;

namespace Orbit.Core.Scene.Controls.Health
{
    public abstract class HpControl : Control, IHpControl
    {
        protected int hp;
        public int Hp { get { return hp; } set { UpdateHp(value); } }
        public int MaxHp { get; set; }

        protected override void InitControl(Entities.ISceneObject me)
        {
            hp = MaxHp;
        }

        protected virtual void UpdateHp(int amount)
        {
            if (amount > MaxHp)
                hp = MaxHp;
            else if (amount <= 0)
            {
                hp = 0;
                OnDeath();
            }
            else
                hp = amount;
        }

        public virtual void RefillHp()
        {
            hp = MaxHp;
        }

        protected abstract void OnDeath();
    }
}
