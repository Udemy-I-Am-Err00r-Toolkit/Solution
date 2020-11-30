using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroidvaniaTools
{
    public class UIManager : Managers
    {
        public SceneReference mainMenuScene;

        protected GameObject miniMap;
        protected GameObject bigMap;
        public GameObject deadScreen;
        [HideInInspector]
        public GameObject gamePausedScreen;
        [HideInInspector]
        public GameObject areYouSureScreen;

        protected float originalTimeScale;

        public bool bigMapOn;

        protected override void Initialization()
        {
            base.Initialization();
            miniMap = FindObjectOfType<MiniMapFinder>().gameObject;
            bigMap = FindObjectOfType<BigMapFinder>().gameObject;
            deadScreen = FindObjectOfType<DeadScreenFinder>().gameObject;
            gamePausedScreen = FindObjectOfType<GamePausedFinder>().gameObject;
            areYouSureScreen = FindObjectOfType<AreYouSureFinder>().gameObject;
            ManageUI();
        }

        protected virtual void Update()
        {
            if(player.GetComponent<InputManager>().BigMapPressed())
            {
                GamePaused();
            }
            if(bigMapOn)
            {
                MoveMap();
            }    
        }

        protected virtual void GamePaused()
        {
            if(!gameManager.gamePaused)
            {
                gamePausedScreen.SetActive(true);
                miniMap.SetActive(false);
                gameManager.gamePaused = true;
                originalTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                gamePausedScreen.SetActive(false);
                areYouSureScreen.SetActive(false);
                bigMapOn = false;
                miniMap.SetActive(true);
                bigMap.SetActive(false);
                gameManager.gamePaused = false;
                Time.timeScale = originalTimeScale;
            }
        }

        public virtual void ReturnToGame()
        {
            miniMap.SetActive(true);
            gamePausedScreen.SetActive(false);
            gameManager.gamePaused = false;
            Time.timeScale = originalTimeScale;
        }

        public virtual void BigMapOn()
        {
            gamePausedScreen.SetActive(false);
            bigMap.SetActive(true);
            bigMapOn = true;
        }

        public virtual void QuitGame()
        {
            areYouSureScreen.SetActive(true);
        }

        public virtual void ReturnToMainMenu()
        {
            bigMap.SetActive(false);
            bigMapOn = false;
            areYouSureScreen.SetActive(false);
            gamePausedScreen.SetActive(true);
        }

        public virtual void ManageUI()
        {
            miniMap.SetActive(true);
            bigMap.SetActive(false);
            gamePausedScreen.SetActive(false);
            areYouSureScreen.SetActive(false);
            deadScreen.SetActive(false);
        }

        public virtual void SureToQuit()
        {
            PlayerPrefs.SetString(" " + character.gameFile + "LoadGame", SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(mainMenuScene);
            Time.timeScale = originalTimeScale;
            gameManager.gamePaused = false;
        }

        protected virtual void MoveMap()
        {
            float vertical = new float();
            float horizontal = new float();
            if(input.UpHeld())
            {
                vertical = .25f;
            }
            if(input.DownHeld())
            {
                vertical = -.25f;
            }
            if(input.LeftHeld())
            {
                horizontal = -.25f;
            }
            if(input.RightHeld())
            {
                horizontal = .25f;
            }
            Vector3 currentPosition = bigMapCamera.transform.position;
            bigMapCamera.transform.position = new Vector3(currentPosition.x + horizontal, currentPosition.y + vertical, -10);
        }
    }
}