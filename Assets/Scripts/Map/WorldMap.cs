using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class WorldMap : Managers
    {
        public Bounds bounds;

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.localPosition + transform.parent.position + bounds.center, bounds.size);
        }
    }
}