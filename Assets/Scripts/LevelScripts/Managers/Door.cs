using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This manages the doors to open when a projectile hits it
    public class Door : Managers
    {
        //The different projectile types that can open the door based on their tag
        [SerializeField]
        protected string[] tagsToOpen;

        //Checks to see if whatever is colliding with this can open the door based on tag and then plays the animation to open the door if it is one of those tags, as well as disables a collider that would normally prevent the player from walking through it
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            for (int i = 0; i < tagsToOpen.Length; i++)
            {
                if (collision.gameObject.tag == tagsToOpen[i])
                {
                    GetComponent<Collider2D>().enabled = false;
                    GetComponent<Animator>().SetBool("Open", true);
                }
            }
        }
    }
}