using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script will have a lot of the standard variables that will be shared between all the different types of Platforms; this script will also check to see if the Player is standing on top of a Platform, as many of the Platforms that will use this script will need to check
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlatformManager : Managers
    {
        //The BoxCollider2D that will automatically be added to any Platform that contains this script
        protected BoxCollider2D platformCollider;
        //The layer that is attached to the player, this will be hardcoded based on whatever the string reference we have in the Layer drop down for the player
        protected LayerMask playerLayer;

        protected override void Initialization()
        {
            base.Initialization();
            platformCollider = GetComponent<BoxCollider2D>();
            //This value is a string reference to whatever you have the "Player" named as in the Layer drop down in the inspector window; depending on your native language, you might have it named something different, so you might need to change it here or on the Layer dropdown, but the string reference needs to match the "Player" in the Layer drop down
            playerLayer = LayerMask.GetMask("Player");
        }

        protected virtual void FixedUpdate()
        {
            CollisionCheck();
        }

        //This method checks to see if the Player is standing on top of the ledge; a lot of the custom platforms will require the Player to be standing on top for the custom logic on those platforms to flow
        protected virtual bool CollisionCheck()
        {
            RaycastHit2D hit = Physics2D.BoxCast(new Vector2(platformCollider.bounds.center.x, platformCollider.bounds.max.y), new Vector2(platformCollider.bounds.size.x - .1f, .5f), 0, Vector2.up, .05f, playerLayer);
            if (hit)
            {
                return true;
            }
            return false;
        }
    }
}