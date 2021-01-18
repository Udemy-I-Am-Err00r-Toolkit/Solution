using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //Base clase for pickups
    public class ItemPickup : MonoBehaviour
    {
        public ItemType item;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                item.UseItem(collision.gameObject);
                Destroy(gameObject);
            }
        }
    }
}