using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //Very simple script that works with the BuoyancyEffector2D component to trigger the animation that should play when the Player is swimming; the BuuyancyEffector2D component handles the complicated logic of having the Player float in water
    [RequireComponent(typeof(BoxCollider2D), typeof(BuoyancyEffector2D))]
    public class Water : Managers
    {
        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject == player)
            {
                character.isSwimming = true;
                player.GetComponent<Animator>().SetBool("Swimming", true);
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject == player)
            {
                character.isSwimming = false;
                player.GetComponent<Animator>().SetBool("Swimming", false);
            }
        }
    }
}