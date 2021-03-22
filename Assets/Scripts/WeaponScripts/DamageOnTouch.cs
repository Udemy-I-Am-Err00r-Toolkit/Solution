using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class DamageOnTouch : AIManagers
    {
        [SerializeField]
        protected int damageAmount;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject == player)
            {
                player.GetComponent<Health>().DealDamage(damageAmount);
                if (transform.position.x < player.transform.position.x)
                {
                    player.GetComponent<PlayerHealth>().left = false;
                }
                else
                    player.GetComponent<PlayerHealth>().left = true;
            }
        }
    }
}