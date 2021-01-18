using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class OneWayPlatform : PlatformManager
    {
        //The different types of one-way platforms
        [SerializeField]
        protected enum OneWayPlatforms { GoingUp, GoingDown, Both }
        [SerializeField]
        protected OneWayPlatforms type;

        protected override void Initialization()
        {
            base.Initialization();
        }

        //Checks to make sure the Player is either below or above the platform when colliding, and depending on what oneway platform type it is, allows the player to pass through it by making the platform collider a trigger collider.
        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject == player)
            {
                if (collision.gameObject.transform.position.y < transform.position.y && (type == OneWayPlatforms.Both || type == OneWayPlatforms.GoingUp))
                {
                    platformCollider.isTrigger = true;
                }
                if (collision.gameObject.transform.position.y > transform.position.y && (type == OneWayPlatforms.Both || type == OneWayPlatforms.GoingDown) && character.isJumpingThroughPlatform)
                {
                    platformCollider.isTrigger = true;
                }
            }
        }

        //Once the player passes through a trigger collider, it turns the collider back into a regular collider
        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            platformCollider.isTrigger = false;
        }
    }
}