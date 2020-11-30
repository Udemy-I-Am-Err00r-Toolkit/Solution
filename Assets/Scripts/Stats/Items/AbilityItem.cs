using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    [CreateAssetMenu(fileName = "ItemType", menuName = "Metroidvania/Items/Consumable/Ability", order = 2)]
    public class AbilityItem : Consumable
    {
        public override void UseItem(GameObject player)
        {
            base.UseItem(player);
            player.GetComponent<Abilities>().Invoke(itemName, 0);
        }
    }
}