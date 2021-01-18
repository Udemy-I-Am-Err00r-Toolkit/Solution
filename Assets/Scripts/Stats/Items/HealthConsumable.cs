using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This is the Scriptable Object for Health pickups, these are unique to the Player, and will fill up the Player's health based on the amount set within this Scriptable Object
    [CreateAssetMenu(fileName = "ItemType", menuName = "Metroidvania/Items/Consumable/HealthConsumable", order = 2)]
    public class HealthConsumable : Consumable
    {
        public int amount;
        public override void UseItem(GameObject player)
        {
            player.GetComponent<PlayerHealth>().GainCurrentHealth(amount);
            base.UseItem(player);
        }
    }
}