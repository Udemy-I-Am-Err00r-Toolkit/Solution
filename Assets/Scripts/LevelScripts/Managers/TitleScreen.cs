﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroidvaniaTools
{
    //This script mainly manages the different UI elements on the Main Title scrren, most of the logic here switches between the different options
    public class TitleScreen : MonoBehaviour
    {
        //A quick reference to what scene needs to load when new game is selected
        public SceneReference newGameScene;
        //A quick reference to the UI that contains the main menu screen
        public GameObject mainMenu;
        //A quick reference to the UI that contains the New Game menu screen
        public GameObject newGameMenu;
        //A quick reference to the UI that contains the Load Game menu screen
        public GameObject loadGameMenu;
        //List that needs to be setup in the Unity Editor with all the discoverable abilities loaded; this does nothing if not setup within the Editor
        public List<AbilityItem> abilitiesToClear = new List<AbilityItem>();
        //
        public List<GameObject> playersToStartWith = new List<GameObject>();

        private void Start()
        {
            MainMenu();
        }

        //Makes sure the correct UI is displyed when the Main Menu is selected
        public virtual void MainMenu()
        {
            mainMenu.SetActive(true);
            loadGameMenu.SetActive(false);
            newGameMenu.SetActive(false);
        }
        //Makes sure the correct UI is displyed when the New Game Menu is selected
        public virtual void NewGameMenu()
        {
            mainMenu.SetActive(false);
            loadGameMenu.SetActive(false);
            newGameMenu.SetActive(true);
        }
        //Makes sure the correct UI is displyed when the Load Game Menu is selected
        public virtual void LoadGameMenu()
        {
            mainMenu.SetActive(false);
            loadGameMenu.SetActive(true);
            newGameMenu.SetActive(false);
        }

        //Creates a new GameFile slot to manage this data seperate from the other game files you would create in your game
        public virtual void NewGame(int slot)
        {
            //The file slot based on if it's 1, 2, or 3
            PlayerPrefs.SetInt("GameFile", slot);
            //Same as above value, but just for the LevelManager script
            PlayerPrefs.SetInt(" " + slot + "SpawnReference", 0);            
            //Sets up the string reference for the scene it needs to load
            PlayerPrefs.SetString(" " + slot + "LoadGame", newGameScene);
            //Sets the PlayerPrefs LoadFromSave value to true in case player dies or quits before first checkpoint
            PlayerPrefs.SetInt(" " + slot + "LoadFromSave", 1);
            //Sets the player to face right when starting new game; if you want player to face left, have the value equal 1 instead of 0
            PlayerPrefs.SetInt(" " + slot + "FacingLeft", 0);
            //Sets the current weapon iteration to 0 as the first weapon to load into the game
            foreach(GameObject character in playersToStartWith)
            {
                PlayerPrefs.SetInt(" " + slot + character.name + "(Clone)" + "CurrentWeapon", 0);
                PlayerPrefs.SetInt(character.name + "(Clone)" + "CurrentWeapon", 0);
                PlayerPrefs.SetInt(" " + slot + character.name + "(Clone)" + "CurrentHealth", character.GetComponent<Health>().maxHealthPoints);
                PlayerPrefs.SetInt(character.name + "(Clone)" + "CurrentHealth", character.GetComponent<Health>().maxHealthPoints);
            }
            //Makes sure the correct Character is selected for new game slot
            PlayerPrefs.SetInt(" " + slot + "Character", 0);
            //Makes sure the correct Character is selected for scene change slot
            PlayerPrefs.SetInt("Character", 0);
            //Clears the player abilities for this slot
            ClearAbilities(slot);
            //Creats a new FogOfWar with all the tiles
            int[] newFog = new int[0];
            //Resests the PlayerPrefsX value of the array for which tiles need to be removed
            PlayerPrefsX.SetIntArray(" " + slot + "TilesToRemove", newFog);
            //Loads the first scene for the game
            SceneManager.LoadScene(newGameScene);
        }

        //Loads a game from a previous save
        public virtual void LoadGame(int slot)
        {
            PlayerPrefs.SetInt("GameFile", slot);
            //Makes sure that there is an actual game file saved for that slot already, if not it runs the if statement
            if (PlayerPrefs.GetInt("FileCreated" + slot) == 0)
            {
                //Creates a new game so the game doesn't crash, might be a good idea to have a UI screen pop-up to remind you there is no slot
                NewGame(slot);
                return;
            }
            //if the above if statement is not true, it loads the game based on slot values
            else
            {
                //Makes sure teh current game file is the correct one for PlayerPrefs
                PlayerPrefs.SetInt("GameFile", slot);
                //Sets the PlayerPrefs value to whatever the loadFromSave value is
                PlayerPrefs.SetInt(" " + slot + "LoadFromSave", 1);
                //Loads the correct scene for that file based on whatever scene the Player was in when last saved
                SceneManager.LoadScene(PlayerPrefs.GetString(" " + slot + "LoadGame"));
            }
        }

        //Clears out the abiliites the player may have found for this slot
        public virtual void ClearAbilities(int slot)
        {
            //Checks the abilites that are loaded into the TitleScreen component within the Unity editor
            for (int i = 0; i < abilitiesToClear.Count; i++)
            {
                //Makes sure that slot file sets the bool from found, to not found
                PlayerPrefs.SetInt(" " + slot + abilitiesToClear[i].itemName, 0);
            }
        }
    }
}