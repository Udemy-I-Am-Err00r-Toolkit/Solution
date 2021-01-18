using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script will handle each individual room of the WorldMap so that the camera will be bound to that specific room to display on the mini-map
    public class WorldMap : Managers
    {
        //The bounds the mini-map camera is allowed to move to on the mini-map
        public Bounds bounds;

        //Visual representation of the bounds
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.localPosition + transform.parent.position + bounds.center, bounds.size);
        }
    }
}