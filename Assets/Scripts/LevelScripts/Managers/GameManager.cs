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
        //Determines if there is an error with the starting location of the player, have the player indicator start at 0
        [HideInInspector]
        public bool playerStartDefault;
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
        }

        //Instantiates the Player into the scene; this method is called by the LevelManager script
        protected virtual void CreatePlayer(GameObject initialPlayer, Vector3 location)
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
            initialPlayer.GetComponent<Character>().InitializePlayer();

        }

        //Method that chanages character, called from the CharacterManager script located on Player
        public virtual void ChangeCharacter(GameObject currentCharacter)
        {
            GameObject oldPlayer = FindObjectOfType<Character>().gameObject;
            currentCharacter.transform.position = oldPlayer.transform.position;
            currentCharacter.transform.rotation = oldPlayer.transform.rotation;
            Instantiate(currentCharacter);
            player = currentCharacter;
            Destroy(oldPlayer);
        }

        //This method is called by other scripts that need to grab the most recent gameobject that is the player
        protected virtual void UpdateCharacter()
        {
            player = FindObjectOfType<Character>().gameObject;
        }
    }
}