using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script will handle a Melee attack you might want to have on the Player
    public class PlayerMeleeAttack : Abilities
    {
        //How much damage needs to be applied when the attack lands
        [SerializeField]
        protected int damageAmount;
        //What layers the collision needs to react to
        [SerializeField]
        protected LayerMask damageLayers;

        //A value the timeTillDamageNext needs to reset back to after input is pressed
        protected float originalTimeTillDamageNext;

        protected override void Initialization()
        {
            base.Initialization();
        }


        private void Update()
        {
            //Checks if the input for a melee is pressed and the time allowed between attacks is good
            if(input.MeleeAttackPressed() && originalTimeTillDamageNext <= 0)
            {
                //Runs MeleeAttack method
                MeleeAttack();
            }
            //If the attack button was just pressed, then don't allow an attack
            if (originalTimeTillDamageNext > 0)
            {
                //Runs the ManageAttackTime method
                ManageAttackTime();
            }
        }


        //Hnadles melee attacks based on character state and what animation should be playing for those states
        protected virtual void MeleeAttack()
        {
            //A local variable that grabs the current animator controller attached to the player animator component
            RuntimeAnimatorController ac = anim.runtimeAnimatorController;
            //Checks to see if the player is currently in a sprinting state
            if(character.isSprinting)
            {
                //Sets the sprinting melee attack on the Character script to true
                character.sprintingMeleeAttack = true;
                //Sets the SprintingMeleeAttack bool parameter on the Animator to true
                anim.SetBool("SprintingMeleeAttack", true);
                //Runs a for loop to find all the animation clips in the Animator component
                for(int i = 0; i < ac.animationClips.Length; i++)
                {
                    //Finds the Animation that handles the SprintingMeleeAttack
                    if(ac.animationClips[i].name == "SprintingMeleeAttack")
                    {
                        //Sets the local variable to the length of the Sprinting Animation
                        originalTimeTillDamageNext = ac.animationClips[i].length;
                        //Runs the CancelAttack method to turn off all the melee attacking bools on character and Animator
                        Invoke("CancelAttack", originalTimeTillDamageNext);
                    }
                }
            }
            else
            {
                //Basically does the exact same thing as the above if statement, but this melee attacks while walking or standing still
                character.meleeAttacking = true;
                anim.SetBool("MeleeAttack", true);
                for (int i = 0; i < ac.animationClips.Length; i++)
                {
                    if (ac.animationClips[i].name == "Melee Attack")
                    {
                        originalTimeTillDamageNext = ac.animationClips[i].length;
                        Invoke("CancelAttack", originalTimeTillDamageNext);
                    }
                }
            }
        }

        //Handles the time till the Player can MeleeAttack again
        protected virtual void ManageAttackTime()
        {
            //Counts down the originalTimeTillDamageNext variable by time since last frame
            originalTimeTillDamageNext -= Time.deltaTime;
            //If the variable is less than 0
            if (originalTimeTillDamageNext < 0)
            {
                //Sets the variable to 0
                originalTimeTillDamageNext = 0;
            }
        }

        //If a damage layer is inside the trigger collider of the swipe, then it sets the hit bool to true, and runs the DealDamage method
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if ((1 << collision.gameObject.layer & damageLayers) != 0 && collision.GetComponent<Health>())
            {
                DealDamage(collision.gameObject);
            }
        }


        //Runs the DealDamage method found on the Health script if hit is true
        protected virtual void DealDamage(GameObject hitTarget)
        {
            //Checks to see if the object is to the right to handle knockback force
            if (hitTarget.transform.position.x > transform.position.x)
            {
                //Sets the left bool on the Health script to false
                hitTarget.GetComponent<Health>().left = false;
            }
            //Same as if statement above but if the object is to the left to handle knockback force
            else
            {
                //Sets the left bool on the Health script to true
                hitTarget.GetComponent<Health>().left = true;
            }
            //Runs the DealDamage method on the health script to negate health points
            hitTarget.GetComponent<Health>().DealDamage(damageAmount);
        }

        //This method sets all the melee attack bools to false all at once
        protected virtual void CancelAttack()
        {
            character.meleeAttacking = false;
            character.sprintingMeleeAttack = false;
            anim.SetBool("MeleeAttack", false);
            anim.SetBool("SprintingMeleeAttack", false);
        }
    }
}