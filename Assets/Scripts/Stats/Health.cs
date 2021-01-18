using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //Base script that houses similar data for Enemies and Player
    public class Health : Managers
    {
        //This is the total amount of health an Enemy or Player can have
        public int maxHealthPoints;
        //This is the current amount of health an Enemy or Player has
        [HideInInspector]
        public int healthPoints;

        protected override void Initialization()
        {
            base.Initialization();
            //This is more specific to enemies so that when a scene loads, all the enemies have max health again
            healthPoints = maxHealthPoints;
        }

        //This method negates the health points based on the damage value found on the Projectile or Melee script that caused damage
        public virtual void DealDamage(int amount)
        {
            healthPoints -= amount;
        }
    }
}