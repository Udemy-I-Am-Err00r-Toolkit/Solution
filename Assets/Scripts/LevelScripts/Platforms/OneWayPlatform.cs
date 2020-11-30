using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class OneWayPlatform : PlatformManager
    {
        [SerializeField]
        protected enum OneWayPlatforms { GoingUp, GoingDown, Both }
        [SerializeField]
        protected OneWayPlatforms type;

        protected override void Initialization()
        {
            base.Initialization();
        }

        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            if(collision.gameObject == player)
            {
                if(collision.gameObject.transform.position.y < transform.position.y && (type == OneWayPlatforms.Both || type == OneWayPlatforms.GoingUp))
                {
                    platformCollider.isTrigger = true;
                }
                if(collision.gameObject.transform.position.y > transform.position.y && (type == OneWayPlatforms.Both || type == OneWayPlatforms.GoingDown) && character.isJumpingThroughPlatform)
                {
                    platformCollider.isTrigger = true;
                }
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            platformCollider.isTrigger = false;
        }
    }
}