using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroidvaniaTools
{
    public class SaveGame : Managers
    {
        [SerializeField]
        protected int reference;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject == player)
            {
                Save();
            }
        }

        protected virtual void Save()
        {
            PlayerPrefs.SetString(" " + character.gameFile + "LoadGame", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetInt(" " + character.gameFile + "SaveSpawnReference", reference);
            PlayerPrefs.SetInt(" " + character.gameFile + "FacingLeft", character.isFacingLeft ? 1 : 0);
            PlayerPrefsX.SetIntArray(" " + character.gameFile + "TilesToRemove", levelManager.tileID);
            player.GetComponent<Health>().healthPoints = player.GetComponent<Health>().maxHealthPoints;
        }
    }
}