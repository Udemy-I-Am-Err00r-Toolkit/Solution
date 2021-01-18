using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script will allow an Enemy to detect a player based on different range views and sizes
    public class PlayerDetector : AIManagers
    {
        //If you want the Enemy to be able to see the Player in a Rectangular or Circular shape
        [SerializeField]
        protected enum DetectionType { Rectangle, Circle }
        [SerializeField]
        protected DetectionType type;
        //If the Player is within range of the shapes above, this bool will handle if the Enemy should follow the Player
        [SerializeField]
        protected bool followPlayerIfFound;
        //How much range a Rectangular shape has to detect the Player
        [SerializeField]
        protected float distance;
        //How much range a Circular shape has to detect the Player
        [SerializeField]
        protected float radius;
        //How much the shape needs to be adjusted away from the Enemy to better fit the AI type
        [SerializeField]
        protected Vector2 detectorOffset;
        //The layers the Enemy would need to detect for; most likely this will just be the Player layer
        [SerializeField]
        protected LayerMask layer;

        protected virtual void FixedUpdate()
        {
            DetectPlayer();

        }

        //This method will handle all the logic for locating the player based on the shape and setup certain parameters for the EnemyMovement script; most of this is just checking if the player is inside the shape and if the Enemy should follow the Player while the Player is in that shape
        protected virtual void DetectPlayer()
        {
            RaycastHit2D hit;
            if (type == DetectionType.Rectangle)
            {
                if (!enemyCharacter.facingLeft)
                {
                    hit = Physics2D.BoxCast(new Vector2(transform.position.x + col.bounds.extents.x + detectorOffset.x + (distance * .5f), col.bounds.center.y), new Vector2(distance, col.bounds.size.y + detectorOffset.y), 0, Vector2.zero, 0, layer);
                }
                else
                {
                    hit = Physics2D.BoxCast(new Vector2(transform.position.x - col.bounds.extents.x - detectorOffset.x - (distance * .5f), col.bounds.center.y), new Vector2(distance, col.bounds.size.y + detectorOffset.y), 0, Vector2.zero, 0, layer);
                }
                if (hit)
                {
                    if (followPlayerIfFound)
                    {
                        enemyCharacter.followPlayer = true;
                    }
                    enemyCharacter.playerIsClose = true;
                }
                else
                {
                    enemyCharacter.followPlayer = false;
                    enemyCharacter.playerIsClose = false;
                    if (enemyMovement.standStill)
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
            }
            if (type == DetectionType.Circle)
            {
                hit = Physics2D.CircleCast(col.bounds.center, radius, Vector2.zero, 0, layer);
                if (hit)
                {
                    if (followPlayerIfFound)
                    {
                        enemyCharacter.followPlayer = true;
                    }
                    enemyCharacter.playerIsClose = true;
                }
                else
                {
                    enemyCharacter.followPlayer = false;
                    enemyCharacter.playerIsClose = false;
                    if (enemyMovement.standStill)
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
            }
        }

        //This mehtod visually draws the different shapes so you can see and play test more easily
        private void OnDrawGizmos()
        {
            col = GetComponent<Collider2D>();
            if (type == DetectionType.Rectangle)
            {
                Gizmos.color = Color.red;
                if (transform.localScale.x > 0)
                {
                    Gizmos.DrawWireCube(new Vector2(transform.position.x + col.bounds.extents.x + detectorOffset.x + (distance * .5f), col.bounds.center.y + detectorOffset.y), new Vector2(distance, col.bounds.size.y));
                }
                else
                {
                    Gizmos.DrawWireCube(new Vector2(transform.position.x - col.bounds.extents.x - detectorOffset.x - (distance * .5f), col.bounds.center.y + detectorOffset.y), new Vector2(distance, col.bounds.size.y));
                }
            }
            if (type == DetectionType.Circle)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(col.bounds.center, radius);
            }
        }
    }
}