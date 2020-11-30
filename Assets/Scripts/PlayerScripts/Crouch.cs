using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    [RequireComponent (typeof (CapsuleCollider2D))]
    public class Crouch : Abilities
    {
        [SerializeField]
        [Range(0, 1)]
        protected float colliderMultiplier;
        [SerializeField]
        protected LayerMask layers;
        private CapsuleCollider2D playerCollider;
        private Vector2 originalCollider;
        private Vector2 crouchingColliderSize;
        private Vector2 originalOffset;
        private Vector2 crouchingOffset;

        protected override void Initialization()
        {
            base.Initialization();
            playerCollider = GetComponent<CapsuleCollider2D>();
            originalCollider = playerCollider.size;
            crouchingColliderSize = new Vector2(playerCollider.size.x, (playerCollider.size.y * colliderMultiplier));
            originalOffset = playerCollider.offset;
            crouchingOffset = new Vector2(playerCollider.offset.x, (playerCollider.offset.y * colliderMultiplier));
        }

        protected virtual void FixedUpdate()
        {
            Crouching();
        }


        protected virtual void Crouching()
        {
            if(input.CrouchHeld() && character.isGrounded)
            {
                character.isCrouching = true;
                anim.SetBool("Crouching", true);
                playerCollider.size = crouchingColliderSize;
                playerCollider.offset = crouchingOffset;
            }
            else
            {
                if (character.isCrouching)
                {
                    if(CollisionCheck(Vector2.up, playerCollider.size.y * .25f, layers))
                    {
                        return;
                    }
                    StartCoroutine(CrouchDisabled());
                }
            }
        }

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