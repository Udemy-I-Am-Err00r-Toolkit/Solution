using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //Base script that houses similar data for Enemies and Player
    public class Health : Managers
    {
        //This is the total amount of health an Enemy or Player can have
        public int maxHealthPoints = 100;
        //How much time after the object is hit that they can no longer receive damage; usually a brief amount of time like half a second
        [SerializeField]
        protected float iFrameTime = .5f;
        //How much vertical knockback needs to be applied to the Player when they are dealt damage
        [SerializeField]
        protected float verticalDamageForce = 50;
        //How much horizontal knockback needs to be applied to the Player when they are dealt damage
        [SerializeField]
        protected float horizontalDamageForce = 50;
        //This is the current amount of health an Enemy or Player has
        [HideInInspector]
        public int healthPoints;
        //A quick bool that manages whether or not the object is facing left when taking damage to apply horizontal knockback force
        [HideInInspector]
        public bool left;
        //A bool that manages the momement the object is hit
        [HideInInspector]
        public bool hit;
        //A reference to the Rigidbody on the object
        protected Rigidbody2D rb;

        protected override void Initialization()
        {
            base.Initialization();
            //Get the Rigidbody2D on the main object
            rb = GetComponent<Rigidbody2D>();
            //This is more specific to enemies so that when a scene loads, all the enemies have max health again
            healthPoints = maxHealthPoints;
        }

        protected virtual void FixedUpdate()
        {
            //Method that will handle knockback force after receiving damage
            HandleDamageMovement();
        }

        //This method negates the health points based on the damage value found on the Projectile or Melee script that caused damage
        public virtual void DealDamage(int amount)
        {
            //If not hit
            if (!hit)
            {
                //Negates healthpoints
                healthPoints -= amount;
                //Sets the hit bool to true
                hit = true;
                //Sets the invulnerable bool to true
                Invoke("Cancel", iFrameTime);
            }
        }

        //Manages all the damage effects that should happen when damage is dealt
        public virtual void HandleDamageMovement()
        {
            //If the hit bool is true
            if (hit)
            {
                //Cancels movement to absorb the hit
                rb.velocity = Vector2.zero;
                //Handles vertical and horizontal knockback depending on what direction the Player is facing
                rb.AddForce(Vector2.up * verticalDamageForce);
                if (!left)
                {
                    rb.AddForce(Vector2.right * horizontalDamageForce);
                }
                else
                {
                    rb.AddForce(Vector2.left * horizontalDamageForce);
                }
            }
        }

        //Method that cancels the hit and invulnerable bools so the object can be hit again
        protected virtual void Cancel()
        {
            hit = false;
        }
    }
}