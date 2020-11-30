using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroidvaniaTools
{
    public class MiniMap : Managers
    {
        protected GameObject currentScene;
        protected Camera miniMapCamera;
        protected float minCameraX;
        protected float minCameraY;
        protected float maxCameraX;
        protected float maxCameraY;

        protected override void Initialization()
        {
            base.Initialization();
            miniMapCamera = GetComponent<Camera>();
            currentScene = GameObject.Find(SceneManager.GetActiveScene().name);
            transform.position = currentScene.gameObject.transform.position;
            xMin = currentScene.GetComponent<WorldMap>().bounds.min.x + transform.position.x + transform.localPosition.x;
            yMin = currentScene.GetComponent<WorldMap>().bounds.min.y + transform.position.y + transform.localPosition.y;
            xMax = currentScene.GetComponent<WorldMap>().bounds.max.x + transform.position.x + transform.localPosition.x;
            yMax = currentScene.GetComponent<WorldMap>().bounds.max.y + transform.position.y + transform.localPosition.y;
            minCameraX = miniMapCamera.ViewportToWorldPoint(new Vector2(0, 0)).x;
            minCameraY = miniMapCamera.ViewportToWorldPoint(new Vector2(0, 0)).y;
            maxCameraX = miniMapCamera.ViewportToWorldPoint(new Vector2(1, 1)).x;
            maxCameraY = miniMapCamera.ViewportToWorldPoint(new Vector2(1, 1)).y;
        }

        protected virtual void LateUpdate()
        {
            transform.position = new Vector3(playerIndicator.transform.position.x, playerIndicator.transform.position.y, -10);
            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, xMin - minCameraX, xMax - maxCameraX), Mathf.Clamp(transform.localPosition.y, yMin - minCameraY, yMax - maxCameraY), -10);
        }
    }
}