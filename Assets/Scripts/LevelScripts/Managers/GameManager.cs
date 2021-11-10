using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MetroidvaniaTools
{
    //This script is kind of like the Character script, but for the different scenes and interactable objects in those scenes; it houses a lot of shared data for easy access by things that are not the Player
    public class GameManager : MonoBehaviour
    {
        //The next four variables handle the bounds for the camera to move around based on the current scene and the LevelManager script
        [HideInInspector]
        public float xMin;
        [HideInInspector]
        public float xMax;
        [HideInInspector]
        public float yMin;
        [HideInInspector]
        public float yMax;
        //Manages if the game is paused so other scripts can receive that information
        [HideInInspector]
        public bool gamePaused;
        protected GameObject player;
        protected GameObject playerIndicator;
        protected GameObject bigMapCamera;
        protected Character character;
        protected LevelManager levelManager;
        protected InputManager input;
        protected UIManager uiManager;

        // Start is called before the first frame update
        void Start()
        {
            Initialization();
        }

        protected virtual void Initialization()
        {
            player = FindObjectOfType<Character>().gameObject;
            character = player.GetComponent<Character>();
            levelManager = FindObjectOfType<LevelManager>();
            input = player.GetComponent<InputManager>();
            playerIndicator = FindObjectOfType<PlayerBlip>().gameObject;
            uiManager = FindObjectOfType<UIManager>();
            bigMapCamera = FindObjectOfType<BigMap>().gameObject;
            //These are the values setup in each different scene by the LevelManager script based on the bounds
            xMin = levelManager.levelSize.min.x;
            xMax = levelManager.levelSize.max.x;
            yMin = levelManager.levelSize.min.y;
            yMax = levelManager.levelSize.max.y;
            CharacterManager.CharacterUpdate += UpdateCharacter;
        }

        //Instantiates the Player into the scene; this method is called by the LevelManager script
        protected virtual void CreatePlayer(GameObject initialPlayer, Vector3 location, int characterSelected)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length > 0)
            {
                foreach (GameObject obj in players)
                {
                    Destroy(obj);
                }
            }
            Instantiate(initialPlayer, new Vector3(location.x, location.y, 0), Quaternion.identity);
            initialPlayer.GetComponent<Character>().InitializePlayer(characterSelected);

        }

        //Method that chanages character, called from the CharacterManager script located on Player
        public virtual void ChangeCharacter(GameObject currentCharacter)
        {
            //Gets a reference of the old player from the scene
            GameObject oldCharacter = FindObjectOfType<Character>().gameObject;
            //Spawns in the new player at the position the old player
            Instantiate(currentCharacter, oldCharacter.transform.position, Quaternion.identity);
            //Handles making sure the player is facing the correct direction
            PlayerPrefs.SetInt("FacingLeft", character.isFacingLeft ? 1 : 0);
            //Initializes the new player and sets up proper character properties
            currentCharacter.GetComponent<Character>().InitializePlayer(levelManager.currentPlayerSelection);
            //Gets rid of old player
            Destroy(oldCharacter);
        }

        //This method is called by other scripts that need to grab the most recent gameobject that is the player
        public virtual void UpdateCharacter()
        {
            player = FindObjectOfType<Character>().gameObject;
            character = player.GetComponent<Character>();
        }
    }
}