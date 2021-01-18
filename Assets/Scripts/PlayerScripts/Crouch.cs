using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //Depending on what collider type you have on your player, this should add that type of collider to ensure specific logic for that collider type can run; in the course I use an CapsuleCollider2D, but if you are using any other 2D collider type, type that collider type rather than CapsuleCollider2D
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class Crouch : Abilities
    {
        //This is the amount the collider will shrink when crouching
        [SerializeField]
        [Range(0, 1)]
        protected float colliderMultiplier;
        //Checks to see if the player is not underneath a collider that would prevent them from standing such as a platform
        [SerializeField]
        protected LayerMask layers;
        //A quck reference to the exact collider type the Player is using
        private CapsuleCollider2D playerCollider;
        //The original size of the collider before entering crouching state
        private Vector2 originalCollider;
        //The size of the collider after entering crouching state
        private Vector2 crouchingColliderSize;
        //The offset of the collider when not in crouching state; depending on how you build your character, you might not need this value, but how I build the character and discuss in course, we do use this value
        private Vector2 originalOffset;
        //The offset of the collider when in crouching state; just like above, depending on how you build your character you might not need this value
        private Vector2 crouchingOffset;

        protected override void Initialization()
        {
            base.Initialization();
            playerCollider = GetComponent<CapsuleCollider2D>();
            //Sets the size of the originalCollider
            originalCollider = playerCollider.size;
            //Sets the size of the collider when in crouching state
            crouchingColliderSize = new Vector2(playerCollider.size.x, (playerCollider.size.y * colliderMultiplier));
            //Sets the offset of the collider when not crouching
            originalOffset = playerCollider.offset;
            //Sets the offset of the collider when crouching
            crouchingOffset = new Vector2(playerCollider.offset.x, (playerCollider.offset.y * colliderMultiplier));
        }

        protected virtual void FixedUpdate()
        {
            Crouching();
        }


        protected virtual void Crouching()
        {
            //Checks input to see if crouch button is pressed and toggles the crouching state and changes collider size as well as offset to collider size and offset
            if (input.CrouchHeld() && character.isGrounded)
            {
                character.isCrouching = true;
                anim.SetBool("Crouching", true);
                playerCollider.size = crouchingColliderSize;
                playerCollider.offset = crouchingOffset;
            }
            else
            {
                //If the input is not held for crouching, handles logic
                if (character.isCrouching)
                {
                    //If there is a platform or something above the Player that should prevent the player from standing, then we do nothing here and return out
                    if (CollisionCheck(Vector2.up, playerCollider.size.y * .25f, layers))
                    {
                        return;
                    }
                    //If there is noting above the Player when no crouch input is detected, then allows the Player to stand
                    StartCoroutine(CrouchDisabled());
                }
            }
        }

        //Toggles everything that should happen when the Player is no longer crouching and reset collider size and offset back to original values
        protected virtual IEnumerator CrouchDisabled()
        {
            playerCollider.offset = originalOffset;
            yield return new WaitForSeconds(.01f);
            playerCollider.size = originalCollider;
            yield return new WaitForSeconds(.15f);
            character.isCrouching = false;
            anim.SetBool("Crouching", false);
        }
    }
}