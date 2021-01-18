using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroidvaniaTools
{
    //Manages everything for the mini-map camera to be in the correct room and make sure the camera is bound to that room as well
    public class MiniMap : Managers
    {
        //A reference to the current room the mini-map needs to be in
        protected GameObject currentScene;
        //A reference to the camera for the mini-map
        protected Camera miniMapCamera;
        //The minimum bounding area for the x coordinate
        protected float minCameraX;
        //The minimum bounding area for the y coordinate
        protected float minCameraY;
        //The maximum bounding area for the x coordinate
        protected float maxCameraX;
        //The maximum bounding area for the y coordinate
        protected float maxCameraY;

        protected override void Initialization()
        {
            base.Initialization();
            miniMapCamera = GetComponent<Camera>();
            currentScene = GameObject.Find(SceneManager.GetActiveScene().name);
            transform.position = currentScene.gameObject.transform.position;
            //These next four values are set in the Managers script, and this script uses these references for the mini-map camera instead of the level bounds they are used for in the CameraFollow script
            xMin = currentScene.GetComponent<WorldMap>().bounds.min.x + transform.position.x + transform.localPosition.x;
            yMin = currentScene.GetComponent<WorldMap>().bounds.min.y + transform.position.y + transform.localPosition.y;
            xMax = currentScene.GetComponent<WorldMap>().bounds.max.x + transform.position.x + transform.localPosition.x;
            yMax = currentScene.GetComponent<WorldMap>().bounds.max.y + transform.position.y + transform.localPosition.y;
            //These next four values make sure that what the camera can see is restricted within the bounding areas for that mini-map room
            minCameraX = miniMapCamera.ViewportToWorldPoint(new Vector2(0, 0)).x;
            minCameraY = miniMapCamera.ViewportToWorldPoint(new Vector2(0, 0)).y;
            maxCameraX = miniMapCamera.ViewportToWorldPoint(new Vector2(1, 1)).x;
            maxCameraY = miniMapCamera.ViewportToWorldPoint(new Vector2(1, 1)).y;
        }

        protected virtual void LateUpdate()
        {
            //Sets up the mini-map camera position to follow the Player Indicator first
            transform.position = new Vector3(playerIndicator.transform.position.x, playerIndicator.transform.position.y, -10);
            //Then ensures that the mini-map camera doesn't go outside of the bounding areas
            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, xMin - minCameraX, xMax - maxCameraX), Mathf.Clamp(transform.localPosition.y, yMin - minCameraY, yMax - maxCameraY), -10);
        }
    }
}