using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class CameraFollow : Managers
    {
        [SerializeField]
        protected float xAdjustment;
        [SerializeField]
        protected float yAdjustment;
        [SerializeField]
        protected float zAdjustment;
        [SerializeField]
        protected float tValue;

        protected float halfCameraX;
        protected float halfCameraY;

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
            FollowPlayer();
        }

        protected virtual void FollowPlayer()
        {
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
            if (!character.isFacingLeft)
            {
                transform.position = Vector3.Lerp(new Vector3(player.transform.position.x + xAdjustment, player.transform.position.y + yAdjustment, player.transform.position.z - zAdjustment), transform.position, tValue);
            }
            else
            {
                transform.position = Vector3.Lerp(new Vector3(player.transform.position.x + -xAdjustment, player.transform.position.y + yAdjustment, player.transform.position.z - zAdjustment), transform.position, tValue);
            }
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, xMin - halfCameraX, xMax + halfCameraX), Mathf.Clamp(transform.position.y, yMin - halfCameraY, yMax + halfCameraY), -zAdjustment);
        }
    }
}