using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

namespace MetroidvaniaTools
{
    public class AimManager : Abilities
    {
        //The game object that contains the IK for the gun when it is in an aiming state
        public Solver2D aimingGun;
        //The game object that contains the IK for the offhand when it is in an aiming state
        public Solver2D aimingLeftHand;
        //The game object that contains the IK for the gun when it isn't in an aiming state
        public Solver2D notAimingGun;
        //The game object that contains the IK for the offhand when it isn't in an aiming state
        public Solver2D notAimingLeftHand;
        //A transform the weapon barrel will be pointed towards when aiming; if not aiming, this transform is right in front of Player
        public Transform whereToAim;
        //A transform the offHand will be resting on to simulate steadying and holding the weapon when firing and aiming
        public Transform whereToPlaceHand;
        //An origin point for the aiming bounds to center around
        public Transform origin;
        //A component that creates world coordinates for the whereToAim transform to move around when the player is aiming; when not aiming, this will be in front of the Player
        public Bounds bounds;
        //How far around the Player is allowed to directly aim at something if within this radius
        [SerializeField]
        protected float autoTargetRadius;
        //Bool that manages if the player is locked on to a target while auto aiming
        private bool lockedOn;
        //A bool that is fed to other scripts to let those scripts know the player is aiming at something other than right in front of Player
        [HideInInspector]
        public bool aiming;

        protected override void Initialization()
        {
            base.Initialization();
            //Ensures the IKs that control aiming are set to false
            aimingGun.enabled = false;
            aimingLeftHand.enabled = false;
            //Creates the center point around where the bounds should originate from
            bounds.center = origin.position;
        }

        protected virtual void FixedUpdate()
        {
            Aiming();
            DirectionalAim();
            //bounds.center = origin.position;
        }

        protected virtual void Aiming()
        {
            //Checks input to see autoAim button is being held or if DirectionalAim returned true
            if (input.AimingHeld() || DirectionalAim())
            {
                //Does additional checks if the autoAim button is held
                if (input.AimingHeld())
                {
                    //Runs a method to see if there are any autoAim targets within the autoTargetRadius
                    CheckForTargets();
                    //If there aren't any targets and DirectionalAim returns false, run the NotAiming method and return out.
                    if (!lockedOn && !DirectionalAim())
                    {
                        NotAiming();
                        return;
                    }
                }
                //If there is a target in the autoTargetRadius and autoAim is held or if DirectionalAim returns true, run the ChangeArms method and set aiming to true
                ChangeArms();
                aiming = true;
                return;
            }
            //If no input is detected and DirectionalAim returns false, run the NotAiming method
            NotAiming();
        }

        //Checks to see if there are any game objects within the autoTargetRadius range
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
                //If there is a gameobject within range for auto target, we run the LockOnTarget method to find out which object is closest
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

        //Quickly toggles a few bools to false and runs the ChangeArms method when not aiming at anything
        protected virtual void NotAiming()
        {
            lockedOn = false;
            aiming = false;
            ChangeArms();
        }

        //Checks input in the different directions to see if the Player should be aiming in a direction other than right in front; this is seperate check from auto aiming
        public virtual bool DirectionalAim()
        {
            //When on ladder, directional aiming is false so the animations are more consistent with gameplay
            if (character.isOnLadder)
            {
                return false;
            }
            //Moves the whereToAim transform directly above the player when aiming up
            if (input.UpHeld())
            {
                whereToAim.transform.position = new Vector2(bounds.center.x, bounds.max.y);
                return true;
            }
            //Moves the whereToAim transform directly below the player when aiming down
            if (input.DownHeld())
            {
                whereToAim.transform.position = new Vector2(bounds.center.x, bounds.min.y);
                return true;
            }
            //Moves the whereToAim transform directly diagnoally above the player when aiming diagonally up
            if (input.TiltedUpHeld())
            {
                //Depending on what direction you are facing, this could be top left or top right of player
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
            //Moves the whereToAim transform directly diagnoally below the player when aiming diagonally down
            if (input.TiltedDownHeld())
            {
                //Depending on what direction you are facing, this could be bottom left or bottom right of player
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
            //If none of these keys are pressed, this method returns false
            return false;
        }

        //A method that manages toggling the different IKs to their correct active or inactive states
        public virtual void ChangeArms()
        {
            //If the weapon was just fired or aiming, then we turn on the aiming IKs
            if (weapon.currentTimeTillChangeArms > 0 || aiming)
            {

                notAimingGun.enabled = false;
                notAimingLeftHand.enabled = false;
                aimingGun.enabled = true;
                aimingLeftHand.enabled = true;
            }
            //If the weapon was fired past the time it should or no longer aiming, then we turn off the aiming IKs
            if (weapon.currentTimeTillChangeArms < 0 && !aiming)
            {
                notAimingGun.enabled = true;
                notAimingLeftHand.enabled = true;
                aimingGun.enabled = false;
                aimingLeftHand.enabled = false;
            }
        }

        //Convenience feature that shows the aiming bounds so you can see it in the scene window
        private void OnDrawGizmos()
        {
            weapon = GetComponent<Weapon>();
            Gizmos.DrawWireSphere(weapon.gunBarrel.position, autoTargetRadius);
            Gizmos.DrawWireCube(origin.position, bounds.size);
        }
    }
}