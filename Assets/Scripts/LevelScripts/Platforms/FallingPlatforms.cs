using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class FallingPlatforms : PlatformManager
    {
        [SerializeField]
        protected enum TypesOfFallingPlatforms { Destructive, Donut }
        [SerializeField]
        protected TypesOfFallingPlatforms platformType;
        [SerializeField]
        protected float timeTillDoSomething;
        [SerializeField]
        protected float timeFalling;
        [SerializeField]
        protected float timeTillReset;
        [SerializeField]
        protected bool destroyPlatform;
        protected Vector3 originalPlatformPosition;
        protected float currentTimeTillDoSomething;
        protected float currentTimeFalling;
        protected bool platformFalling;
        protected bool destructivePlatform;


        protected override void Initialization()
        {
            base.Initialization();
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
            if(CollisionCheck() || destructivePlatform)
            {
                if(platformType == TypesOfFallingPlatforms.Destructive)
                {
                    destructivePlatform = true;
                    DestructivePlatform();
                }
                if(platformType == TypesOfFallingPlatforms.Donut)
                {
                    DonutPlatform();
                }
            }
            if (!CollisionCheck() && platformType == TypesOfFallingPlatforms.Donut)
            {
                currentTimeTillDoSomething = timeTillDoSomething;
            }
        }

        protected virtual void DestructivePlatform()
        {
            currentTimeTillDoSomething -= Time.deltaTime;
            if (currentTimeTillDoSomething < 0)
            {
                platformCollider.enabled = false;
                platformFalling = true;
            }
        }

        protected virtual void DonutPlatform()
        {
            currentTimeTillDoSomething -= Time.deltaTime;
            if (currentTimeTillDoSomething < 0)
            {
                platformFalling = true;
            }
        }

        protected virtual void PlatformFalling()
        {
            if(platformFalling)
            {
                currentTimeFalling -= Time.deltaTime;
                platformRB.bodyType = RigidbodyType2D.Dynamic;
                platformRB.constraints = RigidbodyConstraints2D.None;
                platformRB.constraints = RigidbodyConstraints2D.FreezeRotation;
                if(currentTimeFalling < 0 && !destroyPlatform)
                {
                    gameObject.SetActive(false);
                    Invoke("PutPlatformBack", timeTillReset);
                }
                if(currentTimeFalling < 0 && destroyPlatform)
                {
                    Destroy(gameObject);
                }
            }
        }

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