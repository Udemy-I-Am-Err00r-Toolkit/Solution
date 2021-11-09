using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //Health script that is specific for Enemies; this script inherits from Health, where a lot of the logic is derived from
    public class EnemyHealth : Health
    {
        
        //This method handles logic specific to dealing damage to an Enemy; this could also be used on a wall that the Player can destroy
        public override void DealDamage(int amount)
        {
            base.DealDamage(amount);
            //This handles what should happen if health is less than zero for an Enemy
            if (healthPoints <= 0 && gameObject.GetComponent<EnemyCharacter>())
            {
                //If the enemy has a random drop it would leave, this handles the logic for dropping items
                if (gameObject.GetComponent<RandomDrop>())
                {
                    gameObject.GetComponent<RandomDrop>().Roll();
                }
                //Disables object from scene when killed
                gameObject.SetActive(false);
                //This is a testing method, probably shouldn't exist in real game
                Invoke("Revive", 1);
            }
        }

        //This revives the enemy quickly so you can test out certain features when building game; this method probably shouldn't exist in real game
        protected virtual void Revive()
        {
            gameObject.GetComponent<Health>().healthPoints += 100;
            gameObject.SetActive(true);
        }

    }
}