using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script makes sure what the camera displays is what is appropriate based on Player direction and if they are falling or jumping, ensures the camera is always looking ahead of the Player
    public class CameraFollow : Managers
    {
        //How much in front of the Player horizontally based on direction you want the camera to view
        [SerializeField]
        protected float xAdjustment;
        //How much above or below the player you want the camera to view when jumping or falling
        [SerializeField]
        protected float yAdjustment;
        //This value should probably be hard coded as 10, but maybe you want a different z value and be able to control that value; in this course we set this value to 10
        [SerializeField]
        protected float zAdjustment;
        //How much the camera should be in between when the player is moving
        [SerializeField]
        protected float tValue;

        //These two values help clamp the camera to positions that prevent it from going outside of the LevelBounds
        protected float halfCameraX;
        protected float halfCameraY;

        //These next two values help adjust the camera vertically when going up or down
        private float originalYAdjustment;
        private bool falling;

        protected override void Initialization()
        {
            base.Initialization();
            originalYAdjustment = yAdjustment;
            halfCameraX = GetComponent<Camera>().ViewportToWorldPoint(new Vector2(0, 0)).x;
            halfCameraY = GetComponent<Camera>().ViewportToWorldPoint(new Vector2(0, 0)).y;
            transform.position = player.transform.position;
        }

        protected virtual void FixedUpdate()
        {
            if (player != null)
            {
                FollowPlayer();
            }
        }

        //The values in here can be played with, and maybe you would want to make seperate values as variables for your game; most of this method is checking to see if certain criteria is met for the camera position to be moved based on vertical and horizontal speeds
        protected virtual void FollowPlayer()
        {
            //The next 4 if statements handle the vertical movement of the camera; you can play with these values as you want to get a more precise camera follow
            if (character.isJumping)
            {
                float newAdjustment = originalYAdjustment;
                newAdjustment += 4;
                yAdjustment = newAdjustment;
            }
            if (!character.isJumping && !character.Falling(0))
            {
                yAdjustment = originalYAdjustment;
            }
            if (character.Falling(-6) && !falling)
            {
                falling = true;
                yAdjustment *= -1;
            }
            if (!character.Falling(0) && falling)
            {
                falling = false;
                yAdjustment *= -1;
            }
            //This if and else statment move the camera horizontally based on where the Player is facing
            if (!character.isFacingLeft)
            {
                transform.position = Vector3.Lerp(new Vector3(player.transform.position.x + xAdjustment, player.transform.position.y + yAdjustment, player.transform.position.z - zAdjustment), transform.position, tValue);
            }
            else
            {
                transform.position = Vector3.Lerp(new Vector3(player.transform.position.x + -xAdjustment, player.transform.position.y + yAdjustment, player.transform.position.z - zAdjustment), transform.position, tValue);
            }
            //Ensures the camera is clamped inside of the current scene bounds based on the LevelManager bounds
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, xMin - halfCameraX, xMax + halfCameraX), Mathf.Clamp(transform.position.y, yMin - halfCameraY, yMax + halfCameraY), -zAdjustment);
        }
    }
}