using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroidvaniaTools
{
    //This script sets up PlayerPrefs so that if the game is loaded, it has all the correct information needed to load that game again after quitting or even starting a new game from a different slot
    public class SaveGame : Managers
    {
        //This variable needs to represent which spawn point in the scene the Player should spawn from; this is validated through the LevelManager component in availableSpawnLocations and playerIndicatorSpawnLocations
        [SerializeField]
        protected int reference;

        protected override void Initialization()
        {
            base.Initialization();
        }

        //If you enter the trigger collider of a save point, it runs the save method
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == player)
            {
                Save();
            }
        }

        protected virtual void Save()
        {
            //The file slot based on if it's 1, 2, or 3
            int gameFile = PlayerPrefs.GetInt("GameFile");
            //Sets a value for the FileCreated PlayerPref that allows to load from save instead of starting a new game; this value gets created the very first time the game is saved
            PlayerPrefs.SetInt("FileCreated" + gameFile, 1);
            //Makes sure the current scene is the scene that loads next to you load game
            PlayerPrefs.SetString(" " + character.gameFile + "LoadGame", SceneManager.GetActiveScene().name);
            //Makes sure the correct spawn point is fed to the LevelManager script next time you load game
            PlayerPrefs.SetInt(" " + character.gameFile + "SpawnReference", reference);
            //Makes sure the Player is facing the correct direction next time you load game
            PlayerPrefs.SetInt(" " + character.gameFile + "FacingLeft", character.isFacingLeft ? 1 : 0);
            //Resets all the characters health to maxhealth and saves the weapons and health to a PlayerPref
            for (int i = 0; i < character.GetComponent<CharacterManager>().characters.Length; i++)
            {
                //Creates a temporary GameObject variable for convenience naming
                GameObject player = character.GetComponent<CharacterManager>().characters[i];
                player.GetComponent<Health>().healthPoints = player.GetComponent<Health>().maxHealthPoints;
                //Checks if the iteration value is not the current character
                if (levelManager.currentPlayerSelection != i)
                {
                    //Sets the current weapon for this character
                    PlayerPrefs.SetInt(" " + gameFile + player.name + "(Clone)" + "CurrentWeapon", PlayerPrefs.GetInt(player.name + "(Clone)" + "CurrentWeapon"));
                    //Sets the current health for this character back to max health
                    PlayerPrefs.SetInt(" " + gameFile + player.name + "(Clone)" + "CurrentHealth", player.GetComponent<Health>().maxHealthPoints);
                    //Sets the current health for this character back to max health
                    PlayerPrefs.SetInt(player.name + "(Clone)" + "CurrentHealth", player.GetComponent<Health>().maxHealthPoints);
                }
                //Checks if the iteration value is the current character
                else
                {
                    //Sets the current weapon for this character
                    PlayerPrefs.SetInt(" " + gameFile + player.name + "CurrentWeapon", PlayerPrefs.GetInt(player.name + "CurrentWeapon"));
                    //Sets the current health for this character to max health
                    PlayerPrefs.SetInt(" " + gameFile + player.name + "CurrentHealth", player.GetComponent<Health>().maxHealthPoints);
                    //Sets the current health for this character to max health
                    PlayerPrefs.SetInt(player.name + "CurrentHealth", player.GetComponent<Health>().maxHealthPoints);
                }
            }
            //Makes sure the FogOfWar tiles that need to be removed when loading are accurate
            levelManager.tileID = levelManager.id.ToArray();
            PlayerPrefsX.SetIntArray(" " + character.gameFile + "TilesToRemove", levelManager.tileID);
            //Makes sure the correct character is selected for the CharacterManager script
            PlayerPrefs.SetInt(" " + character.gameFile + "Character", PlayerPrefs.GetInt("Character"));
        }
    }
}