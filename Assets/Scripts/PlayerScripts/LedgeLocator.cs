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
        //The animation that should play when the player is climbing up a ledge
        public AnimationClip clip;
        //How much the player needs to move inwards when climbing up ledge to be standing on top of ledge
        public float climbingHorizontalOffset;

        //A quick reference to the top of the player to shoot a raycast and check for ledges
        private Vector2 topOfPlayer;
        //If the player is hanging from a ledge, this game object would be that ledge
        private GameObject ledge;
        //If you don't include an animation clip, this value will ensure this script will work and set a default value for how long it should take the player to climb on top of a ledge
        private float animationTime = .5f;
        //Ensures that if the Player needs to fall from a ledge, the Player doesn't automatically grab back onto the ledge when falling
        private bool falling;
        //A security bool that prevents the player from moving more than they should when they are hanging from a ledge and need to have the position adjusted
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
            //If there is an animation clip loaded into the clip variable, then we set the the animation time to that animation's length
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
            //First checks to see if the player is not falling from the ledge
            if (!falling)
            {
                //Checks for ledge when facing right
                if (transform.localScale.x > 0)
                {
                    //Sets the position for topOfPlayer for the raycast to check for a ledge
                    topOfPlayer = new Vector2(col.bounds.max.x + .1f, col.bounds.max.y);
                    //The raycast that checks for a ledge
                    RaycastHit2D hit = Physics2D.Raycast(topOfPlayer, Vector2.right, .2f);
                    //If the raycast hits a gameobject that containst the Ledge script, then it flows the logic in this if statement
                    if (hit && hit.collider.gameObject.GetComponent<Ledge>())
                    {
                        //Sets the ledge game object value to whatever platform the raycast hit
                        ledge = hit.collider.gameObject;
                        //This ensures the player is in the "sweet spot" to be hanging from a ledge by making sure the player is below the top of the ledge, but above the middle of the ledge
                        if (col.bounds.max.y < ledge.GetComponent<Collider2D>().bounds.max.y && col.bounds.max.y > ledge.GetComponent<Collider2D>().bounds.center.y)
                        {
                            character.grabbingLedge = true;
                            anim.SetBool("LedgeHanging", true);
                        }
                    }
                }
                //Does all the same stuff as above in this method, but checks in a leftward direction in stead of righward
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
                //Ensures that when the player is on a ledge, that we immedietly adjust the position if necassary, turn off gravity, and stop the player from moving horizontally
                if (ledge != null && character.grabbingLedge)
                {
                    AdjustPlayerPosition();
                    rb.velocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    GetComponent<HorizontalMovement>().enabled = false;
                }
                //If the player is no longer on a ledge, toggles the gravity back on and allows the player to move horizontally
                if (ledge != null && character.grabbingLedge && !character.isOnLadder)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    GetComponent<HorizontalMovement>().enabled = true;
                }
            }
        }

        protected virtual void LedgeHanging()
        {
            //If the Player is hanging from a ledge and the up button is pressed
            if (character.grabbingLedge && Input.GetAxis("Vertical") > 0)
            {
                //Stops playing the ledge hanging animation
                anim.SetBool("LedgeHanging", false);
                //If the Player is facing right, moves the player slightly righward so they are standing on top of the ledge
                if (transform.localScale.x > 0)
                {
                    StartCoroutine(ClimbingLedge(new Vector2(transform.position.x + climbingHorizontalOffset, ledge.GetComponent<Collider2D>().bounds.max.y), animationTime - .3f));
                }
                //If the Player is facing left, moves the player slightly leftward so they are standing on top of the ledge
                else
                {
                    StartCoroutine(ClimbingLedge(new Vector2(transform.position.x - climbingHorizontalOffset, ledge.GetComponent<Collider2D>().bounds.max.y), animationTime - .3f));
                }
            }
            //If the Player presses down when hanging from a ledge, allows the player to fall from the ledge
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

        //A method that Lerps the player position on top of the ledge, plays the animation to climb the ledge, and toggles everything to remove from a ledge hanging state
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

        //Snaps the Player into a position that would look more realistic when hanging from a ledge
        protected virtual void AdjustPlayerPosition()
        {
            //If the Player hasn't snapped to correct position, allows the logic in this method to flow; once player is snapped to position, this method is ignored
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

        //A very brief delay needed to allow the Player to fall from the ledge so they don't enter the falling state and then immediately snap back into the ledge hanging state
        protected virtual void NotFalling()
        {
            falling = false;
        }
    }
}
