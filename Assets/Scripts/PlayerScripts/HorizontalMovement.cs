using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class HorizontalMovement : Abilities
    {
        [SerializeField]
        protected float timeTillMaxSpeed;
        [SerializeField]
        protected float maxSpeed;
        [SerializeField]
        protected float sprintMultiplier;
        [SerializeField]
        protected float crouchSpeedMultiplier;
        [SerializeField]
        protected float hookSpeedMultiplier;
        [SerializeField]
        protected float ladderSpeed;
        [HideInInspector]
        public GameObject currentLadder;
        public List<Vector3> deltaPositions = new List<Vector3>();
        [HideInInspector]
        public Vector3 bestDeltaPosition;
        [HideInInspector]
        public bool above;
        private float acceleration;
        private float horizontalInput;
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
            MovementPressed();
        }


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

        protected virtual void Movement()
        {
            if(gameManager.gamePaused)
            {
                return;
            }
            if(MovementPressed())
            {
                anim.SetBool("Moving", true);
                acceleration = maxSpeed / timeTillMaxSpeed;
                runTime += Time.deltaTime;
                currentSpeed = horizontalInput * acceleration * runTime;
                CheckDirection();
            }
            else
            {
                anim.SetBool("Moving", false);
                acceleration = 0;
                runTime = 0;
                currentSpeed = 0;
            }
            SpeedMultiplier();
            anim.SetFloat("CurrentSpeed", currentSpeed);
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
        }

        protected virtual void RemoveFromGrapple()
        {
            if(grapplingHook.removed)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, Time.deltaTime * 500);
                if(transform.rotation == Quaternion.identity)
                {
                    grapplingHook.removed = false;
                    rb.freezeRotation = true;
                }
            }
        }

        protected virtual void LadderMovement()
        {
            if(character.isOnLadder && currentLadder != null)
            { 
                FallSpeed(0);
                if((transform.position.y + col.bounds.size.y) + .15f > currentLadder.GetComponent<Ladder>().topOfLadder.y)
                {
                    Debug.Log("Above");
                    above = true;
                }
                else
                {
                    above = false;
                }
                if(input.UpHeld())
                {
                    anim.SetBool("OnLadder", true);
                    anim.SetBool("ClimbingLadder", true);
                    transform.position = Vector2.MoveTowards(transform.position, currentLadder.GetComponent<Ladder>().topOfLadder, ladderSpeed * Time.deltaTime);
                    if(above)
                    {
                        anim.SetBool("OnLadder", false);
                        anim.SetBool("ClimbingLadder", false);
                        currentLadder.GetComponent<Ladder>().edgeCollider.isTrigger = true;
                    }
                    return;
                }
                else
                    anim.SetBool("ClimbingLadder", false);
                if (input.DownHeld())
                {
                    anim.SetBool("OnLadder", true);
                    anim.SetBool("ClimbingLadder", true);
                    transform.position = Vector2.MoveTowards(transform.position, currentLadder.GetComponent<Ladder>().bottomOfLadder, ladderSpeed * Time.deltaTime);
                    if(above)
                    {
                        currentLadder.GetComponent<Ladder>().edgeCollider.isTrigger = true;
                    }
                    return;
                }
                else
                    anim.SetBool("ClimbingLadder", false);
                currentLadder.GetComponent<Ladder>().edgeCollider.isTrigger = false;
            }
            else
            {
                anim.SetBool("OnLadder", false) ;
                 FallSpeed(1);
            }
        }

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

        protected virtual void CheckDirection()
        {
            if (currentSpeed > 0)
            {
                if(character.isFacingLeft)
                {
                    character.isFacingLeft = false;
                    Flip();
                }
                if (currentSpeed > maxSpeed)
                {
                    currentSpeed = maxSpeed;
                }
            }
            if (currentSpeed < 0)
            {
                if(!character.isFacingLeft)
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

        protected virtual void SpeedMultiplier()
        {
            if (input.SprintingHeld())
            {
                currentSpeed *= sprintMultiplier;
            }
            if(character.isCrouching)
            {
                currentSpeed *= crouchSpeedMultiplier;
            }
            if(grapplingHook.connected)
            {
                if(input.UpHeld() || input.DownHeld() || CollisionCheck(Vector2.right, .1f, jump.collisionLayer) || CollisionCheck(Vector2.left, .1f, jump.collisionLayer) || CollisionCheck(Vector2.down, .1f, jump.collisionLayer) || CollisionCheck(Vector2.up, .1f, jump.collisionLayer) || character.isGrounded)
                {
                    return;
                }
                currentSpeed *= hookSpeedMultiplier;
                if(grapplingHook.hookTrail.transform.position.y > grapplingHook.objectConnectedTo.transform.position.y)
                {
                    currentSpeed *= -hookSpeedMultiplier;
                }
                rb.rotation -= currentSpeed;
            }
            if(character.isWallSliding)
            {
                currentSpeed = 0;
            }
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