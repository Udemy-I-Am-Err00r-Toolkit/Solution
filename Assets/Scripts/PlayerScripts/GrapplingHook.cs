using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class GrapplingHook : Abilities
    {
        //How long the grappling hook can grow to
        [SerializeField]
        protected float hookLength;
        //How short the grappling hook can be
        [SerializeField]
        protected float minHookLength;
        //How fast the player moves towards and away the hook while in a grapple hanging state
        [SerializeField]
        protected float hookReelSpeed;
        //How much the player gets launched vertically when released from grappling hook
        [SerializeField]
        protected float verticalForce;
        //How much the player gets launched horizontally when released from grappling hook
        [SerializeField]
        protected float horizontalForce;
        //Handles if the hook is connected to an object; this is fed to other scripts that might need to know this information as well
        [HideInInspector]
        public bool connected;
        //Handles if the hook is no longer connected to an object
        [HideInInspector]
        public bool removed;
        //What game object the hook is connected to
        [HideInInspector]
        public GameObject objectConnectedTo;
        //The prefab that will be drawn from the guntip to the hook to show the trail the hook is shot from
        public GameObject hookTrail;
        //A value of how far the player is from the object the grappling hook is connected to; this value works with hookLength and minHookLength to keep the player within those lengths
        private float distanceFromHookedObject;
        //Determines whether or not the hookTrail should be shown
        private bool canDrawLine;

        protected override void Initialization()
        {
            base.Initialization();
            //Makes sure the hookTail gameobject is invisible when the game loads
            hookTrail.SetActive(false);
            //Makes sure the grapplingHook is inactive when loading if not the current weapon; we do this because th grappling hook handles different logic than any other weapon
            if (weapon.currentWeapon != null && weapon.currentWeapon.projectile.tag != "GrapplingHook")
            {
                enabled = false;
            }
        }

        protected virtual void FixedUpdate()
        {
            GrappleFired();
            RemoveGrapple();
        }

        //This method handles everything that should happen when the grappling hook input is true, most of this logic is toggling things on, and setting the distanceFromHookedObject
        protected virtual void GrappleFired()
        {
            if (input.WeaponFiredHeld() && weapon.currentProjectile != null && weapon.currentProjectile.GetComponent<Projectile>().fired && weapon.currentProjectile.tag == "GrapplingHook")
            {
                distanceFromHookedObject = Vector2.Distance(weapon.gunBarrel.position, weapon.currentProjectile.transform.position);
                canDrawLine = true;
                Invoke("DrawLine", .1f);
                //If the distanceFromHookedObject value is greater than hookLength and the grappling hook is not connected to another object, than this will disable the hook
                if (distanceFromHookedObject > hookLength)
                {
                    canDrawLine = false;
                    DrawLine();
                    weapon.currentProjectile.GetComponent<Projectile>().DestroyProjectile();
                }
            }
            //If the WeaponFiredHeld bool is false, then it disables the grappling hook regardless if the hook is connected or not
            else
            {
                canDrawLine = false;
                DrawLine();
            }
            //If the hook is connected to something while the input is true, we run this method
            if (connected)
            {
                GrappleHanging();
            }
        }

        //If canDrawLine is true, it will draw the grappling hook trail; if not, it will handle all the logic of removing it
        protected virtual void DrawLine()
        {
            if (canDrawLine)
            {
                hookTrail.SetActive(true);
                hookTrail.transform.position = weapon.gunBarrel.position;
                hookTrail.transform.rotation = weapon.gunRotation.rotation;
                hookTrail.GetComponent<SpriteRenderer>().size = new Vector2(distanceFromHookedObject, .64f);
            }
            else
            {
                distanceFromHookedObject = 0;
                hookTrail.GetComponent<SpriteRenderer>().size = new Vector2(0, .64f);
                hookTrail.SetActive(false);
                if (weapon.currentProjectile != null && weapon.currentProjectile.tag == "GrapplingHook")
                {
                    weapon.currentProjectile.GetComponent<Projectile>().DestroyProjectile();
                }
            }
        }

        //Handles all the logic that should be applied to the Player when the hook is connected with a gameobject that should enter the GrappleHanging state; most of this logic is positioning the weapon and checking for input when the player is grappl hanging
        protected virtual void GrappleHanging()
        {
            //Allows the player to rotate around when in the grapple hanging state
            rb.freezeRotation = false;
            //Turns on the grappling hook animation
            anim.SetBool("GrapplingHook", true);
            //This handles the amount of speed the Player should move towards and away from the game object the player is hooked onto
            float step = hookReelSpeed * Time.deltaTime;
            //Makes sure that the weapon is aiming at the object the hook is connected to
            aimManager.whereToAim.transform.position = objectConnectedTo.transform.position;
            //Makes sure the aiming IKs are in their correct positions when grapple hanging
            aimManager.aimingGun.transform.GetChild(0).position = aimManager.whereToAim.position;
            aimManager.aimingLeftHand.transform.GetChild(0).position = aimManager.whereToPlaceHand.position;
            //Ensures the hook doesn't become inactive while it is connected to a hookable game object
            weapon.currentProjectile.GetComponent<Projectile>().projectileLifeTime = weapon.currentWeapon.lifeTime;
            //Makes sure the hook's position is at the center of the hookable game object
            weapon.currentProjectile.transform.position = objectConnectedTo.transform.position;
            //Manages the distanceFromHookedObject value while grapple hanging
            distanceFromHookedObject = Vector2.Distance(weapon.gunBarrel.position, objectConnectedTo.transform.position);
            //Moves the player towards the object connected to while grapple hanging
            if (input.UpHeld() && distanceFromHookedObject >= minHookLength)
            {
                transform.position = Vector2.MoveTowards(transform.position, objectConnectedTo.transform.position, step);
            }
            //Moves the player away from object connected to while grapple hanging
            if (input.DownHeld() && distanceFromHookedObject < hookLength - .5f)
            {
                transform.position = Vector2.MoveTowards(transform.position, objectConnectedTo.transform.position, -1 * step);
            }
        }

        //Toggles everything back to normal when no longer connected with the grappling hook or no longer holding the input
        public virtual void RemoveGrapple()
        {
            //Checks for different removal conditions
            if (!input.WeaponFiredHeld() || weapon.currentTimeTillChangeArms <= 0 || removed)
            {
                //If somehow the removed bool is still false, it toggles it back to true here
                removed = true;
                //If the player is currently connected to a hookable object, handles the logic to set them back to a normal state
                if (connected)
                {
                    //Turns off the grapplingHook animation
                    anim.SetBool("GrapplingHook", false);
                    //Ensures the connected bool is back to false
                    connected = false;
                    //Turns off the HingeJoint2D component on the grappling hook
                    objectConnectedTo.GetComponent<HingeJoint2D>().enabled = false;
                    //Gives the player a vertical boost when removing from a grapple hanging state
                    objectConnectedTo = null; rb.AddForce(Vector2.up * verticalForce);
                    //Gives the player a horizontal boost when removing from a grapple hanging state, depending on what direction they are facing
                    if (!character.isFacingLeft)
                    {
                        rb.AddForce(Vector2.right * horizontalForce);
                    }
                    else
                    {
                        rb.AddForce(Vector2.left * horizontalForce);
                    }
                    //Very quick coroutine that makes sure the boost from being removed from grappling hook is applied
                    StartCoroutine(DisableMovement());
                }
                //Disables the grappling hook projectile and sets it's position back to where it should be
                if (weapon.currentProjectile != null && weapon.currentProjectile.tag == "GrapplingHook")
                {
                    weapon.currentProjectile.transform.position = weapon.gunBarrel.position;
                    weapon.currentProjectile.GetComponent<Projectile>().DestroyProjectile();
                }
                //Runs quick method that gets rid of projectile and hook trail from scene
                ReturnHook();
            }
        }

        //Quick method that resets the grappling hook when it should be removed from the scene
        protected virtual void ReturnHook()
        {
            canDrawLine = false;
            connected = false;
            DrawLine();
        }

        //Very quick method that runs to ensure boost that launches player from grapple hanging state is allowed
        protected virtual IEnumerator DisableMovement()
        {
            movement.enabled = false;
            yield return new WaitForSeconds(.1f);
            movement.enabled = true;
        }
    }
}