using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //Scriptable Object for all the Consumable Type items; generally this would be used for health items, but can be used for ammo pickups as well if you add that in your game
    [CreateAssetMenu(fileName = "ItemType", menuName = "Metroidvania/Items/Consumable/ConsumableItem", order = 2)]
    public class Consumable : ItemType
    {
        public string itemName;
        //This is the prefab that would act as the interactable game object in the scene.
        public GameObject prefab;

        public override void UseItem(GameObject player)
        {
            base.UseItem(player);
        }
    }
}