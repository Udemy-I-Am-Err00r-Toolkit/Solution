using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    [CreateAssetMenu(fileName = "ItemType", menuName = "Metroidvania/Items/Consumable/ConsumableItem", order = 2)]
    public class Consumable : ItemType
    {
        public string itemName;
        public GameObject prefab;

        public override void UseItem(GameObject player)
        {
            base.UseItem(player);
        }
    }
}