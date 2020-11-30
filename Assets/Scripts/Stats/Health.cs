using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class Health : Managers
    {
        public int maxHealthPoints;
        [HideInInspector]
        public int healthPoints;

        protected override void Initialization()
        {
            base.Initialization();
            healthPoints = maxHealthPoints;
        }

        public virtual void DealDamage(int amount)
        {
            healthPoints -= amount;
        }
    }
}