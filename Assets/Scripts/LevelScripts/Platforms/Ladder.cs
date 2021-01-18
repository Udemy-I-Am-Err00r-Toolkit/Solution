using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script will allow a gameobject to act as a ladder and let the player climb up or descend down whlie it is inside the trigger collider; once on top, it uses an edge collider to keep the player from falling back down
    [RequireComponent(typeof(EdgeCollider2D))]
    public class Ladder : PlatformManager
    {
        //The position that the Player needs to be at to be standing on top of the ladder and preventing the Player from falling through the ladder
        [HideInInspector]
        public Vector3 topOfLadder;
        //The position the Player needs to be at to be standing on ground again while still inside the trigger collider of the ladder
        [HideInInspector]
        public Vector3 bottomOfLadder;
        //A quick reference of the edge collider that allows the Player to stand on top of when the player is above the topOfLadder position
        [HideInInspector]
        public EdgeCollider2D edgeCollider;
        //A quick reference to the HorizontalMovement script on the Player; the HorizontalMovement script handles having the player move up and down the ladder, and needs a reference to whatever gameobject the Player is inside of that is acting as a ladder
        protected HorizontalMovement movement;
        //A quick bool that manages if the Player is inside the trigger collider of the ladder
        protected bool insideLadder;

        protected override void Initialization()
        {
            base.Initialization();
            edgeCollider = GetComponent<EdgeCollider2D>();
            movement = player.GetComponent<HorizontalMovement>();
            topOfLadder = new Vector3(transform.position.x, platformCollider.bounds.max.y);
            bottomOfLadder = new Vector3(transform.position.x, platformCollider.bounds.min.y);
            platformCollider.isTrigger = true;
        }

        //If the Player is inside the trigger collider of a ladder, then it sets up the logic to allow the Player to go up and down the ladder
        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject == player)
            {
                character.isOnLadder = true;
                movement.currentLadder = gameObject;
                insideLadder = true;
            }
        }

        //If the Player is outside of the trigger collider on the ladder, then it ensures the player no longer interacts with the ladder
        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject == player && !movement.above)
            {

                character.isOnLadder = false;
                movement.currentLadder = null;
                insideLadder = false;
            }
        }
    }
}