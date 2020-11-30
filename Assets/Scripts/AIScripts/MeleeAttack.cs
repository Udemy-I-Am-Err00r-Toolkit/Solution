using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class MeleeAttack : AIManagers
    {
        [SerializeField]
        protected bool hitPlayerWhenClose;
        [SerializeField]
        protected int damageAmount;

        protected Collider2D swipeCollider;
        protected Animator anim;
        protected GameObject swipe;
        protected PlayerHealth playerHealth;
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

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == player && !hit)
            {
                hit = true;
                DealDamage();
            }
        }

        protected virtual void HitPlayer()
        {
            if(hitPlayerWhenClose && !enemyCharacter.playerIsClose)
            {
                return;
            }
            timeTillDoAction -= Time.deltaTime;
            if(timeTillDoAction <= 0)
            {
                swipe.SetActive(true);
                anim.SetBool("Attack", true);
                timeTillDoAction = originalTimeTillDoAction;
                if(hit)
                {
                    hit = false;
                }
            }
            Invoke("CancelSwipe", anim.GetCurrentAnimatorStateInfo(0).length);
        }

        protected virtual void DealDamage()
        {
            if(hit)
            {
                playerHealth.DealDamage(damageAmount);
            }
        }


        protected virtual void CancelSwipe()
        {
            anim.SetBool("Attack", false);
            swipe.SetActive(false);
        }
    }
}