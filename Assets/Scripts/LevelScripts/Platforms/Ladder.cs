using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    [RequireComponent(typeof(EdgeCollider2D))]
    public class Ladder : PlatformManager
    {
        [HideInInspector]
        public Vector3 topOfLadder;
        [HideInInspector]
        public Vector3 bottomOfLadder;
        [HideInInspector]
        public EdgeCollider2D edgeCollider;

        protected HorizontalMovement movement;
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


        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if(collision.gameObject == player)
            {
                character.isOnLadder = true;
                movement.currentLadder = gameObject;
                insideLadder = true;
            }
        }

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