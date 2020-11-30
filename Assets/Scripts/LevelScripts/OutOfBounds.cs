using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
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