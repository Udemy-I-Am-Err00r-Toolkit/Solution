using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script will handle moving the player indicator accuratley around the mini-map based on the player movement
    public class PlayerIndicatorMovement : Managers
    {
        //Position the indicator was last in
        protected Vector3 previousPosition;
        //The position on the mini-map that the Player Indicator would be located in relation to the Player; this value is going to be scaled down to 1/10th what the Player moves around in
        protected Vector3 relativePosition;
        //The starting point the Player Indicator should start in when scene loads
        protected Transform origin;

        protected override void Initialization()
        {
            base.Initialization();
            //Sets up the correct game file in case the game is being loaded from a save
            //Adds the NewCharacter method to the event for the CharacterManager delegate
            CharacterManager.CharacterUpdate += NewCharacter;
            //Checks to see if the game is being loaded from save based on the LevelManager script
            if (gameManager.playerStartDefault)
            {
                //Sets the original placement for the Player Indicator
                origin = levelManager.playerIndicatorSpawnLocations[0];
            }
            //If the scene is not being loaded from a save game file
            else
            {
                int gameFile = PlayerPrefs.GetInt("GameFile");
                //Sets the original placement for the Player Indicator
                origin = levelManager.playerIndicatorSpawnLocations[PlayerPrefs.GetInt(" " + gameFile + "SpawnReference")];
            }
            relativePosition = player.transform.position * -.1f;
        }

        protected virtual void LateUpdate()
        {
            if (player != null)
            {
                //Sets up the current position based on origin
                Vector3 currentPosition = player.transform.position;
                //Checks to see if the current position is not the previous position; it knows this because at the bottom of this method it sets the previous position, so if the Player Indicator moves, on the next frame tick it will check to see if those positions still match
                if (currentPosition != previousPosition)
                {
                    //If the previous position does not equal the current position, it first gets the relative position by doing a bunch of complicated math, then multiplying that value the 1/10th value the mini-map is scaled to
                    transform.position = GetRelativePosition(origin, player.transform.position * -.1f);
                    //After getting the relative position, it then sets the position to it's cooresponding position in an x positive area, y negative area; this math is required for the mini-map to display accurate placement of Player Indicator
                    transform.position = new Vector2(Mathf.Abs(transform.position.x - relativePosition.x), -Mathf.Abs(transform.position.y - relativePosition.y));
                }
                //It will always set the previous position to the current position every tick to ensure if on the next tick the two are still the same
                previousPosition = currentPosition;
            }
        }

        //Very complicated math that I found online to help solve for accurate placement of the Player Indicator based on the Player position and then translate to a smaller scale; quite frankly, I couldn't tell you how this method technically works, but it work
        protected virtual Vector3 GetRelativePosition(Transform origin, Vector3 position)
        {
            Vector3 distance = position - origin.position;
            Vector3 relativeP = Vector3.zero;
            relativeP.x = Vector3.Dot(distance, origin.right.normalized);
            relativeP.y = Vector3.Dot(distance, origin.up.normalized);
            relativeP.z = Vector3.Dot(distance, origin.forward.normalized);
            return relativeP;
        }

        //Method found on GameManager script to set the new player
        protected virtual void NewCharacter()
        {
            UpdateCharacter();
        }
    }
}