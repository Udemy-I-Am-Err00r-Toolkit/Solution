using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class EnemyMovement : AIManagers
    {
        [SerializeField]
        protected enum MovementType { Normal, HugWalls, Flying }
        [SerializeField]
        protected MovementType type;
        [SerializeField]
        protected bool spawnFacingLeft;
        [SerializeField]
        protected bool turnAroundOnCollision;
        [SerializeField]
        protected bool avoidFalling;
        [SerializeField]
        protected bool jump;
        public bool standStill;
        [SerializeField]
        protected float timeTillMaxSpeed;
        [SerializeField]
        protected float maxSpeed;
        [SerializeField]
        protected float jumpVerticalForce;
        [SerializeField]
        protected float jumpHorizontalForce;
        [SerializeField]
        protected float minDistance;
        [SerializeField]
        protected LayerMask collidersToTurnAroundOn;
        [HideInInspector]
        public bool turn;

        private bool spawning = true;
        private float acceleration;
        private float direction;
        private float runTime;

        protected bool wait;
        protected bool wasJumping;
        protected float originalWaitTime = .1f;
        protected float currentWaitTime;
        protected float currentSpeed; 

        protected override void Initialization()
        {
            base.Initialization();
            if(spawnFacingLeft)
            {
                enemyCharacter.facingLeft = true;
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            }
            currentWaitTime = originalWaitTime;
            timeTillDoAction = originalTimeTillDoAction;
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

        protected virtual void Movement()
        {
            if(type == MovementType.Flying)
            {
                rb.gravityScale = 0;
            }
            if(!enemyCharacter.facingLeft)
            {
                direction = 1;
                if(CollisionCheck(Vector2.right, .5f, collidersToTurnAroundOn) && turnAroundOnCollision && !wasJumping && !spawning || (enemyCharacter.followPlayer && player.transform.position.x < transform.position.x))
                {
                    enemyCharacter.facingLeft = true;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    if(standStill)
                    {
                        rb.velocity = new Vector2(-jumpHorizontalForce, rb.velocity.y);
                    }
                }
            }
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
            acceleration = maxSpeed / timeTillMaxSpeed;
            runTime += Time.deltaTime;
            currentSpeed = direction * acceleration * runTime;
            CheckSpeed();
            if (!standStill && !enemyCharacter.followPlayer)
            {
                rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
            }
        }

        protected virtual void FollowPlayer()
        {
            if (enemyCharacter.followPlayer)
            {
                bool tooClose = new bool();
                if (Mathf.Abs(transform.position.x - player.transform.position.x) < minDistance)
                {
                    tooClose = true;
                }
                else
                {
                    tooClose = false;
                }
                if (!enemyCharacter.facingLeft)
                {
                    Vector2 distanceToPlayer = (new Vector3(transform.position.x - 2, transform.position.y) - player.transform.position).normalized * minDistance + player.transform.position;
                    transform.position = Vector2.MoveTowards(transform.position, distanceToPlayer, currentSpeed * Time.deltaTime);
                    if (tooClose)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
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


        protected virtual void CheckSpeed()
        {
            if(currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
            if(currentSpeed < -maxSpeed)
            {
                currentSpeed = -maxSpeed;
            }
        }

        protected virtual void EdgeOfFloor()
        {
            if(rayHitNumber == 1 && avoidFalling && !wait && type == MovementType.Normal)
            {
                currentWaitTime = originalWaitTime;
                wait = true;
                enemyCharacter.facingLeft = !enemyCharacter.facingLeft;
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y); 
            }
        }

        protected virtual void Jumping()
        {
            if(type == MovementType.Normal)
            {
                if(rayHitNumber > 0 && jump)
                {
                    timeTillDoAction -= Time.deltaTime;
                    if(timeTillDoAction <= 0)
                    {
                        rb.AddForce(Vector2.up * jumpVerticalForce);
                        if(!enemyCharacter.facingLeft)
                        {
                            rb.velocity = new Vector2(jumpHorizontalForce, rb.velocity.y);
                        }
                        else
                        {
                            rb.velocity = new Vector2(-jumpHorizontalForce, rb.velocity.y);
                        }
                    }
                }
                if(rayHitNumber > 0 && rb.velocity.y < 0)
                {
                    wasJumping = true;
                    if(standStill)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                    timeTillDoAction = originalTimeTillDoAction;
                    Invoke("NoLongerInTheAir", .5f);
                }
            }
        }

        protected virtual void NoLongerInTheAir()
        {
            wasJumping = false;
        }

        protected virtual void Spawning()
        {
            spawning = false;
        }

        protected virtual void HugWalls()
        {
            if(type == MovementType.HugWalls)
            {
                turnAroundOnCollision = false;
                float newZValue = transform.localEulerAngles.z;
                rb.gravityScale = 0;
                if(rayHitNumber == 1 && !wait)
                {
                    wait = true;
                    currentWaitTime = originalWaitTime;
                    rb.velocity = Vector2.zero;
                    if(!enemyCharacter.facingLeft)
                    {
                        transform.localEulerAngles = new Vector3(0, 0, newZValue - 90);
                    }
                    else
                    {
                        transform.localEulerAngles = new Vector3(0, 0, newZValue + 90);
                    }
                }
                if(turn && !wait)
                {
                    wait = true;
                    currentWaitTime = originalWaitTime;
                    rb.velocity = Vector2.zero;
                    if(!enemyCharacter.facingLeft)
                    {
                        transform.eulerAngles = new Vector3(0, 0, newZValue + 90);
                        if(Mathf.Round(transform.eulerAngles.z) == 0)
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
                        if(Mathf.Round(transform.eulerAngles.z) == 270)
                        {
                            transform.position = new Vector2(transform.position.x - (transform.localScale.x * .5f), transform.position.y);
                        }
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, 0, newZValue - 90);
                        if(Mathf.Round(transform.eulerAngles.z) == 0)
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
                        if(Mathf.Round(transform.eulerAngles.z) == 270)
                        {
                            transform.position = new Vector2(transform.position.x + (transform.localScale.x * .5f), transform.position.y);
                        }
                    }
                }
                if(Mathf.Round(transform.eulerAngles.z) == 0)
                {
                    rb.velocity = new Vector2(currentSpeed, 0);
                }
                if(Mathf.Round(transform.eulerAngles.z) == 90)
                {
                    rb.velocity = new Vector2(0, currentSpeed);
                }
                if(Mathf.Round(transform.eulerAngles.z) == 180)
                {
                    rb.velocity = new Vector2(-currentSpeed, 0);
                }
                if(Mathf.Round(transform.eulerAngles.z) == 270)
                {
                    rb.velocity = new Vector2(0, -currentSpeed);
                }
                if(rayHitNumber == 0 && !wait)
                {
                    transform.localEulerAngles = Vector3.zero;
                    rb.gravityScale = 1;
                }
            }
        }

        protected virtual void HandleWait()
        {
            currentWaitTime -= Time.deltaTime;
            if(currentWaitTime <= 0)
            {
                wait = false;
                currentWaitTime = 0;
            }
        }
    }
}