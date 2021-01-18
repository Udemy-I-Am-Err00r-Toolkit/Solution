using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script is specific to Enemy ranged weapons, and houses the data needed to manage a ranged attack from an enemy
    public class EnemyWeapon : AIManagers
    {
        //Bool that checks to see if an enemy can fire a weapon automatically
        [SerializeField]
        protected bool automatic;
        //Bool that only allows the enemy to shoot at the Player when they are close
        [SerializeField]
        protected bool onlyFireWhenPlayerClose;
        //Bool that will determine whether or not the enemy should fire directly in front of it or aim at the Player when firing
        [SerializeField]
        protected bool aimAtPlayer;
        //The ranged weapon the Enemy has; right now this solution only supports the enemy to have one ranged weapon
        [SerializeField]
        protected WeaponTypes weapon;
        //Determines where on the Enemy a projectile should spawn from when being fired
        [SerializeField]
        protected Transform projectileSpawnPosition;
        //Determines the rotation of the projectile based on where the Player is to the enemy and if the enemy can aim at the Player
        [SerializeField]
        protected Transform projectileSpawnRotation;
        //A small amount of shots that are fired in a burst for a more specific attack type
        [SerializeField]
        protected int automaticBurstAmount;

        //The pool in which the projectiles should pull from
        [HideInInspector]
        public List<GameObject> currentPool;
        [HideInInspector]
        public List<GameObject> totalPools;
        //The current projectile that was most recently fired by the Enemy
        [HideInInspector]
        public GameObject currentProjectile;
        //A quick reference to the Object Pooling system instantiated by the Singleton Pattern
        protected ObjectPooler objectPooler;
        //The game object that has all the projectile for the Enemy in the inspector
        protected GameObject projectileParentFolder;

        //Quick bool that makes sure the Object Pooler doesn't generate too many items
        protected bool poolSpawned;
        //Quick bool that manages when the Enemy is shooting automatically for burst fire
        protected bool autoFire;
        //The amount of time in between shots for automatic firing for the Enemy
        protected float autoTime;
        //How many shots have been fired by the Enemy, this is for busrt fire
        protected int shotsFired;

        protected override void Initialization()
        {
            base.Initialization();
            Invoke("Pool", .05f);
        }

        //Sets up the Pool of projectiles for the Enemy as well as setup basic parameters for the Enemy to manage how a weapon can and should be used
        protected virtual void Pool()
        {
            projectileParentFolder = new GameObject();
            objectPooler = FindObjectOfType<ObjectPooler>();
            objectPooler.CreateEnemyPool(weapon, currentPool, projectileParentFolder, this);
            timeTillDoAction = originalTimeTillDoAction;
            if (automatic)
            {
                autoTime = weapon.timeBetweenShots;
            }
            poolSpawned = true;
        }

        protected virtual void FixedUpdate()
        {
            HandleFiring();
        }

        //This takes data set in the Pool method and manages the actions of when the Enemy can fire weapon
        protected virtual void HandleFiring()
        {
            //Checks to see if the Enemy pool has been loaded into the scene first
            if (poolSpawned)
            {
                timeTillDoAction -= Time.deltaTime;
                if (timeTillDoAction <= 0)
                {
                    //If the weapon is automatic, it sets the autoFire bool to true and then runs the logic to fire the weapon automatically
                    if (automatic)
                    {
                        autoFire = true;
                        FireAutomaticWeapon();
                    }
                    //If the weapon is not automatic, fires the weapon one round at a time
                    else
                    {
                        FireWeapon();
                    }
                }
            }
            //Checks if additional logic needs to be applied to aim at the player
            if (aimAtPlayer)
            {
                Aim();
            }
        }

        //Method for non automatic weapons being fired
        protected virtual void FireWeapon()
        {
            //Checks to see if there is any conditions to see if Player needs to be close to the Enemy before firing or if the Player doesn't need to be close to the Enemy
            if (onlyFireWhenPlayerClose && enemyCharacter.playerIsClose || !onlyFireWhenPlayerClose)
            {
                //Creates a projectile based on the next available projectile in the Pool
                currentProjectile = objectPooler.GetEnemyObject(currentPool, weapon, projectileParentFolder, weapon.projectile.tag);
                if (currentProjectile != null)
                {
                    //Places the projectile in the correct position and resets timeTillDoAction back to it's original value
                    Invoke("PlaceProjectile", .05f);
                    timeTillDoAction = originalTimeTillDoAction;
                }
            }
        }

        //Method for automatic weapons being fired
        protected virtual void FireAutomaticWeapon()
        {
            if (autoFire && onlyFireWhenPlayerClose && enemyCharacter.playerIsClose || autoFire && !onlyFireWhenPlayerClose)
            {
                //Determines if the amount of time between shots
                autoTime -= Time.deltaTime;
                if (autoTime <= 0)
                {
                    //Creates projectile in correct position
                    currentProjectile = objectPooler.GetEnemyObject(currentPool, weapon, projectileParentFolder, weapon.projectile.tag);
                    if (currentProjectile != null)
                    {
                        Invoke("PlaceProjectile", .05f);
                    }
                    //Resets the time between shots for auto fire
                    autoTime = weapon.timeBetweenShots;
                    //Adds one to the shotsFired value for burst firing; if not burst firing it will just continue shooting projectiles in the amount of autoTime
                    shotsFired++;
                    if (shotsFired == automaticBurstAmount)
                    {
                        timeTillDoAction = originalTimeTillDoAction;
                        shotsFired = 0;
                        autoFire = false;
                    }
                }
            }
        }

        //Complicated math logic that ensures the projectile is rotated and moving in a direction to hit the Player
        protected virtual void Aim()
        {
            if (player != null && aimAtPlayer)
            {
                Vector3 target = player.transform.position;
                target.z = 0;

                Vector3 currentPosition = projectileSpawnPosition.position;
                if (!enemyCharacter.facingLeft)
                {
                    target.x = target.x - currentPosition.x;
                    target.y = playerCollider.bounds.center.y - currentPosition.y;
                }
                else
                {
                    target.x = currentPosition.x - target.x;
                    target.y = currentPosition.y - playerCollider.bounds.center.y;
                }
                float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
                projectileSpawnRotation.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
        }

        //The method that places the projectile in the correct position based on whether or not the Enemy is facing left or right
        protected virtual void PlaceProjectile()
        {
            currentProjectile.transform.position = projectileSpawnPosition.position;
            currentProjectile.transform.rotation = projectileSpawnRotation.rotation;
            currentProjectile.SetActive(true);
            if (!enemyCharacter.facingLeft)
            {
                currentProjectile.GetComponent<Projectile>().left = false;
            }
            else
            {
                currentProjectile.GetComponent<Projectile>().left = true;
            }
            currentProjectile.GetComponent<Projectile>().fired = true;
        }
    }
}
