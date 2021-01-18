using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script will manage Platforms that would fall if the player is on top; there are different types of platforms, and you can customize how long the Player needs to interact with the platform before it does something
    [RequireComponent(typeof(Rigidbody2D))]
    public class FallingPlatforms : PlatformManager
    {
        //An enum that defines names for the different types of falling platforms; Destructive platforms break and cause the Player to fall through the platform when it breaks, Donut platforms fall and allow the Player to 'ride' the platform as it's falling
        [SerializeField]
        protected enum TypesOfFallingPlatforms { Destructive, Donut }
        [SerializeField]
        protected TypesOfFallingPlatforms platformType;
        //How long the Player needs to be standing consecutively on the platform before it falls
        [SerializeField]
        protected float timeTillDoSomething;
        //How long the platform should fall for before it is disabled from the scene
        [SerializeField]
        protected float timeFalling;
        //If the platform should go back to the original place after it falls, this is how much time needs to pass after it fell before it is placed in its original location
        [SerializeField]
        protected float timeTillReset;
        //A bool that checks if the platform should be removed from the scene until it loads again, or if it should be placed back in its original position after the timeTillReset value is zero
        [SerializeField]
        protected bool destroyPlatform;
        //Falling platforms need a Rigidbody2D component to apply gravity
        protected Rigidbody2D platformRB;
        //The original position the platform should go to if it isn't being destroyed after falling
        protected Vector3 originalPlatformPosition;
        //The current amount of time the player has been consecutively standing on top of a platform
        protected float currentTimeTillDoSomething;
        //The current amount of time the platform has been falling for
        protected float currentTimeFalling;
        //A bool that quickly manages if the platform is falling
        protected bool platformFalling;
        //This bool will automatically ensure the platform is destroyed after a set amount of time after the Player steps on top of it; the Player does not need to be standing on it consectuviley for this bool to work.
        protected bool destructivePlatform;


        protected override void Initialization()
        {
            base.Initialization();
            platformRB = GetComponent<Rigidbody2D>();
            platformRB.bodyType = RigidbodyType2D.Kinematic;
            currentTimeTillDoSomething = timeTillDoSomething;
            currentTimeFalling = timeFalling;
            originalPlatformPosition = transform.position;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            PlatformCheck();
            PlatformFalling();
        }

        protected virtual void PlatformCheck()
        {
            //Checks to see if the player is standing on top of it or if the platform has the destructivePlatform bool set to true
            if (CollisionCheck() || destructivePlatform)
            {
                //If the platform is a destructive type
                if (platformType == TypesOfFallingPlatforms.Destructive)
                {
                    //It sets the bool that automatically causes the platform to fall to true once the Player sets foot on top
                    destructivePlatform = true;
                    //This method negates the time for currentTimeTillDoSomething automatically and has the platform start to fall once that time is less than zero
                    DestructivePlatform();
                }
                //If the platform is a Donut type
                if (platformType == TypesOfFallingPlatforms.Donut)
                {
                    //Runs the method to have the platform fall once the Player has been standing on it for the needed amount of time
                    DonutPlatform();
                }
            }
            //If the Player is not standing on top of the platform and it is a Donut type, we reset the amount of currentTimeTillDoSomething back to its original value
            if (!CollisionCheck() && platformType == TypesOfFallingPlatforms.Donut)
            {
                currentTimeTillDoSomething = timeTillDoSomething;
            }
        }

        //This method runs and continues to run the second the Player stands on top of a platform
        protected virtual void DestructivePlatform()
        {
            currentTimeTillDoSomething -= Time.deltaTime;
            if (currentTimeTillDoSomething < 0)
            {
                platformCollider.enabled = false;
                platformFalling = true;
            }
        }

        //This method only runs as long as the Player is standing on top of the platform
        protected virtual void DonutPlatform()
        {
            currentTimeTillDoSomething -= Time.deltaTime;
            if (currentTimeTillDoSomething < 0)
            {
                platformFalling = true;
            }
        }

        //This method causes the platform to start falling once currentTimeTillDoSomething reaches zero; the falling is more for animation purposes, but depending on if you want to destroy the platform after it is finished falling, this method will call the PutPlatformBack method, or destroy the platform until the scene loads again
        protected virtual void PlatformFalling()
        {
            if (platformFalling)
            {
                //Negates currentTimeFalling
                currentTimeFalling -= Time.deltaTime;
                //Changes the body type on the Rigidbody2D so gravity is affected
                platformRB.bodyType = RigidbodyType2D.Dynamic;
                //Ensures that the position of the platform is not constrained by the Rigidbody2D component
                platformRB.constraints = RigidbodyConstraints2D.None;
                //Ensures that the rotation is constrained and doesn't allow the platform to rotate while falling
                platformRB.constraints = RigidbodyConstraints2D.FreezeRotation;
                //If you don't want to destroy the platform from the scene, it will put the platform back in its original location after timeTillReset value
                if (currentTimeFalling < 0 && !destroyPlatform)
                {
                    gameObject.SetActive(false);
                    Invoke("PutPlatformBack", timeTillReset);
                }
                //If you do want the platform to be destroyed from the scene until the scene loads again, this method will do that
                if (currentTimeFalling < 0 && destroyPlatform)
                {
                    Destroy(gameObject);
                }
            }
        }

        //Puts the platform back into its original place, turns off gravity again, freezes the rotation again, and resetes a ton of other values back to the original values to allow the platform to fall all over again, depending on if it is destructive or donut
        protected virtual void PutPlatformBack()
        {
            platformRB.bodyType = RigidbodyType2D.Kinematic;
            platformRB.constraints = RigidbodyConstraints2D.FreezeAll;
            currentTimeTillDoSomething = timeTillDoSomething;
            currentTimeFalling = timeFalling;
            platformFalling = false;
            destructivePlatform = false;
            platformCollider.enabled = true;
            transform.position = originalPlatformPosition;
            gameObject.SetActive(true);
        }
    }
}