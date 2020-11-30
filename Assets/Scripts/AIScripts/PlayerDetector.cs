using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class PlayerDetector : AIManagers
    {
        [SerializeField]
        protected enum DetectionType { Rectangle, Circle  }
        [SerializeField]
        protected DetectionType type;
        [SerializeField]
        protected bool followPlayerIfFound;
        [SerializeField]
        protected float distance;
        [SerializeField]
        protected float radius;
        [SerializeField]
        protected Vector2 detectorOffset;
        [SerializeField]
        protected LayerMask layer;

        protected virtual void FixedUpdate()
        {
            DetectPlayer();
           
        }

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