using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script will handle a Melee attack you might want to have on your Enemy
    public class MeleeAttack : AIManagers
    {
        //This bool will make sure the Enemy doesn't swing the Melee weapon if the player isn't close to the Enemy
        [SerializeField]
        protected bool hitPlayerWhenClose;
        //How much damage needs to be applied to the Player when the Player is hit by the attack
        [SerializeField]
        protected int damageAmount;

        //A collider that gets adjusted through an animation to determine if the Player is inside that collider
        protected Collider2D swipeCollider;
        //The animation that needs to play when attacking
        protected Animator anim;
        //The game object that is the physical attack; this game object appears as the slashing sprites from the animation
        protected GameObject swipe;
        //A quick reference to the PlayerHealth script to deal damage
        protected PlayerHealth playerHealth;
        //A quick bool that turns true if the melee attack struck the Player
        protected bool hit;

        protected override void Initialization()
        {
            base.Initialization();
            swipe = transform.GetChild(0).gameObject;
            anim = swipe.GetComponent<Animator>();
            swipeCollider = swipe.GetComponent<Collider2D>();
            playerHealth = player.GetComponent<PlayerHealth>();
            swipe.SetActive(false);
        }

        protected virtual void FixedUpdate()
        {
            HitPlayer();
        }

        //If the Player is inside the trigger collider of the swipe, then it sets the hit bool to true, and runs the DealDamage method
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == player && !hit)
            {
                hit = true;
                DealDamage();
            }
        }

        //This method manages if and when the Enemy should attack the Player; most of this logic is managed by whether or not the Enemy should attack only when close, and how much time is left in the timeTillDoAction variable
        protected virtual void HitPlayer()
        {
            if (hitPlayerWhenClose && !enemyCharacter.playerIsClose)
            {
                return;
            }
            timeTillDoAction -= Time.deltaTime;
            if (timeTillDoAction <= 0)
            {
                swipe.SetActive(true);
                anim.SetBool("Attack", true);
                timeTillDoAction = originalTimeTillDoAction;
                if (hit)
                {
                    hit = false;
                }
            }
            Invoke("CancelSwipe", anim.GetCurrentAnimatorStateInfo(0).length);
        }

        //Runs the DealDamage method found on the PlayerHealth script if hit is true
        protected virtual void DealDamage()
        {
            if (hit)
            {
                if (player.transform.position.x < transform.position.x)
                {
                    playerHealth.left = false;
                }
                else
                    playerHealth.left = true;
                playerHealth.DealDamage(damageAmount);
            }
        }

        //Manages the animation and disables the swipe game object from the scene until the Enemy melee attacks again
        protected virtual void CancelSwipe()
        {
            anim.SetBool("Attack", false);
            swipe.SetActive(false);
        }
    }
}