using System.Collections;
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

        public List<AbilityItem> abilitiesToClear = new List<AbilityItem>();

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
            //Makes sure the player spawns at the 0 point on the playerSpawn and playerIndicatorSpawn within the LevelManager script; this value is more specific so that if you quit the game right after making new game and before you reach a save point, it doesn't cause any issues when trying to load that game
            PlayerPrefs.SetInt(" " + slot + "SaveSpawnReference", 0);
            //Same as above value, but just for the LevelManager script
            PlayerPrefs.SetInt(" " + slot + "SpawnReference", 0);
            //Sets the health to 100 because most games start you off with full health, you can toggle this any way you want if you don't want the player to start with 100 health
            PlayerPrefs.SetInt(" " + slot + "CurrentHealth", 100);
            //Sets up the string reference for the scene it needs to load
            //PlayerPrefs.SetString("LoadGame", newGameScene);
            ClearAbilities(slot);
            //Loads the first scene for the game
            SceneManager.LoadScene(newGameScene);
        }

        //Loads a game from a previous save
        public virtual void LoadGame(int slot)
        {
            //Makes sure that the PlayerPrefs knows the game is loading from a save, so the LevelManager script can perform additional logic it needs to for that load
            bool loadFromSave = true;
            //Makes sure teh current game file is the correct one for PlayerPrefs
            PlayerPrefs.SetInt("GameFile", slot);
            //Sets the PlayerPrefs value to whatever the loadFromSave value is
            PlayerPrefs.SetInt(" " + slot + "LoadFromSave", loadFromSave ? 1 : 0);
            //Loads the correct scene for that file based on whatever scene the Player was in when last saved
            SceneManager.LoadScene(PlayerPrefs.GetString(" " + slot + "LoadGame"));
        }

        public virtual void ClearAbilities(int slot)
        {
            for (int i = 0; i < abilitiesToClear.Count; i++)
            {
                PlayerPrefs.SetInt(" " + slot + abilitiesToClear[i].itemName, 0);
            }
        }
    }
}