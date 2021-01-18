using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class Dash : Abilities
    {
        //How much the player moves when dashing
        [SerializeField]
        protected float dashForce;
        //How long the player needs to wait before being allowed to dash after dashing
        [SerializeField]
        protected float dashCooldownTime;
        //How long the player dashes for
        [SerializeField]
        protected float dashAmountTime;
        //Layers the player can move through when dashing
        [SerializeField]
        protected LayerMask dashingLayers;

        //Local bool that determines if the player is allowed to dash based on dashCooldownTime
        private bool canDash;
        //Current amount of time passed since the player last dashed; this works with dashCooldownTime
        private float dashCountDown;
        //The exact collider of the Player; this is needed over Collider2D variable we have in Character script because we need to resize the collider
        private CapsuleCollider2D capsuleCollider2D;
        //The last position the player was standing before dashing; this is used to ensure if the player finishes dash somewhere they can't stand, they go back to this position.
        private Vector2 deltaPosition;

        protected override void Initialization()
        {
            base.Initialization();
            capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            Dashing();
        }

        //Handles all the logic that should happen when the player enters the dashing state; most of this is just toggling between two different states
        protected virtual void Dashing()
        {
            if (input.DashPressed() && canDash && !character.isCrouching && !gameManager.gamePaused)
            {
                deltaPosition = transform.position;
                dashCountDown = dashCooldownTime;
                character.isDashing = true;
                capsuleCollider2D.direction = CapsuleDirection2D.Horizontal;
                capsuleCollider2D.size = new Vector2(capsuleCollider2D.size.y, capsuleCollider2D.size.x);
                anim.SetBool("Dashing", true);
                StartCoroutine(FinishedDashing());
            }
        }

        protected virtual void FixedUpdate()
        {
            DashMode();
            ResetDashCounter();
        }

        //Handles all the logic when the player is currently in the dash state; unlike the Dashing() method, this doesn't determine if the player should be allowed to dash, but handles the logic of when the player is in the dash
        protected virtual void DashMode()
        {
            //Checks to see if the Player is currently in the dashing state
            if (character.isDashing)
            {
                //Makes sure player doesn't fall when dashing
                FallSpeed(0);
                //Makes sure movement is restricted when dashing
                movement.enabled = false;

                //Propels the player in the correct direction when dashing, and checks to see if the player collides with something while dashing
                if (!character.isFacingLeft)
                {
                    DashCollision(Vector2.right, .5f, dashingLayers);
                    rb.AddForce(Vector2.right * dashForce);
                }
                else
                {
                    DashCollision(Vector2.left, .5f, dashingLayers);
                    rb.AddForce(Vector2.left * dashForce);
                }
            }
        }

        //Checks to see what the player is colliding with while dashing; if it is colliding with something that is a dashLayer, then we handle the logic to allow the player to pass through those objects
        protected virtual void DashCollision(Vector2 direction, float distance, LayerMask collision)
        {
            RaycastHit2D[] hits = new RaycastHit2D[10];
            int numHits = col.Cast(direction, hits, distance);
            for (int i = 0; i < numHits; i++)
            {
                if ((1 << hits[i].collider.gameObject.layer & collision) != 0)
                {
                    hits[i].collider.enabled = false;
                    StartCoroutine(TurnColliderBackOn(hits[i].collider.gameObject));
                }
            }
        }

        //If the player is currently dashing, then the dashCountDown should be greater than zero, and this method starts negating that number; once it's less than zero again, the player is allowed to dash again.
        protected virtual void ResetDashCounter()
        {
            if (dashCountDown > 0)
            {
                canDash = false;
                dashCountDown -= Time.deltaTime;
            }
            else
                canDash = true;
        }

        //This manages when to set everything back to the original values they were in prior to the dash; most of this logic is toggling between two different states. This also makes sure that if the player can't stand where they when finished dashing, it teleports player back to somewhere where they can stand.
        protected virtual IEnumerator FinishedDashing()
        {
            yield return new WaitForSeconds(dashAmountTime);
            capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
            capsuleCollider2D.size = new Vector2(capsuleCollider2D.size.y, capsuleCollider2D.size.x);
            anim.SetBool("Dashing", false);
            character.isDashing = false;
            FallSpeed(1);
            movement.enabled = true;
            rb.velocity = new Vector2(0, rb.velocity.y);
            RaycastHit2D[] hits = new RaycastHit2D[10];
            yield return new WaitForSeconds(.1f);
            hits = Physics2D.CapsuleCastAll(new Vector2(col.bounds.center.x, col.bounds.center.y + .05f), new Vector2(col.bounds.size.x, col.bounds.size.y - .1f), CapsuleDirection2D.Vertical, 0, Vector2.zero, 0, jump.collisionLayer);
            if (hits.Length > 0)
            {
                transform.position = deltaPosition;
            }
        }

        //Turns the collider back on for objects that the Player can pass through when dashing.
        protected virtual IEnumerator TurnColliderBackOn(GameObject obj)
        {
            yield return new WaitForSeconds(dashAmountTime);
            obj.GetComponent<Collider2D>().enabled = true;
        }
    }
}