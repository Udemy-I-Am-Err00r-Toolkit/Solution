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
            //Makes sure the current scene is the scene that loads next to you load game
            PlayerPrefs.SetString(" " + character.gameFile + "LoadGame", SceneManager.GetActiveScene().name);
            //Makes sure the correct spawn point is fed to the LevelManager script next time you load game
            PlayerPrefs.SetInt(" " + character.gameFile + "SaveSpawnReference", reference);
            //Makes sure the Player is facing the correct direction next time you load game
            PlayerPrefs.SetInt(" " + character.gameFile + "FacingLeft", character.isFacingLeft ? 1 : 0);
            //Makes sure the FogOfWar tiles that need to be removed when loading are accurate
            PlayerPrefsX.SetIntArray(" " + character.gameFile + "TilesToRemove", levelManager.tileID);
            //Refils the player health back to full health, as most games do when you save
            player.GetComponent<Health>().healthPoints = player.GetComponent<Health>().maxHealthPoints;
        }
    }
}