using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object that allows abilities to be toggled on when pickup is found
namespace MetroidvaniaTools
{
    [CreateAssetMenu(fileName = "ItemType", menuName = "Metroidvania/Items/Consumable/Ability", order = 2)]
    public class AbilityItem : Consumable
    {
        //This method is called when the Player enters the trigger collider for a pickup
        public override void UseItem(GameObject player)
        {
            base.UseItem(player);
            //The Abilities script needs to have a method that matches the itemName so it can toggle on the Ability right when it is picked up
            player.GetComponent<Abilities>().Invoke(itemName, 0);
        }
    }
}