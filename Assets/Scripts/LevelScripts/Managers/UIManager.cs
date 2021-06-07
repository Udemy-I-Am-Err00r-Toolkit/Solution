using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroidvaniaTools
{
    //This script handles a lot of the toggling between the different UI screens, and ensure the correct screen is front and center depending on what buttons are pressed and how the UI is interacted with
    public class UIManager : Managers
    {
        //A quick reference to the scenet that acts as the title screen
        public SceneReference mainMenuScene;
        //A quick reference to the UI for the mini-map at the top right of the screen
        protected GameObject miniMap;
        //A quick reference to the UI for the bigMap that pops up when game is paused
        protected GameObject bigMap;
        //A quick reference to the UI for the game over type screen
        public GameObject deadScreen;
        //A quick reference to the screen that pops up when game is paused
        [HideInInspector]
        public GameObject gamePausedScreen;
        //A quick reference to a screen that pops up when you try to quit the game
        [HideInInspector]
        public GameObject areYouSureScreen;
        //When the game is paused, time is paused as well, this is a quick reference to whatever the time scale was previous to pausing
        protected float originalTimeScale;
        //A quick reference on if the bigMap UI is front and center
        [HideInInspector]
        public bool bigMapOn;

        //This will setup all the initial references to the different UI screens, and then run the ManageUI method
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

        //Checks input to pause game; the input is labeled as BigMapPressed, and that is because when filming this course I decided to use this key input that was already established to pause the game
        protected virtual void Update()
        {
            if (player.GetComponent<InputManager>().BigMapPressed())
            {
                GamePaused();
            }
            if (bigMapOn)
            {
                MoveMap();
            }
        }

        //Sets up the correct screens and stops time when the game is paused; it toggles betwen the paused on not paused depending on the toggle and this method just toggles between two different states
        protected virtual void GamePaused()
        {
            if (!gameManager.gamePaused)
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

        //If the ReturnToGame button is pressed within the GamePaused screen, sets up the correct UI elements to start the game back
        public virtual void ReturnToGame()
        {
            miniMap.SetActive(true);
            gamePausedScreen.SetActive(false);
            gameManager.gamePaused = false;
            Time.timeScale = originalTimeScale;
        }

        //If the Map button is pressed within the GamePaused screen, sets up the correct UI elements to show the BigMap
        public virtual void BigMapOn()
        {
            gamePausedScreen.SetActive(false);
            bigMap.SetActive(true);
            bigMapOn = true;
        }

        //If the Quit Game button is pressed within the GamePaused screen, first asks if you're sure you want to quit by activating that UI
        public virtual void QuitGame()
        {
            areYouSureScreen.SetActive(true);
        }

        //If the Return To Main Menu button is pressed within the Are You Sure or Big Map screens, goes backwards in the UI to the main Paused screen
        public virtual void ReturnToMainMenu()
        {
            bigMap.SetActive(false);
            bigMapOn = false;
            areYouSureScreen.SetActive(false);
            gamePausedScreen.SetActive(true);
        }

        //If the game is no longer paused and not dead, sets up the standard UI that should appear when playing game
        public virtual void ManageUI()
        {
            miniMap.SetActive(true);
            bigMap.SetActive(false);
            gamePausedScreen.SetActive(false);
            areYouSureScreen.SetActive(false);
            deadScreen.SetActive(false);
        }

        //If you want to quit game, it saves the player's position within that scene, and goes back to the main menu screen
        public virtual void SureToQuit()
        {
            SceneManager.LoadScene(mainMenuScene);
            Time.timeScale = originalTimeScale;
            gameManager.gamePaused = false;
        }

        //Allows the UI to move around while looking at the Big Map
        protected virtual void MoveMap()
        {
            float vertical = new float();
            float horizontal = new float();
            if (input.UpHeld())
            {
                vertical = .25f;
            }
            if (input.DownHeld())
            {
                vertical = -.25f;
            }
            if (input.LeftHeld())
            {
                horizontal = -.25f;
            }
            if (input.RightHeld())
            {
                horizontal = .25f;
            }
            Vector3 currentPosition = bigMapCamera.transform.position;
            bigMapCamera.transform.position = new Vector3(currentPosition.x + horizontal, currentPosition.y + vertical, -10);
        }
    }
}