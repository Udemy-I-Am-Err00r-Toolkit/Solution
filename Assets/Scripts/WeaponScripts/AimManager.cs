using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.IK;

namespace MetroidvaniaTools
{
    public class AimManager : Abilities
    {

        public Solver2D aimingGun;
        public Solver2D aimingLeftHand;
        public Solver2D notAimingGun;
        public Solver2D notAimingLeftHand;
        public Transform whereToAim;
        public Transform whereToPlaceHand;
        public Transform origin;
        public Bounds bounds;
        [SerializeField]
        protected float autoTargetRadius;
        private bool lockedOn;
        [HideInInspector]
        public bool aiming;

        protected override void Initialization()
        {
            base.Initialization();
            aimingGun.enabled = false;
            aimingLeftHand.enabled = false;
            bounds.center = origin.position;
        }

        protected virtual void FixedUpdate()
        {
            Aiming();
            DirectionalAim();
            bounds.center = origin.position;
        }

        protected virtual void Aiming()
        {
            if (input.AimingHeld() || DirectionalAim())
            {
                if (input.AimingHeld())
                {
                    CheckForTargets();
                    if (!lockedOn && !DirectionalAim())
                    {
                        NotAiming();
                        return;
                    }
                }
                ChangeArms();
                aiming = true;
                return;
            }
            NotAiming();
        }

        protected virtual void CheckForTargets()
        {
            GameObject[] targets;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(weapon.gunBarrel.position, autoTargetRadius);
            if (colliders.Length > 0)
            {
                targets = new GameObject[colliders.Length];
                for (int i = 0; i < colliders.Length; i++)
                {
                    targets[i] = colliders[i].gameObject;
                }
                LockOnTarget(targets);
            }
        }

        //Does the actual checking of gameobjects for which ones are targets, and of those that are targets, finds the closest one.
        protected virtual GameObject LockOnTarget(GameObject[] targets)
        {
            Transform closestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (GameObject potentialTarget in targets)
            {
                if (potentialTarget.tag == "Target")
                {
                    Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        closestTarget = potentialTarget.transform;
                    }
                }
            }
            if (closestTarget != null)
            {
                lockedOn = true;
                whereToAim.transform.position = closestTarget.position;
                aimingGun.transform.GetChild(0).position = whereToAim.transform.position;
                aimingLeftHand.transform.GetChild(0).position = whereToPlaceHand.transform.position;
                return closestTarget.gameObject;
            }
            lockedOn = false;
            return null;
        }

        protected virtual void NotAiming()
        {
            lockedOn = false;
            aiming = false;
            ChangeArms();
        }

        public virtual bool DirectionalAim()
        {
            if(character.isOnLadder)
            {
                return false;
            }
            if (input.UpHeld())
            {
                whereToAim.transform.position = new Vector2(bounds.center.x, bounds.max.y);
                return true;
            }
            if (input.DownHeld())
            {
                whereToAim.transform.position = new Vector2(bounds.center.x, bounds.min.y);
                return true;
            }
            if (input.TiltedUpHeld())
            {
                if (!character.isFacingLeft)
                {
                    whereToAim.transform.position = new Vector2(bounds.max.x, bounds.max.y);
                }
                else
                {
                    whereToAim.transform.position = new Vector2(bounds.min.x, bounds.max.y);
                }
                return true;
            }
            if (input.TiltedDownHeld())
            {
                if (!character.isFacingLeft)
                {
                    whereToAim.transform.position = new Vector2(bounds.max.x, bounds.min.y);
                }
                else
                {
                    whereToAim.transform.position = new Vector2(bounds.min.x, bounds.min.y);
                }
                return true;
            }
            return false;
        }

        public virtual void ChangeArms()
        {
            if (weapon.currentTimeTillChangeArms > 0 || aiming)
            {

                notAimingGun.enabled = false;
                notAimingLeftHand.enabled = false;
                aimingGun.enabled = true;
                aimingLeftHand.enabled = true;
            }
            if (weapon.currentTimeTillChangeArms < 0 && !aiming)
            {
                notAimingGun.enabled = true;
                notAimingLeftHand.enabled = true;
                aimingGun.enabled = false;
                aimingLeftHand.enabled = false;
            }
        }

        private void OnDrawGizmos()
        {
            weapon = GetComponent<Weapon>();
            Gizmos.DrawWireSphere(weapon.gunBarrel.position, autoTargetRadius);
            Gizmos.DrawWireCube(origin.position, bounds.size);
        }
    }
}