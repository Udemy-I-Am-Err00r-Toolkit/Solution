using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //Very robust script that manages every single different movement the Enemy is allowed to make; you can combine a lot of these movement types to create different movements for different enemies. This script does A LOT and is one of the main brains for AI
    public class EnemyMovement : AIManagers
    {
        //Different base types of movement; the enemy either moves normally, hugs wall when they run into them, or is flying
        [SerializeField]
        protected enum MovementType { Normal, HugWalls, Flying }
        [SerializeField]
        protected MovementType type;
        //Determines if AI should load into the scene facing left
        [SerializeField]
        protected bool spawnFacingLeft;
        //Determines if AI should turn around when running into something; most likely this will be set to true, but give you the option to set that if you want
        [SerializeField]
        protected bool turnAroundOnCollision;
        //Determines if AI should turn around if there is a gap in the floor or if the AI should fall through the hole
        [SerializeField]
        protected bool avoidFalling;
        //Determines if the AI should be able to jump at certain intervals
        [SerializeField]
        protected bool jump;
        //Quick bool that makes sure the Enemy is standing still and is usually toggled in between jumps and if the Enemy is too close to the Player
        public bool standStill;
        //How long it takes an Enemy to reach max speed after moving
        [SerializeField]
        protected float timeTillMaxSpeed;
        //How fast the Enemy can possibly go
        [SerializeField]
        protected float maxSpeed;
        //How high the Enemy can jump if allowed to jump
        [SerializeField]
        protected float jumpVerticalForce;
        //How far in front of the Enemy it can jump if allowed to jump
        [SerializeField]
        protected float jumpHorizontalForce;
        //How close the Enemy can get to the Player before stopping
        [SerializeField]
        protected float minDistance;
        //What layers the Enemy can turn around on if it runs into a wall
        [SerializeField]
        protected LayerMask collidersToTurnAroundOn;
        //This is for the HugWalls movement type and makes sure the Enemy rotates around the object they are colliding with
        [HideInInspector]
        public bool turn;

        //Ensures that when an Enemy is being loaded at Start, no collisions are detected
        private bool spawning = true;
        private float acceleration;
        //A quick value that helps navigate the Enemy in the correct direction based on speed
        private float direction;
        private float runTime;

        //This is for the HugWalls movement type and makes sure the Enemy isn't moving while turning
        protected bool wait;
        protected bool wasJumping;
        //This quick float ensures the AI doesn't get stuck in a rapid state of going back and forth when it needs to change directions for whatever reason
        protected float originalWaitTime = .1f;
        protected float currentWaitTime;
        protected float currentSpeed;

        protected override void Initialization()
        {
            base.Initialization();
            if (spawnFacingLeft)
            {
                enemyCharacter.facingLeft = true;
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            }
            currentWaitTime = originalWaitTime;
            timeTillDoAction = originalTimeTillDoAction;
            //Ensures the scene is loaded before the Enemy moves
            Invoke("Spawning", .01f);
        }

        protected virtual void FixedUpdate()
        {
            Movement();
            CheckGround();
            EdgeOfFloor();
            HugWalls();
            Jumping();
            FollowPlayer();
            HandleWait();
        }

        //Handles movement for Normal and Flying enemies
        protected virtual void Movement()
        {
            //This ensures if the Enemy is flying, it never has gravity applied to it
            if (type == MovementType.Flying)
            {
                rb.gravityScale = 0;
            }
            //If the enemy is facing right
            if (!enemyCharacter.facingLeft)
            {
                //Direction is set to one so math is applied in a positive fashion
                direction = 1;
                //Checks to see if the Enemy is colliding with something and should turn around when it does; a lot of the other checks in this are just to ensure the Enemy doesn't enter a looping phase where it keeps changing directions
                if (CollisionCheck(Vector2.right, .5f, collidersToTurnAroundOn) && turnAroundOnCollision && !wasJumping && !spawning || (enemyCharacter.followPlayer && player.transform.position.x < transform.position.x))
                {
                    //Flips the Enemy if it should turn around
                    enemyCharacter.facingLeft = true;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    if (standStill)
                    {
                        rb.velocity = new Vector2(-jumpHorizontalForce, rb.velocity.y);
                    }
                }
            }
            //Same logic as the above if statement block, but for leftward direction
            else
            {
                direction = -1;
                if (CollisionCheck(Vector2.left, .5f, collidersToTurnAroundOn) && turnAroundOnCollision && !wasJumping && !spawning || (enemyCharacter.followPlayer && player.transform.position.x > transform.position.x))
                {
                    enemyCharacter.facingLeft = false;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    if (standStill)
                    {
                        rb.velocity = new Vector2(jumpHorizontalForce, rb.velocity.y);
                    }
                }
            }
            //Handles a lot of similar logic we have the HorizontalMovement script, but for the Enemy
            acceleration = maxSpeed / timeTillMaxSpeed;
            runTime += Time.deltaTime;
            currentSpeed = direction * acceleration * runTime;
            CheckSpeed();
            if (!standStill && !enemyCharacter.followPlayer)
            {
                rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
            }
        }

        //This method will have the Enemy move towards the Player if that is what should happen
        protected virtual void FollowPlayer()
        {
            if (enemyCharacter.followPlayer)
            {
                bool tooClose = new bool();
                //This is the math needed to check the distance of the Player from the enemy, and if that distance is less than minDistance, then the tooClose bool is set to true
                if (Mathf.Abs(transform.position.x - player.transform.position.x) < minDistance)
                {
                    tooClose = true;
                }
                else
                {
                    tooClose = false;
                }
                //Handles moving the Enemy towards the Player; if the tooClose bool is true, it stops movement here too
                if (!enemyCharacter.facingLeft)
                {
                    Vector2 distanceToPlayer = (new Vector3(transform.position.x - 2, transform.position.y) - player.transform.position).normalized * minDistance + player.transform.position;
                    transform.position = Vector2.MoveTowards(transform.position, distanceToPlayer, currentSpeed * Time.deltaTime);
                    if (tooClose)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
                //Same logic as if statement block above, but for checking left
                else
                {
                    Vector2 distanceToPlayer = (new Vector3(transform.position.x + 2, transform.position.y) - player.transform.position).normalized * minDistance + player.transform.position;
                    transform.position = Vector2.MoveTowards(transform.position, distanceToPlayer, -currentSpeed * Time.deltaTime);
                    if (tooClose)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
            }
        }

        //Limits the Enemy speed to maxSpeed
        protected virtual void CheckSpeed()
        {
            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
            if (currentSpeed < -maxSpeed)
            {
                currentSpeed = -maxSpeed;
            }
        }

        //This method uses the raycasts found on the EnemyCharacter script to determine if there are gaps in the floor and hanldes logic for turning the Enemy around if there is a gap in the floor
        protected virtual void EdgeOfFloor()
        {
            if (rayHitNumber == 1 && avoidFalling && !wait && type == MovementType.Normal)
            {
                currentWaitTime = originalWaitTime;
                wait = true;
                enemyCharacter.facingLeft = !enemyCharacter.facingLeft;
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            }
        }

        //This method manages having the Enemy jump based on the timeTillDoAction value and what direction the Enemy is facing; this method will only run if the Movement type is Normal, the jump bool is set to true in the inspector, and the Enemy is grounded
        protected virtual void Jumping()
        {
            if (type == MovementType.Normal)
            {
                if (rayHitNumber > 0 && jump)
                {
                    timeTillDoAction -= Time.deltaTime;
                    if (timeTillDoAction <= 0)
                    {
                        rb.AddForce(Vector2.up * jumpVerticalForce);
                        if (!enemyCharacter.facingLeft)
                        {
                            rb.velocity = new Vector2(jumpHorizontalForce, rb.velocity.y);
                        }
                        else
                        {
                            rb.velocity = new Vector2(-jumpHorizontalForce, rb.velocity.y);
                        }
                    }
                }
                if (rayHitNumber > 0 && rb.velocity.y < 0)
                {
                    wasJumping = true;
                    if (standStill)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                    timeTillDoAction = originalTimeTillDoAction;
                    Invoke("NoLongerInTheAir", .5f);
                }
            }
        }

        //Quick bool that helps manage the Jumping method from performing multiple jumps as it already jumped
        protected virtual void NoLongerInTheAir()
        {
            wasJumping = false;
        }

        //Quick bool that restricts the movement of the Enemy while the scene is loading
        protected virtual void Spawning()
        {
            spawning = false;
        }

        //The method that manages flipping and rotating the Enemy around game objects that it is connected to; this method only runs if the HugWalls movement type is selected. Most of this method handles all the different directions and rotation values the Enemy would encounter while moving
        //Not recomened to change anything in this method, this method is very fragile and can break easily
        protected virtual void HugWalls()
        {
            if (type == MovementType.HugWalls)
            {
                turnAroundOnCollision = false;
                float newZValue = transform.localEulerAngles.z;
                rb.gravityScale = 0;
                if (rayHitNumber == 1 && !wait)
                {
                    wait = true;
                    currentWaitTime = originalWaitTime;
                    rb.velocity = Vector2.zero;
                    if (!enemyCharacter.facingLeft)
                    {
                        transform.localEulerAngles = new Vector3(0, 0, newZValue - 90);
                    }
                    else
                    {
                        transform.localEulerAngles = new Vector3(0, 0, newZValue + 90);
                    }
                }
                if (turn && !wait)
                {
                    wait = true;
                    currentWaitTime = originalWaitTime;
                    rb.velocity = Vector2.zero;
                    if (!enemyCharacter.facingLeft)
                    {
                        transform.eulerAngles = new Vector3(0, 0, newZValue + 90);
                        if (Mathf.Round(transform.eulerAngles.z) == 0)
                        {
                            transform.position = new Vector2(transform.position.x, transform.position.y - (transform.localScale.x * .5f));
                        }
                        if (Mathf.Round(transform.eulerAngles.z) == 90)
                        {
                            transform.position = new Vector2(transform.position.x + (transform.localScale.x * .5f), transform.position.y);
                        }
                        if (Mathf.Round(transform.eulerAngles.z) == 180)
                        {
                            transform.position = new Vector2(transform.position.x, transform.position.y + (transform.localScale.x * .5f));
                        }
                        if (Mathf.Round(transform.eulerAngles.z) == 270)
                        {
                            transform.position = new Vector2(transform.position.x - (transform.localScale.x * .5f), transform.position.y);
                        }
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, 0, newZValue - 90);
                        if (Mathf.Round(transform.eulerAngles.z) == 0)
                        {
                            transform.position = new Vector2(transform.position.x, transform.position.y + (transform.localScale.x * .5f));
                        }
                        if (Mathf.Round(transform.eulerAngles.z) == 90)
                        {
                            transform.position = new Vector2(transform.position.x - (transform.localScale.x * .5f), transform.position.y);
                        }
                        if (Mathf.Round(transform.eulerAngles.z) == 180)
                        {
                            transform.position = new Vector2(transform.position.x, transform.position.y - (transform.localScale.x * .5f));
                        }
                        if (Mathf.Round(transform.eulerAngles.z) == 270)
                        {
                            transform.position = new Vector2(transform.position.x + (transform.localScale.x * .5f), transform.position.y);
                        }
                    }
                }
                if (Mathf.Round(transform.eulerAngles.z) == 0)
                {
                    rb.velocity = new Vector2(currentSpeed, 0);
                }
                if (Mathf.Round(transform.eulerAngles.z) == 90)
                {
                    rb.velocity = new Vector2(0, currentSpeed);
                }
                if (Mathf.Round(transform.eulerAngles.z) == 180)
                {
                    rb.velocity = new Vector2(-currentSpeed, 0);
                }
                if (Mathf.Round(transform.eulerAngles.z) == 270)
                {
                    rb.velocity = new Vector2(0, -currentSpeed);
                }
                if (rayHitNumber == 0 && !wait)
                {
                    transform.localEulerAngles = Vector3.zero;
                    rb.gravityScale = 1;
                }
            }
        }

        //Helps ensure the Enemy doesn't go into a repetitive phase where they are flipping and rotating constanty when they should flip and rotate
        protected virtual void HandleWait()
        {
            currentWaitTime -= Time.deltaTime;
            if (currentWaitTime <= 0)
            {
                wait = false;
                currentWaitTime = 0;
            }
        }
    }
}