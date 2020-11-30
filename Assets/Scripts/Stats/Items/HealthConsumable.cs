using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
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