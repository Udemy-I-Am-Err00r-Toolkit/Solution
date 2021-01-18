using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //Scriptable Object for all items; this script is empty and houses the most basic data for all Items
    [CreateAssetMenu(fileName = "ItemType", menuName = "Metroidvania/Items/NullItem", order = 2)]
    public class ItemType : ScriptableObject
    {
        public virtual void UseItem(GameObject player)
        {

        }
    }
}