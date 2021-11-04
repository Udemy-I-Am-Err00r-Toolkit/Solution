using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class Weapon : Abilities
    {
        [Header(" ")]
        [Header("This is the script that would house all the data for the weapon")]
        //This is the list of different weapon types the Player has access to
        [SerializeField]
        protected List<WeaponTypes> weaponTypes;
        //The position that the projectile should spawn from when being fired
        public Transform gunBarrel;
        //The bone that would handle propper rotation for the projectile when fired; this bone is usually the bone the gun arm exists on
        public Transform gunRotation;

        //The projectiles that need to be loaded for the pool that the weapon would pull from based on the WeaponType
        [HideInInspector]
        public List<GameObject> currentPool = new List<GameObject>();
        //If the WeaponType has the bulletsToReset bool set to true, this is the list of projectiles that it would reset from
        [HideInInspector]
        public List<GameObject> bulletsToReset = new List<GameObject>();
        //The total amount of pools that the different WeaponTypes can pull from; this is different than the currentPool, which is the current weapons pool.
        [HideInInspector]
        public List<GameObject> totalPools;

        //The most recent projectile fired
        [HideInInspector]
        public GameObject currentProjectile;
        //The weapon that is selected from the weaponTypes list that is currently equipped
        [HideInInspector]
        public WeaponTypes currentWeapon;
        //The amount of time the aiming IKs should remain active after firing a weapon
        [HideInInspector]
        public float currentTimeTillChangeArms;

        //A quick empty game object that acts as a parent folder for all the different projectile types
        private GameObject projectileParentFolder;
        //The amount of time between each projectile shot
        private float currentTimeBetweenShots;

        protected override void Initialization()
        {
            base.Initialization();
            //This sets up the current weapon at Start
            ChangeWeapon();
        }

        protected virtual void Update()
        {
            //Makes sure if the game is paused, that the weapons can't be fired so when game is unpaused, it doesn't fire a weapon
            if (gameManager.gamePaused || character.meleeAttacking)
            {
                return;
            }
            if (input.WeaponFired())
            {
                FireWeapon();
            }
            if (input.ChangeWeaponPressed())
            {
                ChangeWeapon();
            }
        }

        protected virtual void FixedUpdate()
        {
            PointGun();
            NegateTimeTillChangeArms();
            FireWeaponHeld();
        }

        //Method that manages the first shot being fired and toggles the character into a weapon fired state
        protected virtual void FireWeapon()
        {
            //Resets the currentTimeTillChangeArms value to the original value
            currentTimeTillChangeArms = currentWeapon.lifeTime;
            //Runs the ChangeArms method found in aimManager script
            aimManager.ChangeArms();
            //Sets up the currentProjectile that just got fired
            currentProjectile = objectPooler.GetObject(currentPool, currentWeapon, this, projectileParentFolder, currentWeapon.projectile.tag);
            if (currentProjectile != null)
            {
                Invoke("PlaceProjectile", .1f);
            }
            currentTimeBetweenShots = currentWeapon.timeBetweenShots;
        }

        protected virtual void FireWeaponHeld()
        {
            //Handles the logic that should run if the weapon is automatic and if the input is held down; much of this logic is to ensure that automatic weapons remain firing and have the propper aiming IKs setup
            if (!character.meleeAttacking && input.WeaponFiredHeld())
            {
                if (currentWeapon.automatic)
                {
                    currentTimeTillChangeArms = currentWeapon.lifeTime;
                    aimManager.ChangeArms();
                    currentTimeBetweenShots -= Time.deltaTime;
                    if (currentTimeBetweenShots < 0)
                    {
                        currentProjectile = objectPooler.GetObject(currentPool, currentWeapon, this, projectileParentFolder, currentWeapon.projectile.tag);
                        if (currentProjectile != null)
                        {
                            Invoke("PlaceProjectile", .1f);
                        }
                        currentTimeBetweenShots = currentWeapon.timeBetweenShots;
                    }
                }
            }
        }

        //Manages moving the gun arm around based on if the Player is aiming or not; if not aiming, points the gun arm right in front of the player depending on if they facing left or right as well as when wall sliding
        protected virtual void PointGun()
        {
            if (!aimManager.aiming)
            {
                if (!character.isFacingLeft)
                {
                    if (character.isWallSliding)
                    {
                        aimManager.whereToAim.position = new Vector2(aimManager.bounds.min.x, aimManager.bounds.center.y);
                    }
                    else
                    {
                        aimManager.whereToAim.position = new Vector2(aimManager.bounds.max.x, aimManager.bounds.center.y);
                    }

                }
                else
                {
                    if (character.isWallSliding)
                    {
                        aimManager.whereToAim.position = new Vector2(aimManager.bounds.max.x, aimManager.bounds.center.y);
                    }
                    else
                    {
                        aimManager.whereToAim.position = new Vector2(aimManager.bounds.min.x, aimManager.bounds.center.y);
                    }
                }
            }
            aimManager.aimingGun.transform.GetChild(0).position = aimManager.whereToAim.position;
            aimManager.aimingLeftHand.transform.GetChild(0).position = aimManager.whereToPlaceHand.position;
        }

        //Manages turning off the aiming IKs when the gun is not being fired
        protected virtual void NegateTimeTillChangeArms()
        {
            if (grapplingHook.connected)
            {
                return;
            }
            currentTimeTillChangeArms -= Time.deltaTime;
        }

        //This method handles setting up the initial weapon as well as setting up everything when the Player needs to change weapons based on input
        protected virtual void ChangeWeapon()
        {
            //This bool makes sure that the current weapon doesn't create a pool of objects if that pool of objects already exists
            bool matched = new bool();
            //Counts all the different weapon types in the list
            for (int i = 0; i < weaponTypes.Count; i++)
            {
                //If there isn't a current weapon, sets up the first weapon type in the list as the current weapon, and creates the pool for that weapon type
                if (currentWeapon == null)
                {
                    currentWeapon = weaponTypes[character.currentWeaponSelected];
                    currentTimeBetweenShots = currentWeapon.timeBetweenShots;
                    NewPool();
                    return;
                }
                //This else statement handles the logic for changing weapons after the initial weapon is loaded
                else
                {
                    //If the current iteration value is the current weapon
                    if (weaponTypes[i] == currentWeapon)
                    {
                        //Add one to the iteration value for the comment above
                        i++;
                        //If the iteration value is the last one in the list, we reset it back to the begining of the list
                        if (i == weaponTypes.Count)
                        {
                            i = 0;
                        }
                        //Whatever the iteration value is, the current weapon is this iteration value in the weaponTypes list
                        currentWeapon = weaponTypes[i];
                        character.currentWeaponSelected = i;
                        PlayerPrefs.SetInt("CurrentWeapon", i);
                        //Restes the currentTimeBetweenShots value to the currentWeapon timeBetweenShots value
                        currentTimeBetweenShots = currentWeapon.timeBetweenShots;
                    }
                }
            }

            for (int i = 0; i < totalPools.Count; i++)
            {
                if (currentWeapon.projectile.tag == totalPools[i].tag)
                {
                    projectileParentFolder = totalPools[i].gameObject;
                    currentProjectile = currentWeapon.projectile;
                    matched = true;
                }
            }
            if (currentWeapon.projectile.tag == "GrapplingHook")
            {
                grapplingHook.enabled = true;
            }
            else
            {
                grapplingHook.removed = true;
                grapplingHook.RemoveGrapple();
                grapplingHook.enabled = false;
            }
            if (!matched)
            {
                NewPool();
            }
            if (currentWeapon.canResetPool)
            {
                bulletsToReset.Clear();
            }
        }

        //Creates a new pool of projectiles if that pool doesn't currently exist
        protected virtual void NewPool()
        {
            GameObject newPool = new GameObject();
            projectileParentFolder = newPool;
            objectPooler.CreatePool(currentWeapon, currentPool, projectileParentFolder, this);
            currentProjectile = currentWeapon.projectile;
            //If the currentWeapon resets projectiles, resets the list to a fresh list
            if (currentWeapon.canResetPool)
            {
                bulletsToReset.Clear();
            }
        }

        //Places the projectile based on the gunBarrel position and rotation, as well as if the Player is facing left or right, as well as if the Player is wall sliding
        protected virtual void PlaceProjectile()
        {
            currentProjectile.transform.position = gunBarrel.position;
            currentProjectile.transform.rotation = gunRotation.rotation;
            currentProjectile.SetActive(true);
            if (!character.isFacingLeft)
            {
                if (character.isWallSliding)
                {
                    currentProjectile.GetComponent<Projectile>().left = true;
                }
                else
                    currentProjectile.GetComponent<Projectile>().left = false;
            }
            else
            {
                if (character.isWallSliding)
                {
                    currentProjectile.GetComponent<Projectile>().left = false;
                }
                else
                    currentProjectile.GetComponent<Projectile>().left = true;
            }
            currentProjectile.GetComponent<Projectile>().fired = true;
        }
    }
}