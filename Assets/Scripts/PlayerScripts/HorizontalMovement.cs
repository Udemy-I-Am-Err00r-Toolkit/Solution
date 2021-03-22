using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will handle all the logic for horizontal movement, as well as vertical movement on ladders
namespace MetroidvaniaTools
{
    public class HorizontalMovement : Abilities
    {
        //How long it takes before player is moving at max speed.
        [SerializeField]
        protected float timeTillMaxSpeed;
        //The value of max speed.
        [SerializeField]
        protected float maxSpeed;
        //The value of speed during a sprinting state
        [SerializeField]
        protected float sprintMultiplier;
        //The value of speed during a crouching state
        [SerializeField]
        protected float crouchSpeedMultiplier;
        //How fast the player should move in a circular direction when grappling.
        [SerializeField]
        protected float hookSpeedMultiplier;
        //How fast the player will climb ladders.
        [SerializeField]
        protected float ladderSpeed;
        //If a player is on the ladder, this would be the gameobject the player is on as a ladder.
        [HideInInspector]
        public GameObject currentLadder;
        //A list of positions that would have the player transport back to if they fall out of level bounds.
        public List<Vector3> deltaPositions = new List<Vector3>();
        //Out of the list above, this is the best position to have the player transport back to.
        [HideInInspector]
        public Vector3 bestDeltaPosition;
        //A bool that lets other scripts know the player is on top of the ladder.
        protected bool above;
        //How quickly the player accelerates to reach max speed.
        private float acceleration;
        //What key is pressed to have the player move horizontally.
        private float horizontalInput;
        //The current amount of time the player has spent moving before reaching the max speed.
        private float runTime;

        private float deltaPositionCountdown = 1;
        private float deltaPositionCountdownCurrent = 0;

        private float currentSpeed;

        protected override void Initialization()
        {
            base.Initialization();
        }

        protected virtual void Update()
        {
            //MovementPressed();
        }


        //Check input for Horizontal Movement.
        public virtual bool MovementPressed()
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                horizontalInput = Input.GetAxis("Horizontal");
                return true;
            }
            else
                return false;
        }

        protected virtual void FixedUpdate()
        {
            Movement();
            RemoveFromGrapple();
            LadderMovement();
            PreviousGroundedPositions();
        }

        //Handles the logic for horizontally moving the player based on the input received from the MovementPressed() method.
        protected virtual void Movement()
        {
            //If game is paused, backs out of the method and doesn't allow you to move when game is unpaused
            if (gameManager.gamePaused)
            {
                return;
            }
            //Sets up movement based on what direction the player should move towards from key input
            if (MovementPressed())
            {
                anim.SetBool("Moving", true);
                acceleration = maxSpeed / timeTillMaxSpeed;
                runTime += Time.deltaTime;
                currentSpeed = horizontalInput * acceleration * runTime;
                CheckDirection();
            }
            //Sets up current speed when there is no longer any input detection for movement
            else
            {
                anim.SetBool("Moving", false);
                acceleration = 0;
                runTime = 0;
                currentSpeed = 0;
            }
            //Sets final speed based on the above logic in this method
            SpeedMultiplier();
            anim.SetFloat("CurrentSpeed", currentSpeed);
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
        }

        //Will rotate the player back to the normal position when no longer grappling.
        protected virtual void RemoveFromGrapple()
        {
            if (grapplingHook.removed)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, Time.deltaTime * 500);
                if (transform.rotation == Quaternion.identity)
                {
                    grapplingHook.removed = false;
                    rb.freezeRotation = true;
                }
            }
        }

        //Handles the vertical movement for ladders and restricts the player from falling while on a ladder and standing on top of ladder.
        protected virtual void LadderMovement()
        {
            //Double checks if the Player is within a ladder's trigger collider to allow climbing of ladder
            if (character.isOnLadder && currentLadder != null)
            {
                //Turns off gravity while on ladder
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                //Determines if the player is standing on top of the ladder
                if (col.bounds.min.y >= (currentLadder.GetComponent<Ladder>().topOfLadder.y - col.bounds.extents.y))
                {
                    anim.SetBool("OnLadder", false);
                    above = true;
                }
                else
                {
                    anim.SetBool("OnLadder", true);
                    above = false;
                }
                //If Player is in ladder trigger collider, makes the player ascend the ladder and plays animations for climbing the ladder
                if (input.UpHeld())
                {
                    anim.SetBool("ClimbingLadder", true);
                    transform.position = Vector2.MoveTowards(transform.position, currentLadder.GetComponent<Ladder>().topOfLadder, ladderSpeed * Time.deltaTime);
                    if (above)
                    {
                        anim.SetBool("ClimbingLadder", false);
                    }
                    return;
                }
                else
                    anim.SetBool("ClimbingLadder", false);
                //If Player is in ladder trigger collider, makes the player descend the ladder and plays animations for climbing the ladder
                if (input.DownHeld())
                {
                    anim.SetBool("ClimbingLadder", true);
                    transform.position = Vector2.MoveTowards(transform.position, currentLadder.GetComponent<Ladder>().bottomOfLadder, ladderSpeed * Time.deltaTime);
                    return;
                }
            }
            //If the player is not in a Ladder trigger collider, then ensures gravity is back on and stops playing the OnLadder animation
            else
            {
                anim.SetBool("OnLadder", false);
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        //Checks to see where the player was last standing to place the player on a safe spot if they fall off
        //out of the level bounds.
        protected virtual void PreviousGroundedPositions()
        {
            if (character.isGrounded && MovementPressed())
            {
                deltaPositionCountdownCurrent -= Time.deltaTime;
                if (deltaPositionCountdownCurrent < 0)
                {
                    if (deltaPositions.Count == 10)
                    {
                        deltaPositions.RemoveAt(0);
                    }
                    deltaPositions.Add(transform.position);
                    deltaPositionCountdownCurrent = deltaPositionCountdown;
                    bestDeltaPosition = deltaPositions[0];
                }
            }
        }

        //Checks to see what direction the player should be facing based on movement.
        protected virtual void CheckDirection()
        {
            //If the current speed is greater than 0, then player is moving right
            if (currentSpeed > 0)
            {
                //If the character is currently facing left while moving right, we need to call the Flip() method on the character script
                if (character.isFacingLeft)
                {
                    character.isFacingLeft = false;
                    Flip();
                }
                //If the currentSpeed is greater than maxSpeed, limit currentSpeed to maxSpeed
                if (currentSpeed > maxSpeed)
                {
                    currentSpeed = maxSpeed;
                }
            }
            //The same as everything above in this method, but for moving left
            if (currentSpeed < 0)
            {
                if (!character.isFacingLeft)
                {
                    character.isFacingLeft = true;
                    Flip();
                }
                if (currentSpeed < -maxSpeed)
                {
                    currentSpeed = -maxSpeed;
                }
            }
        }


        //Handles different states in which we need to adjust the horizontal speed.
        protected virtual void SpeedMultiplier()
        {
            //If sprinting, multiply currentSpeed with how fast the sprintMultiplier is
            if (input.SprintingHeld())
            {
                currentSpeed *= sprintMultiplier;
            }
            //If crouching, multiply currentSpeed with how fast the crouchSpeedMultiplier is
            if (character.isCrouching)
            {
                currentSpeed *= crouchSpeedMultiplier;
            }
            //If connected to a grappling hook, restricts movement until player is no longer grappling
            if (grapplingHook.connected)
            {
                //Ensures that when grappling, the Player isn't affected by collisions and can continue grappling
                if (input.UpHeld() || input.DownHeld() || CollisionCheck(Vector2.right, .1f, jump.collisionLayer) || CollisionCheck(Vector2.left, .1f, jump.collisionLayer) || CollisionCheck(Vector2.down, .1f, jump.collisionLayer) || CollisionCheck(Vector2.up, .1f, jump.collisionLayer) || character.isGrounded)
                {
                    return;
                }
                //Because the grappling movement is totally different from regular groudned and jumping movement, we handle all the logic for moving the player here
                currentSpeed *= hookSpeedMultiplier;
                if (grapplingHook.hookTrail.transform.position.y > grapplingHook.objectConnectedTo.transform.position.y)
                {
                    currentSpeed *= -hookSpeedMultiplier;
                }
                //Makes sure player rotates around while grappling
                rb.rotation -= currentSpeed;
            }
            //Limits horizontal movement while wall sliding
            if (character.isWallSliding)
            {
                currentSpeed = 0;
            }
            //Makes sure the player doesn't stick to a wall like velcro when jumping into it; this doesn't apply to oneway platforms and ladders, only platforms that would normally not allow the Player to pass through
            if (currentPlatform != null && (!currentPlatform.GetComponent<OneWayPlatform>() || !currentPlatform.GetComponent<Ladder>()))
            {
                if (!character.isFacingLeft && CollisionCheck(Vector2.right, .05f, jump.collisionLayer) || character.isFacingLeft && CollisionCheck(Vector2.left, .05f, jump.collisionLayer))
                {
                    currentSpeed = .01f;
                }
            }
        }
    }
}