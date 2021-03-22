using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class Ladder : PlatformManager
    {
        //The position that the Player needs to be at to be standing on top of the ladder and preventing the Player from falling through the ladder
        [HideInInspector]
        public Vector3 topOfLadder;
        //The position the Player needs to be at to be standing on ground again while still inside the trigger collider of the ladder
        [HideInInspector]
        public Vector3 bottomOfLadder;
        //A quick reference to the HorizontalMovement script on the Player; the HorizontalMovement script handles having the player move up and down the ladder, and needs a reference to whatever gameobject the Player is inside of that is acting as a ladder
        protected HorizontalMovement movement;

        protected override void Initialization()
        {
            base.Initialization();
            movement = player.GetComponent<HorizontalMovement>();
            topOfLadder = new Vector3(transform.position.x, platformCollider.bounds.max.y);
            bottomOfLadder = new Vector3(transform.position.x, platformCollider.bounds.min.y);
            platformCollider.isTrigger = true;
        }

        //If the Player is outside of the trigger collider on the ladder, then it ensures the player no longer interacts with the ladder
        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject == player)
            {
                character.isOnLadder = false;
                movement.currentLadder = null;
            }
        }

        //If the Player is inside the trigger collider of a ladder, then it sets up the logic to allow the Player to go up and down the ladder
        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject == player)
            {
                if (character.isJumping)
                {
                    character.isOnLadder = false;
                    movement.currentLadder = null;
                    return;
                }
                else
                {
                    character.isOnLadder = true;
                    movement.currentLadder = gameObject;
                }
            }
        }
    }
}