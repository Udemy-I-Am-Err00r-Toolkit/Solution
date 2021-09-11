using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //Scriptable object that contains the different data for the different weapons, either Enemy or Player
    [CreateAssetMenu(fileName = "WeaponType", menuName = "Metroidvania/Weapons", order = 1)]
    public class WeaponTypes : ScriptableObject
    {
        //Check this if it is a melee weapon
        public bool melee;
        //If Melee, then this would be the GameObject for swipe and other propoerties
        public GameObject meleeWeapon;
        //The actual prefab of the projectile
        public GameObject projectile;
        //How fast the projectile moves when fired
        public float projectileSpeed;
        //How much the CreatePool method should make of this projectile when loading scene
        public int amountToPool;
        //How long the projectile is active before it is removed from scene if it doesn't collide with anything
        public float lifeTime;
        //A bool that allows the gun to be fired automatically
        public bool automatic;
        //How much time need between each shot; this is true for automatic weapons
        public float timeBetweenShots;
        //Will allow the ObjectPooler to add more projectiles if needed
        public bool canExpandPool;
        //Will allow the ObjectPooler to deactivate oldest active projectile to keep firing weapon
        public bool canResetPool;

        //Because you can't have the ObjectPooler both expand the pool if there are no more projectiles left in pool and reset at the same time, this ensures if by mistake both are selected, then the canResetPool is false.
        protected virtual void OnEnable()
        {
            if (canExpandPool && canResetPool)
            {
                canResetPool = false;
            }
        }
    }
}