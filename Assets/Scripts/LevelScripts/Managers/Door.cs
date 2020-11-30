using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class Door : Managers
    {
        [SerializeField]
        protected string[] tagsToOpen;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            for(int i = 0; i < tagsToOpen.Length; i++)
            {
                if(collision.gameObject.tag == tagsToOpen[i])
                {
                    GetComponent<Collider2D>().enabled = false;
                    GetComponent<Animator>().SetBool("Open", true);
                }
            }
        }
    }
}