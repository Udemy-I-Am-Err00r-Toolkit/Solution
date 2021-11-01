using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

namespace MetroidvaniaTools
{
    //This script will handle a Melee attack you might want to have on your Enemy
    public class PlayerMeleeAttack : Abilities
    {
        //How much damage needs to be applied to the Player when the Player is hit by the attack
        [SerializeField]
        protected int damageAmount;
        [SerializeField]
        protected float timeTillDamageNext;
        [SerializeField]
        protected LayerMask damageLayers;

        //A collider that gets adjusted through an animation to determine if the Player is inside that collider
        protected Collider2D meleeCollider;
        //The animation that needs to play when attacking
        protected Animator meleeAnim;
        //A quick bool that turns true if the melee attack struck the Player
        protected bool hit;
        protected float originalTimeTillDamageNext;
        protected PlayerMeleeWeapon playerMeleeWeapon;

        protected override void Initialization()
        {
            base.Initialization();
        }


        private void Update()
        {
            if(input.MeleeAttackPressed() && originalTimeTillDamageNext <= 0)
            {
                MeleeAttack();
            }
            if (originalTimeTillDamageNext > 0)
            {
                ManageAttackTime();
            }
        }

        protected virtual void FixedUpdate()
        {
            Hit();
        }

        protected virtual void MeleeAttack()
        {
            float meleeAttackTime;
            RuntimeAnimatorController ac = anim.runtimeAnimatorController;
            if(character.isSprinting)
            {
                character.sprintingMeleeAttack = true;
                anim.SetBool("SprintingMeleeAttack", true);
                for(int i = 0; i < ac.animationClips.Length; i++)
                {
                    if(ac.animationClips[i].name == "SprintingMeleeAttack")
                    {
                        meleeAttackTime = ac.animationClips[i].length;
                        originalTimeTillDamageNext = meleeAttackTime;
                        Invoke("CancelAttack", meleeAttackTime);
                    }
                }
            }
            else
            {
                character.meleeAttacking = true;
                anim.SetBool("MeleeAttack", true);
                for (int i = 0; i < ac.animationClips.Length; i++)
                {
                    if (ac.animationClips[i].name == "Melee Attack")
                    {
                        meleeAttackTime = ac.animationClips[i].length;
                        originalTimeTillDamageNext = meleeAttackTime;
                        Invoke("CancelAttack", meleeAttackTime);
                    }
                }
            }
        }

        protected virtual void ManageAttackTime()
        {
            originalTimeTillDamageNext -= Time.deltaTime;
            if (originalTimeTillDamageNext < 0)
            {
                originalTimeTillDamageNext = 0;
            }
        }

        //If the Player is inside the trigger collider of the swipe, then it sets the hit bool to true, and runs the DealDamage method
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if ((1 << collision.gameObject.layer & damageLayers) != 0)
            {
                Debug.Log("I hit something I should");
                hit = true;
                DealDamage(collision.gameObject);
            }
        }

        //This method manages if and when the Enemy should attack the Player; most of this logic is managed by whether or not the Enemy should attack only when close, and how much time is left in the timeTillDoAction variable
        protected virtual void Hit()
        {
            timeTillDamageNext -= Time.deltaTime;
            if (timeTillDamageNext <= 0)
            {
                timeTillDamageNext = originalTimeTillDamageNext;
                if (hit)
                {
                    hit = false;
                }
            }
        }

        //Runs the DealDamage method found on the PlayerHealth script if hit is true
        protected virtual void DealDamage(GameObject hitTarget)
        {
            if (hit && hitTarget.GetComponent<Health>())
            {
                if (hitTarget.transform.position.x < transform.position.x)
                {
                    hitTarget.GetComponent<Health>().left = false;
                }
                else
                    hitTarget.GetComponent<Health>().left = true;
                hitTarget.GetComponent<Health>().DealDamage(damageAmount);
            }
        }

        protected virtual void CancelAttack()
        {
            character.meleeAttacking = false;
            character.sprintingMeleeAttack = false;
            anim.SetBool("MeleeAttack", false);
            anim.SetBool("SprintingMeleeAttack", false);
        }
    }
}