using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class ObjectPooler : MonoBehaviour
    {
        //This is a Singleton pattern that creates an object pooling system in every scene at Awake
        private static ObjectPooler instance;
        public static ObjectPooler Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("ObjectPooler");
                    obj.AddComponent<ObjectPooler>();
                }
                return instance;
            }
        }

        //The Awake method runs before Start, but this Awake method in this script makes sure there is only one object pooling system in a scene at a time, and if somehow there are more than one, it gets rid of any other one.
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        //This is the current item that is being pooled based on whatever Enemy or Player weapon script is calling it
        private GameObject currentItem;

        //This method will create a base amount of objects to pool from this method is called by Player weapon script
        public void CreatePool(WeaponTypes weapon, List<GameObject> currentPool, GameObject projectileParentFolder, Weapon weaponScript)
        {
            weaponScript.totalPools.Add(projectileParentFolder);
            for (int i = 0; i < weapon.amountToPool; i++)
            {
                currentItem = Instantiate(weapon.projectile);
                currentItem.SetActive(false);
                currentPool.Add(currentItem);
                currentItem.transform.SetParent(projectileParentFolder.transform);
            }
            projectileParentFolder.name = weapon.name;
            projectileParentFolder.tag = weapon.projectile.tag;
        }

        //This method will create a base amount of objects to pool from this method is called by Enemy weapon script
        public void CreateEnemyPool(WeaponTypes weapon, List<GameObject> currentPool, GameObject projectileParentFolder, EnemyWeapon weaponScript)
        {
            weaponScript.totalPools.Add(projectileParentFolder);
            for (int i = 0; i < weapon.amountToPool; i++)
            {
                currentItem = Instantiate(weapon.projectile);
                currentItem.SetActive(false);
                currentPool.Add(currentItem);
                currentItem.transform.SetParent(projectileParentFolder.transform);
            }
            projectileParentFolder.name = weapon.name;
            projectileParentFolder.tag = weapon.projectile.tag;
        }

        //This method will draw a projectile from the available pool that containst the projectiles it should grab from; this is called by the Player weapon script each time the weapon is shot.
        public virtual GameObject GetObject(List<GameObject> currentPool, WeaponTypes weapon, Weapon weaponScript, GameObject projectileParentFolder, string tag)
        {
            for (int i = 0; i < currentPool.Count; i++)
            {
                if (!currentPool[i].activeInHierarchy && currentPool[i].tag == tag)
                {
                    if (weapon.canResetPool && weaponScript.bulletsToReset.Count < weapon.amountToPool)
                    {
                        weaponScript.bulletsToReset.Add(currentPool[i]);
                    }
                    return currentPool[i];
                }
            }
            foreach (GameObject item in currentPool)
            {
                //This if statement block handles the logic to allow a pool to expand and create new game objects if there are none left in the pool that was created from the CreatePool method
                if (weapon.canExpandPool && item.tag == tag)
                {
                    currentItem = Instantiate(weapon.projectile);
                    currentItem.SetActive(false);
                    currentPool.Add(currentItem);
                    currentItem.transform.SetParent(projectileParentFolder.transform);
                    return currentItem;
                }
                //This if statement block handles the logic to reset the projectiles oldest first if there are no objects left in the pool; a popular game that uses this is Mega Man, in that Mega Man can only fire three shots before the oldest shot is removed and placed back at the tip of gun.
                if (weapon.canResetPool && item.tag == tag)
                {
                    currentItem = weaponScript.bulletsToReset[0];
                    weaponScript.bulletsToReset.RemoveAt(0);
                    currentItem.SetActive(false);
                    weaponScript.bulletsToReset.Add(currentItem);
                    currentItem.GetComponent<Projectile>().DestroyProjectile();
                    return currentItem;
                }
            }
            return null;
        }

        //Same thing as above method, but strictly for Enemy projectiles.
        public virtual GameObject GetEnemyObject(List<GameObject> currentPool, WeaponTypes weapon, GameObject projectileParentFolder, string tag)
        {
            for (int i = 0; i < currentPool.Count; i++)
            {
                if (!currentPool[i].activeInHierarchy && currentPool[i].tag == tag)
                {
                    return currentPool[i];
                }
            }
            foreach (GameObject item in currentPool)
            {
                if (weapon.canExpandPool && item.tag == tag)
                {
                    currentItem = Instantiate(weapon.projectile);
                    currentItem.SetActive(false);
                    currentPool.Add(currentItem);
                    currentItem.transform.SetParent(projectileParentFolder.transform);
                    return currentItem;
                }
            }
            return null;
        }
    }
}