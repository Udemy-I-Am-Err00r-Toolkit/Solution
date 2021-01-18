using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script is used to help place the Player back into a location that would make sense if they fall out of bounds; this uses a trigger collider that when the Player enters warps the Player back to the last standing location, or if the specificLocation bool is selected, to wherever that location variable is set to
    public class OutOfBounds : Managers
    {
        [SerializeField]
        protected bool specificLocation;
        [SerializeField]
        protected Vector3 location;
        protected override void Initialization()
        {
            base.Initialization();
        }
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == player)
            {
                if (specificLocation)
                {
                    player.transform.position = location;
                }
                else
                {
                    player.transform.position = player.GetComponent<HorizontalMovement>().bestDeltaPosition;
                }
                StartCoroutine(levelManager.FallFadeOut());
            }
        }
        private void OnDrawGizmos()
        {
            if (specificLocation)
            {
                Gizmos.DrawWireSphere(location, 1);
            }
        }
    }
}