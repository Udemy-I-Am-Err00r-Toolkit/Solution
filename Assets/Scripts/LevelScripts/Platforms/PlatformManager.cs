using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlatformManager : Managers
    {
        protected BoxCollider2D platformCollider;
        protected Rigidbody2D platformRB;
        protected LayerMask playerLayer;

        protected override void Initialization()
        {
            base.Initialization();
            platformCollider = GetComponent<BoxCollider2D>();
            platformRB = GetComponent<Rigidbody2D>();
            playerLayer = LayerMask.GetMask("Player");
        }

        protected virtual void FixedUpdate()
        {
            CollisionCheck();
        }

        protected virtual bool CollisionCheck()
        {
            RaycastHit2D hit = Physics2D.BoxCast(new Vector2(platformCollider.bounds.center.x, platformCollider.bounds.max.y), new Vector2(platformCollider.bounds.size.x - .1f, .5f), 0, Vector2.up, .05f, playerLayer);
            if(hit)
            {
                return true;
            }
            return false;
        }
    }
}