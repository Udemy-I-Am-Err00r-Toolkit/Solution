using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    [CreateAssetMenu(fileName = "ItemType", menuName = "Metroidvania/Items/NullItem", order = 2)]
    public class ItemType : ScriptableObject
    {
        public virtual void UseItem(GameObject player)
        {

        }
    }
}