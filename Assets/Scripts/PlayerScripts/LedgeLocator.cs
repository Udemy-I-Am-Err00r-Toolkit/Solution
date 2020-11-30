/*This script is part of a larger solution, but can be used as a standalone
 * script; if you want to use this as a standalone script, make sure everything
 * that is commented off in psuedocode is no longer commented out, and that
 * you delete the Initialization method found within this script, as well as
 * change every reference of character.grabbingLedge to just grabbingLedge.
 
   If you do want to use this with my larger solution, then you don't need to 
   make any changes to this script
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{

    public class LedgeLocator : Abilities
    {
        public AnimationClip clip;
        public float climbingHorizontalOffset;

        private Vector2 topOfPlayer;
        private GameObject ledge;
        private float animationTime = .5f;
        private bool falling;
        private bool moved;

        /*
        [HideInInspector]
        public bool grabbingLedge;
        private Collider2D col;
        private Rigidbody2D rb;
        private Animator anim;

        private void Start()
        {
            col = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            if (clip != null)
            {
                animationTime = clip.length;
            }
        }
        */

        protected override void Initialization()
        {
            base.Initialization();
            if (clip != null)
            {
                animationTime = clip.length;
            }
        }

        protected virtual void FixedUpdate()
        {
            CheckForLedge();
            LedgeHanging();
        }

        protected virtual void CheckForLedge()
        {
            if (!falling)
            {
                if (transform.localScale.x > 0)
                {
                    topOfPlayer = new Vector2(col.bounds.max.x + .1f, col.bounds.max.y);
                    RaycastHit2D hit = Physics2D.Raycast(topOfPlayer, Vector2.right, .2f);
                    if (hit && hit.collider.gameObject.GetComponent<Ledge>())
                    {
                        ledge = hit.collider.gameObject;
                        if (col.bounds.max.y < ledge.GetComponent<Collider2D>().bounds.max.y && col.bounds.max.y > ledge.GetComponent<Collider2D>().bounds.center.y)
                        {
                            character.grabbingLedge = true;
                            anim.SetBool("LedgeHanging", true);
                        }
                    }
                }
                else
                {
                    topOfPlayer = new Vector2(col.bounds.min.x - .1f, col.bounds.max.y);
                    RaycastHit2D hit = Physics2D.Raycast(topOfPlayer, Vector2.left, .2f);
                    if (hit && hit.collider.gameObject.GetComponent<Ledge>())
                    {
                        ledge = hit.collider.gameObject;
                        if (col.bounds.max.y < ledge.GetComponent<Collider2D>().bounds.max.y && col.bounds.max.y > ledge.GetComponent<Collider2D>().bounds.center.y)
                        {
                            anim.SetBool("LedgeHanging", true);
                            character.grabbingLedge = true;
                        }
                    }
                }
                if (ledge != null && character.grabbingLedge)
                {
                    AdjustPlayerPosition();
                    rb.velocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    GetComponent<HorizontalMovement>().enabled = false;
                }
                else
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    GetComponent<HorizontalMovement>().enabled = true;
                }
            }
        }

        protected virtual void LedgeHanging()
        {
            if (character.grabbingLedge && Input.GetAxis("Vertical") > 0)
            {
                anim.SetBool("LedgeHanging", false);
                if (transform.localScale.x > 0)
                {
                    StartCoroutine(ClimbingLedge(new Vector2(transform.position.x + climbingHorizontalOffset, ledge.GetComponent<Collider2D>().bounds.max.y), animationTime - .3f));
                }
                else
                {
                    StartCoroutine(ClimbingLedge(new Vector2(transform.position.x - climbingHorizontalOffset, ledge.GetComponent<Collider2D>().bounds.max.y), animationTime - .3f));
                }
            }
            if (character.grabbingLedge && Input.GetAxis("Vertical") < 0)
            {
                ledge = null;
                moved = false;
                character.grabbingLedge = false;
                anim.SetBool("LedgeHanging", false);
                falling = true;
                rb.bodyType = RigidbodyType2D.Dynamic;
                GetComponent<HorizontalMovement>().enabled = true;
                Invoke("NotFalling", .5f);
            }
        }

        protected virtual IEnumerator ClimbingLedge(Vector2 topOfPlatform, float duration)
        {
            float time = 0;
            Vector2 startValue = transform.position;
            while (time < duration)
            {
                anim.SetBool("LedgeClimbing", true);
                transform.position = Vector2.Lerp(startValue, topOfPlatform, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            ledge = null;
            moved = false;
            character.grabbingLedge = false;
            anim.SetBool("LedgeClimbing", false);
        }

        protected virtual void AdjustPlayerPosition()
        {
            if (!moved)
            {
                moved = true;
                if (transform.localScale.x > 0)
                {
                    transform.position = new Vector2((ledge.GetComponent<Collider2D>().bounds.min.x - col.bounds.extents.x) + ledge.GetComponent<Ledge>().hangingHorizontalOffset, (ledge.GetComponent<Collider2D>().bounds.max.y - col.bounds.size.y - .5f) + ledge.GetComponent<Ledge>().hangingVerticalOffset);
                }
                else
                {
                    transform.position = new Vector2((ledge.GetComponent<Collider2D>().bounds.max.x + col.bounds.extents.x) - ledge.GetComponent<Ledge>().hangingHorizontalOffset, (ledge.GetComponent<Collider2D>().bounds.max.y - col.bounds.size.y - .5f) + ledge.GetComponent<Ledge>().hangingVerticalOffset);
                }
            }
        }

        protected virtual void NotFalling()
        {
            falling = false;
        }
    }
}
