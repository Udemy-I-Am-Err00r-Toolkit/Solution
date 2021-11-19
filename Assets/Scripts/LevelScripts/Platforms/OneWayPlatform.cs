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
        //A quick delay so the player can receive platform collision again
        [SerializeField]
        protected float delay = .5f;

        //This will run when the player colides with the platform and the logic inside works for going up through the platform
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            //Checks to make sure it's the player colliding with the platform
            if(collision.gameObject == player)
            {
                //Checks to see if the maximum point on the player collider is lower than the center of the platform, meaning the player is beneath the platform
                if(!character.isGrounded && player.GetComponent<Collider2D>().bounds.min.y < platformCollider.bounds.center.y && (type == OneWayPlatforms.Both || type == OneWayPlatforms.GoingUp))
                {
                    //Method that will allow the player to pass through the platform collider while everything else stays
                    Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), platformCollider, true);
                    //Runs Coroutine to reestablish collider for player and turn off the isJumpingThroughPlatform bool
                    StartCoroutine(StopIgnoring());
                }
            }
        }

        //This method will run as long as the player is touching the collider and the logic inside works for going down through the platform
        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            //Checks to make sure it's the player colliding with the platform
            if (collision.gameObject == player)
            {
                //Checks to see if the minimum point on the player collider is above than the center of the platform, meaning the player is above the platform while holding down and jumping
                if(player.GetComponent<Jump>().downwardJump && player.GetComponent<Collider2D>().bounds.min.y > platformCollider.bounds.center.y && (type == OneWayPlatforms.Both || type == OneWayPlatforms.GoingDown))
                {
                    //Method that will allow the player to pass through the platform collider while everything else stays
                    Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), platformCollider, true);
                    //Runs Coroutine to reestablish collider for player and turn off the isJumpingThroughPlatform bool
                    StartCoroutine(StopIgnoring());
                }
            }
        }

        //Coroutine that resets the isJumpingThroughPlatform bool back to false and allows the player to collide with the platform
        protected virtual IEnumerator StopIgnoring()
        {
            //Half secod delay wait to perform the logic next; change value through code up top
            yield return new WaitForSeconds(delay);
            //Makes the player collide with platform again
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), platformCollider, false);
            //Resets the isJumpingThroughPlatform bool back to false
        }
    }
}