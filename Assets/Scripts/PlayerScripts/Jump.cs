using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script handles all the different types of jumps as well as checking for ground.
namespace MetroidvaniaTools
{
    public class Jump : Abilities
    {
        //Will not allow the player to double jump if the player is already in a falling state.
        [SerializeField]
        protected bool limitAirJumps;
        //The total number of jumps allowed from a grounded state.
        [SerializeField]
        protected int maxJumps;
        //The initial force of the jump when the jump button is pressed and a jump is allowed.
        [SerializeField]
        protected float jumpForce;
        //How much additional force to the jump is applied if the jump button is held down.
        [SerializeField]
        protected float holdForce;
        //How long you will need to hold down the jump button before reaching the maximum jump height.
        [SerializeField]
        protected float buttonHoldTime;
        //How close the player needs to be to the ground for the grounded state.
        [SerializeField]
        protected float distanceToCollider;
        //How far horizontally the player moves when they wall jump
        [SerializeField]
        protected float horizontalWallJumpForce;
        //How hight the player moves when they wall jump.
        [SerializeField]
        protected float verticalWallJumpForce;
        //How fast the player rises vertically from a jump.
        [SerializeField]
        protected float maxJumpSpeed;
        //How fast the player falls vertically when in the falling state.
        [SerializeField]
        protected float maxFallSpeed;
        //How fast the player needs to be falling before entering the fall state.
        [SerializeField]
        protected float acceptedFallSpeed;
        //How long the player is allowed to glide.
        [SerializeField]
        protected float glideTime;
        //How gravity is affected during the glide state (and others, but mainly glide)
        [SerializeField]
        [Range(-2, 2)]
        protected float gravity;
        //How long the player will not be allowed to be controlled after wall jumping.
        [SerializeField]
        protected float wallJumpTime;
        //What layers the player needs to look for for a grounded state.
        public LayerMask collisionLayer;

        //Bool that handles entering the wall jump state.
        private bool isWallJumping;
        //Bool that lets other scripts know a wall jump has occured.
        private bool justWallJumped;
        //Manages the direction the player should face when wall sliding.
        private bool flipped;
        //The current amount of time you have held the jump button for buttonHoldTime.
        private float jumpCountDown;
        //The current amount of time the player has been gliding.
        private float fallCountDown;
        //The current of amount of time since the player wall jumped, works with wallJumpTime.
        private float wallJumpCountdown;
        //The number of additonal jumps left after each jump is performed.
        private int numberOfJumpsLeft;


        protected override void Initialization()
        {
            base.Initialization();
            numberOfJumpsLeft = maxJumps;
            jumpCountDown = buttonHoldTime;
            fallCountDown = glideTime;
            wallJumpCountdown = wallJumpTime;
        }

        protected virtual void Update()
        {
            CheckForJump();
        }

        //Checks all the different conditions for each different type of jump.
        protected virtual bool CheckForJump()
        {
            if (gameManager.gamePaused)
            {
                return false;
            }
            if (input.JumpPressed())
            {
                //Checks to see if the player should jump down through a one-way platform.
                if (currentPlatform != null && currentPlatform.GetComponent<OneWayPlatform>() && input.DownHeld())
                {
                    character.isJumpingThroughPlatform = true;
                    Invoke("NotJumpingThroughPlatform", .1f);
                    return false;
                }
                //Checks to see if the player fell off a ledge first and then tried to jump.
                if (!character.isGrounded && numberOfJumpsLeft == maxJumps)
                {
                    character.isJumping = false;
                    return false;
                }
                //Checks to see if you don't want to jump while falling and prohibits jump if you don't want to jump while falling.
                if (limitAirJumps && character.Falling(acceptedFallSpeed))
                {
                    character.isJumping = false;
                    return false;
                }
                //Checks to see if you are wall sliding so the player can perform a wall jump.
                if (character.isWallSliding)
                {
                    wallJumpTime = wallJumpCountdown;
                    isWallJumping = true;
                    return false;
                }
                //If none of the other if statements are true and the logic flows here, then performs a standard jump and negates a jump from
                // numberOfJumpsLeft.
                numberOfJumpsLeft--;
                if (numberOfJumpsLeft >= 0)
                {
                    if (numberOfJumpsLeft == 0)
                    {
                        anim.SetBool("SecondJumpAnimationBool", true);
                    }
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    jumpCountDown = buttonHoldTime;
                    character.isJumping = true;
                    fallCountDown = glideTime;
                }
                return true;
            }
            else
                return false;
        }


        protected virtual void FixedUpdate()
        {
            IsJumping();
            Gliding();
            GroundCheck();
            WallSliding();
            WallJump();
        }

        //Handles the initial force of a jump and also checks to see how much additional force should be applied while holding down the button.
        protected virtual void IsJumping()
        {
            //If the bool to allow the jump is true, apply original jump force, then apply any additional force from holding the button
            if (character.isJumping)
            {
                rb.AddForce(Vector2.up * jumpForce);
                AdditionalAir();
            }
            //If the jump force is causing the player to move vertically too quickly, limits the vertical speed to maxJumpSpeed value
            if (rb.velocity.y > maxJumpSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxJumpSpeed);
            }
        }

        //Checks for input and manages the fall speed while gliding.
        protected virtual void Gliding()
        {
            //If the Player's vertical speed is less than zero and the button that controls the glide is held, then allows the Player to enter the gliding state
            if (character.Falling(0) && input.JumpHeld())
            {
                //Negates the time the player is allowed to glide
                fallCountDown -= Time.deltaTime;
                //As long as the fallCountDown value is greater than zero and the player isn't already in a free fall state, then allows the player to enter gliding state
                if (fallCountDown > 0 && rb.velocity.y > acceptedFallSpeed)
                {
                    anim.SetBool("Gliding", true);
                    FallSpeed(gravity);
                    return;
                }
            }
            anim.SetBool("Gliding", false);
        }

        //Handles the additional amount of jump force when holding down jump button.
        protected virtual void AdditionalAir()
        {
            if (input.JumpHeld())
            {
                jumpCountDown -= Time.deltaTime;
                if (jumpCountDown <= 0)
                {
                    jumpCountDown = 0;
                    character.isJumping = false;
                }
                else
                    rb.AddForce(Vector2.up * holdForce);
            }
            else
            {
                character.isJumping = false;
            }
        }

        //Checks to see if character is grounded and manages the grounded state.
        protected virtual void GroundCheck()
        {
            //Runs method to see what the Player is colliding with beneath them
            if (CollisionCheck(Vector2.down, distanceToCollider, collisionLayer) && !character.isJumping)
            {
                //If the player is on a moving platform, sets the Player as a child of the moving platform so the Player moves with the platform
                if (currentPlatform.GetComponent<MoveablePlatform>())
                {
                    transform.parent = currentPlatform.transform;
                }
                //Handles all the logic that should be toggled when in a grounded state
                anim.SetBool("Grounded", true);
                character.isGrounded = true;
                numberOfJumpsLeft = maxJumps;
                fallCountDown = glideTime;
                justWallJumped = false;
            }
            //If there isn't a platform gameobject right beneath player, then toggles everythign for a not grounded state
            else
            {
                //ensures if the Player was on a moving platform previously, that the Player is no longer a child game object of anything
                transform.parent = null;
                anim.SetBool("Grounded", false);
                character.isGrounded = false;
                if (character.Falling(0) && rb.velocity.y < maxFallSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
                }
            }
            //This value is fed to the VerticalSpeed animation, and depending on whether the player is rising or falling, this will feed the y value of velocity to the animator for that blend tree
            anim.SetFloat("VerticalSpeed", rb.velocity.y);
        }

        //Checks to see if the player is wall sliding and manages the wall sliding state.
        protected virtual bool WallSliding()
        {
            //Does a collision check to see if the player is touching a wall
            if (WallCheck())
            {
                //If the player is touching a wall and doesn't have the Player's back to the wall, flips the Player so their back is against the wall
                if (!flipped)
                {
                    Flip();
                    flipped = true;
                }
                //Slows the Player's fall speed to what we set as the gravity speed in the inspector
                FallSpeed(gravity);
                character.isWallSliding = true;
                anim.SetBool("WallSliding", true);
                return true;
            }
            //If there is no collision with a wall, makes sure the Player is no longer in a wall sliding state
            else
            {
                character.isWallSliding = false;
                anim.SetBool("WallSliding", false);
                if (flipped && !isWallJumping)
                {
                    Flip();
                    flipped = false;
                }
                return false;
            }
        }

        //Checks to see if the player is running into a wall when jumping to enter the wall sliding state.
        protected virtual bool WallCheck()
        {
            if ((!character.isFacingLeft && CollisionCheck(Vector2.right, distanceToCollider, collisionLayer) || character.isFacingLeft && CollisionCheck(Vector2.left, distanceToCollider, collisionLayer)) && movement.MovementPressed() && !character.isGrounded)
            {
                if (currentPlatform.GetComponent<OneWayPlatform>() || currentPlatform.GetComponent<Ladder>())
                {
                    return false;
                }
                //Newer logic I added after fliming course, this will allow you to immedietly wall jump again if the player is colliding with a wall; will update video shortly
                if (justWallJumped)
                {
                    wallJumpTime = 0;
                    justWallJumped = false;
                    isWallJumping = false;
                    movement.enabled = true;
                }
                return true;
            }
            return false;
        }

        //Handles the direction and force of the wall jump.
        protected virtual void WallJump()
        {
            if (isWallJumping)
            {
                //Pushes the player vertically and horizontally away from wall depending on direction the Player should move in
                rb.AddForce(Vector2.up * verticalWallJumpForce);
                if (!character.isFacingLeft)
                {
                    rb.AddForce(Vector2.left * horizontalWallJumpForce);
                }
                if (character.isFacingLeft)
                {
                    rb.AddForce(Vector2.right * horizontalWallJumpForce);
                }
                movement.enabled = false;
                //A very, very brief delay that needs to occur so the Player can move away from wall
                Invoke("JustWallJumped", .05f);
            }
            //After wall jumping, ensures that movement is restricted to allow the boost to perform when jumping away from wall
            if (wallJumpTime > 0)
            {
                wallJumpTime -= Time.deltaTime;
                //Once wallJumpTime goes back to zero after being negated, it sets everything back to the normal state
                if (wallJumpTime <= 0)
                {
                    movement.enabled = true;
                    isWallJumping = false;
                    wallJumpTime = 0;
                }
            }
        }

        //Lets other scripts know the player just wall jumped.
        protected virtual void JustWallJumped()
        {
            justWallJumped = true;
        }

        //Lets other scripts know the player isn't jumping through a platform.
        protected virtual void NotJumpingThroughPlatform()
        {
            character.isJumpingThroughPlatform = false;
        }

        //Turns off horizontal movement when wall jumping to restrict movement when performing a wall jump.
        protected virtual IEnumerator WallJumped()
        {
            movement.enabled = false;
            yield return new WaitForSeconds(wallJumpTime);
            movement.enabled = true;
            isWallJumping = false;
        }
    }
}