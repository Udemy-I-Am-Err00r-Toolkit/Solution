using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class PlayerIndicatorMovement : Managers
    {
        protected Vector3 previousPosition;
        protected Vector3 relativePosition;
        protected Transform origin;

        protected override void Initialization()
        {
            base.Initialization();
            origin = levelManager.playerIndicatorSpawnLocations[PlayerPrefs.GetInt(" " + character.gameFile + "SpawnReference")];
            relativePosition = player.transform.position * -.1f;
        }

        protected virtual void LateUpdate()
        {
            Vector3 currentPosition = player.transform.position;
            if(currentPosition != previousPosition)
            {
                transform.position = GetRelativePosition(origin, player.transform.position * -.1f);
                transform.position = new Vector2(Mathf.Abs(transform.position.x - relativePosition.x), -Mathf.Abs(transform.position.y - relativePosition.y));
            }
            previousPosition = currentPosition;
        }

        protected virtual Vector3 GetRelativePosition(Transform origin, Vector3 position)
        {
            Vector3 distance = position - origin.position;
            Vector3 relativeP = Vector3.zero;
            relativeP.x = Vector3.Dot(distance, origin.right.normalized);
            relativeP.y = Vector3.Dot(distance, origin.up.normalized);
            relativeP.z = Vector3.Dot(distance, origin.forward.normalized);
            return relativeP;
        }
    }
}