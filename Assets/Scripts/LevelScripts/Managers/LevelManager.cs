using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MetroidvaniaTools
{
    //This script does a lot to manage the overall data of each level as well as ensure the Player and Player Indicator are in correct positions when a scene loads; it also manages the Fog of War
    public class LevelManager : Managers
    {
        //How big the current scene is; this bounds value will help restrict the camera with the level bounds
        public Bounds levelSize;
        //The Player game object that needs to load into the scene
        public GameObject initialPlayer;
        //Current selection on the characters list that is the character
        [HideInInspector]
        public int currentPlayerSelection;
        //The reference number that the Player and PlayerIndicator should start from based on a list of references baked into the scene
        [HideInInspector]
        public int currentStartReference;
        //A quick reference of a UI element that fades in and out between scenes
        public Image fadeScreen;

        //An array of game objects that contain the Fog Of War script
        protected FogOfWar[] fog;
        //A list of the same game objects above, this list is then converted to the above array for saving when it needs to be saved
        protected List<FogOfWar> fogTiles = new List<FogOfWar>();
        //A list of unique values every game object has that will grow as the Player discovers more of the map and removes Fog Of War tiles
        [HideInInspector]
        public List<int> id = new List<int>();
        //The same data as above, but converted into an array for saving when it needs to be saved
        public int[] tileID;

        //The location on your mini-map that the Fog Of War tiles need to spawn from to cover the mini-map
        public Transform fogSpawnLocation;
        //The master game object that contains each Fog Of War tile used; this master game object will have children game objects that will act as the individual tiles
        public GameObject fogOfWar;
        //The possible locations in the scene the Player can spawn at when the scene loads
        [SerializeField]
        protected List<Transform> availableSpawnLocations = new List<Transform>();
        //The possible locations in the mini-map the Player Indicator can spawn at when the scene loads
        public List<Transform> playerIndicatorSpawnLocations = new List<Transform>();

        //The location the Player should be at when the scene loads
        private Vector3 startingLocation;
        //The location the Player Indicator should be at when the scene loads
        private Vector3 playerIndicatorLocation;

        //Checks to see if the game is being loaded from a save so it can have more accurate data for health and such rather than from a fresh save that would have full health and such
        [HideInInspector]
        public bool loadFromSave;
        private int gameFile;

        protected virtual void Awake()
        {
            //Sets the game file up so that when saving, it saves on the correct file; when playing a built game, this value will be selected in a 'New Game' or 'Load Game' slot
            gameFile = PlayerPrefs.GetInt("GameFile");
            //Determines if the game should load from a save or not; this will help keep data consistent from when a scene simply changes as opposed to player dying or loading from the main menu
            loadFromSave = PlayerPrefs.GetInt(" " + gameFile + "LoadFromSave") == 1 ? true : false;
            //If the game is loading from a save, then it needs to setup some references based on PlayerPrefs of that game file
            if (loadFromSave)
            {
                //The reference for the Player and PlayerIndicator to start from
                currentStartReference = PlayerPrefs.GetInt(" " + gameFile + "SpawnReference");
            }
            //If the game is not loading from a save and is just changing scenes, then run the following logic
            else
            {
                //The reference for the Player and PlayerIndicator to start from
                currentStartReference = PlayerPrefs.GetInt("SpawnReference");
            }
            if (availableSpawnLocations.Count <= currentStartReference || currentStartReference < 0)
            {
                //Sets the currentStartReference to a default value
                currentStartReference = 0;
            }
            //This will set the player to the correct reference based on the PlayerPref
            startingLocation = availableSpawnLocations[currentStartReference].position;
            //This will set the player indicator to the correct reference based on the PlayerPref
            playerIndicatorLocation = playerIndicatorSpawnLocations[currentStartReference].position;
            //This will set the current character being used to the correct reference based on PlayerPref when last saved game
            if (loadFromSave)
            {
                currentPlayerSelection = PlayerPrefs.GetInt(" " + gameFile + "Character");
            }
            else
            {
                currentPlayerSelection = PlayerPrefs.GetInt("Character");
            }
            //If somehow when loading the scene the currentPlayerSelected is higher than the number of selectable players within the CharacterManager script or is a negative number, it sets it to a default of 0
            if (currentPlayerSelection >= initialPlayer.GetComponent<CharacterManager>().characters.Length || currentPlayerSelection < 0)
            {
                //Default value of the current selected playable character
                currentPlayerSelection = 0;
            }
            //Sets up which player should initialize based on the CharacterManager values for the player
            initialPlayer = initialPlayer.GetComponent<CharacterManager>().characters[currentPlayerSelection];
            //Runs a method in the GameManager script to initialize player
            CreatePlayer(initialPlayer, startingLocation);
            //Sets up the fog of war
            Instantiate(fogOfWar, fogSpawnLocation.position, Quaternion.identity);
            //Sets up array of fog tiles based on total number of fog tiles
            fog = FindObjectsOfType<FogOfWar>();
        }

        protected override void Initialization()
        {
            base.Initialization();
            //An empty array to store the number of tiles that will need to be removed
            int[] numberArray;
            //Fades in the scene
            StartCoroutine(FadeIn());
            //First grabs a reference of all the tiles within the fog array setup in the Awake method
            for (int i = 0; i < fog.Length; i++)
            {
                //Adds the fog tiles to a list so we can remove the ones that we have already found
                fogTiles.Add(fog[i]);
            }
            if (loadFromSave)
            {
                //Sets up the empty array we setup in the initialization based on the load data
                numberArray = PlayerPrefsX.GetIntArray(" " + gameFile + "TilesToRemove");
            }
            else
            {
                //Sets up the empty array we setup in the initialization based on the next scene data
                numberArray = PlayerPrefsX.GetIntArray("TilesToRemove");
            }
            //Checks which fog times setup in the empty array so it can remove them from the fogTiles list
            foreach (int number in numberArray)
            {
                id.Add(number);
                Destroy(fogTiles[number].gameObject);
            }
            CharacterManager.CharacterUpdate += NewCharacter;
            //Ensures after the level loads that it is aware the game is no longer loading from a save so when scenes change, it doesn't get confused and load the wrong data
            Invoke("CancelLoadFromSave", .1f);
        }

        //This method is called by the FogOfWar script located on each fog tile, this is an OnTriggerEnter method that checks if the PlayerIndicator has collided with that tile
        public virtual void RemoveFog(FogOfWar fogTile)
        {
            id.Add(fogTiles.IndexOf(fogTile));
            Destroy(fogTile.gameObject);
        }

        //When we load the next scene by walking through a door, it remembers everything it should so when the next scene loads it has the correct data; this also loads the next scene of course and starts the FadeOut method
        public virtual void NextScene(SceneReference scene, int spawnReference)
        {
            tileID = id.ToArray();
            PlayerPrefsX.SetIntArray("TilesToRemove", tileID);
            PlayerPrefs.SetInt("FacingLeft", character.GetComponent<Character>().isFacingLeft ? 1 : 0);
            PlayerPrefs.SetInt("SpawnReference", spawnReference);
            PlayerPrefs.SetInt("CurrentHealth", player.GetComponent<Health>().healthPoints);
            PlayerPrefs.SetInt("CurrentWeapon", character.currentWeaponSelected);
            StartCoroutine(FadeOut(scene));
        }

        //Handles lerping the screen to fade in
        protected virtual IEnumerator FadeIn()
        {
            float timeStarted = Time.time;
            float timeSinceStarted = Time.time - timeStarted;
            float percentageComplete = timeSinceStarted / .5f;
            Color currentColor = fadeScreen.color;
            while (true)
            {
                timeSinceStarted = Time.time - timeStarted;
                percentageComplete = timeSinceStarted / .5f;
                currentColor.a = Mathf.Lerp(1, 0, percentageComplete);
                fadeScreen.color = currentColor;
                if (percentageComplete >= 1)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        //Handles lerping the screen to fade out when the player falls out of bounds, this method is called by the OutOfBounds script
        public virtual IEnumerator FallFadeOut()
        {
            float timeStarted = Time.time;
            float timeSinceStarted = Time.time - timeStarted;
            float percentageComplete = timeSinceStarted / .5f;
            Color currentColor = fadeScreen.color;
            while (true)
            {
                timeSinceStarted = Time.time - timeStarted;
                percentageComplete = timeSinceStarted / .5f;
                currentColor.a = Mathf.Lerp(0, 1, percentageComplete);
                fadeScreen.color = currentColor;
                if (percentageComplete >= 1)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            StartCoroutine(FadeIn());
        }

        //Handles lerping the screen to fade out when the player goes to the next scene, at the end of this method it loads the next scene; this method is called by the NextScene method in this script
        protected virtual IEnumerator FadeOut(SceneReference scene)
        {
            float timeStarted = Time.time;
            float timeSinceStarted = Time.time - timeStarted;
            float percentageComplete = timeSinceStarted / .5f;
            Color currentColor = fadeScreen.color;
            while (true)
            {
                timeSinceStarted = Time.time - timeStarted;
                percentageComplete = timeSinceStarted / .5f;
                currentColor.a = Mathf.Lerp(0, 1, percentageComplete);
                fadeScreen.color = currentColor;
                if (percentageComplete >= 1)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            SceneManager.LoadScene(scene);
        }

        private void CancelLoadFromSave()
        {
            //At this point, loadFromSave needs to be set to false so as we go to the next scene, it manages the logic correctly
            loadFromSave = false;
            //Ensures the PlayerPrefs that manages the LoadFromSave value has the correct value
            PlayerPrefs.SetInt(" " + gameFile + "LoadFromSave", levelManager.loadFromSave ? 1 : 0);
        }

        protected virtual void NewCharacter()
        {
            UpdateCharacter();
        }

        //Shows visually the bounds for the Level
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(levelSize.center, levelSize.size);
        }
    }
}