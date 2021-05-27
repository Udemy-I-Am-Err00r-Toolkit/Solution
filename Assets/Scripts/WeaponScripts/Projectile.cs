using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class Projectile : MonoBehaviour
    {
        //The scriptable object this projectile belongs to
        [SerializeField]
        protected WeaponTypes weapon;
        //The amount of damage this projectile should inflict
        [SerializeField]
        protected int damageAmount;
        //The different layers the projectile should inflice damage to
        [SerializeField]
        protected LayerMask damageLayers;
        //A quick bool that gets toggled by the weapon script to allow the projectile to run the Movement method in this script
        [HideInInspector]
        public bool fired;
        //A quick bool that determines if the projectile should be moving in a leftward or rightward direction
        [HideInInspector]
        public bool left;
        //How much time this projectile should be active after fired; based on value from scriptable object
        [HideInInspector]
        public float projectileLifeTime;
        //Makes sure the projectile game object is facing the correct direction
        private bool flipped;

        //Once the projectile is active in the scene, we set it's lifeTime value to the scriptable object lifetime value; we then negate this value after it is fired so it can be removed from scene if it doesn't collide with something
        protected virtual void OnEnable()
        {
            projectileLifeTime = weapon.lifeTime;
        }

        protected virtual void FixedUpdate()
        {
            Movement();
        }

        public virtual void Movement()
        {
            //Weapon script toggles this bool when the projectile should be fired
            if (fired)
            {
                //Negates projectileLifeTime value after fired
                projectileLifeTime -= Time.deltaTime;
                //Logic that is performed if projectileLifeTime is greater than zero
                if (projectileLifeTime > 0)
                {
                    //Checks if projectile is grappling hook, handles additional logic that pertains only to that projectile type
                    if (gameObject.tag == "GrapplingHook")
                    {
                        //Makes sure the projectile is a child of the player so the hook trail can be drawn behind the projectile
                        transform.parent = GameObject.FindWithTag("Player").transform;
                        transform.Translate(Vector2.right * weapon.projectileSpeed * Time.deltaTime);
                        return;
                    }
                    //Logic that should run when player is facing right and projectile is fired
                    if (!left)
                    {
                        //If projectile gameobject localScale is in a leftward facing direction, flips it to a righward facing direction
                        if (flipped)
                        {
                            flipped = false;
                            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                        }
                        //Moves the projectile in the correct direction based on projectile speed
                        transform.Translate(Vector2.right * weapon.projectileSpeed * Time.deltaTime);
                    }
                    //Logic that should run when player is facing left and projectile is fired
                    else
                    {
                        //If projectile gameobject localScale is in a righward facing direction, flips it to a leftward facing direction
                        if (!flipped)
                        {
                            flipped = true;
                            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                        }
                        //Moves the projectile in the correct direction based on projectile speed
                        transform.Translate(Vector2.left * weapon.projectileSpeed * Time.deltaTime);
                    }
                }
                //if projectile lifeTime value is less than zero, run this method
                else
                {
                    DestroyProjectile();
                }
            }
        }

        //This method handles all the logic of when a projectile enters a layer that it should cause damage to
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if ((1 << collision.gameObject.layer & damageLayers) != 0)
            {
                if (collision.gameObject.GetComponent<Health>() != null)
                {
                    collision.gameObject.GetComponent<Health>().DealDamage(damageAmount);
                }
                if (collision.gameObject.tag == "Player")
                {
                    if (collision.transform.position.x < transform.position.x)
                    {
                        collision.gameObject.GetComponent<PlayerHealth>().left = false;
                    }
                    else
                        collision.gameObject.GetComponent<PlayerHealth>().left = true;
                }
                DestroyProjectile();
            }
        }

        //This method removes the projectile from the scene whenever it should
        public virtual void DestroyProjectile()
        {
            projectileLifeTime = weapon.lifeTime;
            gameObject.SetActive(false);
        }
    }
}