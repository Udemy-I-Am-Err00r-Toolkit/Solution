using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class DamageOnTouch : AIManagers
    {
        [SerializeField]
        protected int damageAmount;
        [SerializeField]
        protected LayerMask damageLayers;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if ((1 << collision.gameObject.layer & damageLayers) != 0)
            {
                collision.gameObject.GetComponent<Health>().DealDamage(damageAmount);
                if (transform.position.x < collision.transform.position.x)
                {
                    collision.gameObject.GetComponent<Health>().left = false;
                }
                else
                    collision.gameObject.GetComponent<Health>().left = true;
            }
        }
    }
}