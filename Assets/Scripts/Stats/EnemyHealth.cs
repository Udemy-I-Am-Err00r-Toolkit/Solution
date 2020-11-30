using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class EnemyHealth : Health
    {
        public override void DealDamage(int amount)
        {
            base.DealDamage(amount);
            if(healthPoints <= 0 && gameObject.GetComponent<EnemyCharacter>())
            {
                if(gameObject.GetComponent<RandomDrop>())
                {
                    gameObject.GetComponent<RandomDrop>().Roll();
                }
                gameObject.SetActive(false);
                Invoke("Revive", 1);
            }
        }

        protected virtual void Revive()
        {
            gameObject.GetComponent<Health>().healthPoints += 100;
            gameObject.SetActive(true);
        }

    }
}