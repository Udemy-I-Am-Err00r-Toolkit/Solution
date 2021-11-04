using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script is specific to Ability prefabs that the Player would pickup to gain an ability
    public class AbilityPickup : MonoBehaviour
    {
        protected Character character;
        public ItemType item;
        //This bool will determine if the pickup has been found by Player, if found, it will disable the game object in the scene at start
        protected bool found;

        private void OnEnable()
        {
            character = FindObjectOfType<Character>();
            //Sets the found bool based on the PlayerPrefs; this is only set when the item trigger collider is interacted with the Player
            found = PlayerPrefs.GetInt(" " + character.gameFile + item.name) == 1 ? true : false;
            if (found)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                item.UseItem(collision.gameObject);
                found = true;
                //Sets the PlayerPrefs value so that when game is loading, can determine if this pickup has already been grabbed by Player
                PlayerPrefs.SetInt(" " + collision.GetComponent<Character>().gameFile + item.name, found ? 1 : 0);
                Destroy(gameObject);
            }
        }
    }
}