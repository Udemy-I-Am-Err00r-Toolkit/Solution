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
            CharacterManager.CharacterUpdate += NewCharacter;
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
            int gameFile = PlayerPrefs.GetInt("GameFile");
            //The file slot based on if it's 1, 2, or 3
            PlayerPrefs.SetInt("FileCreated" + gameFile, 1);
            //Refils the player health back to full health, as most games do when you save
            player.GetComponent<Health>().healthPoints = player.GetComponent<Health>().maxHealthPoints;
            //Makes sure the current scene is the scene that loads next to you load game
            PlayerPrefs.SetString(" " + character.gameFile + "LoadGame", SceneManager.GetActiveScene().name);
            //Makes sure the correct spawn point is fed to the LevelManager script next time you load game
            PlayerPrefs.SetInt(" " + character.gameFile + "SpawnReference", reference);
            //Makes sure the Player is facing the correct direction next time you load game
            PlayerPrefs.SetInt(" " + character.gameFile + "FacingLeft", character.isFacingLeft ? 1 : 0);
            //Sets the current health to the correct amount
            PlayerPrefs.SetInt(" " + character.gameFile + "CurrentHealth", player.GetComponent<Health>().healthPoints);
            //Makes sure the Player has the correct weapon selected when loading game
            for (int i = 0; i<character.GetComponent<CharacterManager>().characters.Length; i ++)
            {
                //Makes sure if the character is the one being played or not; this is so it can name the PlayerPref correctly with the additional "(Clone)" string within the character name
                if(levelManager.currentPlayerSelection != i)
                {
                    //Sets the game file weapon with the last weapon that character used based on the PlayerPref of that character
                    PlayerPrefs.SetInt(" " + gameFile + character.GetComponent<CharacterManager>().characters[i].name + "(Clone)" + "CurrentWeapon", PlayerPrefs.GetInt(character.GetComponent<CharacterManager>().characters[i].name + "(Clone)" + "CurrentWeapon"));
                }
                //If the current character is the one being played
                else
                {
                    //Sets the game file weapon with the weapon the character was using when saving the game
                    PlayerPrefs.SetInt(" " + gameFile + character.name + "CurrentWeapon", character.GetComponent<Character>().currentWeaponSelected);
                }
            }
            //Makes sure the FogOfWar tiles that need to be removed when loading are accurate
            levelManager.tileID = levelManager.id.ToArray();
            PlayerPrefsX.SetIntArray(" " + character.gameFile + "TilesToRemove", levelManager.tileID);
            //Makes sure the correct character is selected for the CharacterManager script
            PlayerPrefs.SetInt(" " + character.gameFile + "Character", PlayerPrefs.GetInt("Character"));
        }

        protected virtual void NewCharacter()
        {
            UpdateCharacter();
        }
    }
}