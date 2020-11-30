using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class Weapon : Abilities
    {
        [Header(" ")]
        [Header("This is the script that would house all the data for the weapon")]
        [SerializeField]
        protected List<WeaponTypes> weaponTypes;
        [Tooltip("This variable is actually really cool")]
        public Transform gunBarrel;
        public Transform gunRotation;

        [HideInInspector]
        public List<GameObject> currentPool = new List<GameObject>();
        [HideInInspector]
        public List<GameObject> bulletsToReset = new List<GameObject>();
        [HideInInspector]
        public List<GameObject> totalPools;

        public GameObject currentProjectile;
        public WeaponTypes currentWeapon;
        public float currentTimeTillChangeArms;

        private GameObject projectileParentFolder;
        private float currentTimeBetweenShots;

        protected override void Initialization()
        {
            base.Initialization();
            ChangeWeapon();
        }

        protected virtual void Update()
        {
            if (gameManager.gamePaused)
            {
                return;
            }
            if (input.WeaponFired())
            {
                FireWeapon();
            }
            if(input.ChangeWeaponPressed())
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

        protected virtual void FireWeapon()
        {
            currentTimeTillChangeArms = currentWeapon.lifeTime;
            aimManager.ChangeArms();
            currentProjectile = objectPooler.GetObject(currentPool, currentWeapon, this, projectileParentFolder, currentWeapon.projectile.tag);
            if(currentProjectile != null)
            {
                Invoke("PlaceProjectile", .1f);
            }
            currentTimeBetweenShots = currentWeapon.timeBetweenShots;
        }

        protected virtual void FireWeaponHeld()
        {
            if(input.WeaponFiredHeld())
            {
                if(currentWeapon.automatic)
                {
                    currentTimeTillChangeArms = currentWeapon.lifeTime;
                    aimManager.ChangeArms();
                    currentTimeBetweenShots -= Time.deltaTime;
                    if(currentTimeBetweenShots < 0)
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

        protected virtual void PointGun()
        {
            if (!aimManager.aiming)
            {
                if (!character.isFacingLeft)
                {
                    if(character.isWallSliding)
                    {
                        aimManager.whereToAim.position = new Vector2(aimManager.bounds.min.x, aimManager.bounds.center.y);
                    }
                    else
                        aimManager.whereToAim.position = new Vector2(aimManager.bounds.max.x, aimManager.bounds.center.y);
                }
                else
                {
                    if (character.isWallSliding)
                    {
                        aimManager.whereToAim.position = new Vector2(aimManager.bounds.max.x, aimManager.bounds.center.y);
                    }
                    else 
                        aimManager.whereToAim.position = new Vector2(aimManager.bounds.min.x, aimManager.bounds.center.y);
                }
            }
            aimManager.aimingGun.transform.GetChild(0).position = aimManager.whereToAim.position;
            aimManager.aimingLeftHand.transform.GetChild(0).position = aimManager.whereToPlaceHand.position;
        }

        protected virtual void NegateTimeTillChangeArms()
        {
            if(grapplingHook.connected)
            {
                return;
            }
            currentTimeTillChangeArms -= Time.deltaTime;
        }

        protected virtual void ChangeWeapon()
        {
            bool matched = new bool();
            for(int i = 0; i < weaponTypes.Count; i ++)
            {
                if(currentWeapon == null)
                {
                    currentWeapon = weaponTypes[0];
                    currentTimeBetweenShots = currentWeapon.timeBetweenShots;
                    NewPool();
                    return;
                }
                else
                {
                    if(weaponTypes[i] == currentWeapon)
                    {
                        i++;
                        if(i == weaponTypes.Count)
                        {
                            i = 0;
                        }
                        currentWeapon = weaponTypes[i];
                        currentTimeBetweenShots = currentWeapon.timeBetweenShots;
                    }
                }
            }
            for(int i = 0; i < totalPools.Count; i ++)
            {
                if(currentWeapon.projectile.tag == totalPools[i].tag)
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
            if(currentWeapon.canResetPool)
            {
                bulletsToReset.Clear();
            }
        }

        protected virtual void NewPool()
        {
            GameObject newPool = new GameObject();
            projectileParentFolder = newPool;
            objectPooler.CreatePool(currentWeapon, currentPool, projectileParentFolder, this);
            currentProjectile = currentWeapon.projectile;
            if (currentWeapon.canResetPool)
            {
                bulletsToReset.Clear();
            }
        }

        protected virtual void PlaceProjectile()
        {
            currentProjectile.transform.position = gunBarrel.position;
            currentProjectile.transform.rotation = gunRotation.rotation;
            currentProjectile.SetActive(true);
            if(!character.isFacingLeft)
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