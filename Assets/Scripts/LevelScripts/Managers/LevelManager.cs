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
        //A quick reference of a UI element that fades in and out between scenes
        public Image fadeScreen;
        //A quick reference of a UI element that fades in when Player dies
        public Image deadScreen;

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
        public List<Transform> availableSpawnLocations = new List<Transform>();
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
            //Sets up the game file based on PlayerPrefs; this is set through the TitleScreen script
            gameFile = PlayerPrefs.GetInt("GameFile");
            //Sets up the loadFromSave bool based on the PlayerPrefs; this is set through the TitleScreen script
            loadFromSave = PlayerPrefs.GetInt(" " + gameFile + "LoadFromSave") == 1 ? true : false;
            //Ensures that if the scene is loading from a save, it sets the Player and Player Indicator at whatever spawn reference they should be at
            if (loadFromSave)
            {
                startingLocation = availableSpawnLocations[PlayerPrefs.GetInt(" " + gameFile + "SpawnReference")].position;
                playerIndicatorLocation = playerIndicatorSpawnLocations[PlayerPrefs.GetInt(" " + gameFile + "SpawnReference")].position;
                initialPlayer = initialPlayer.GetComponent<CharacterManager>().characters[PlayerPrefs.GetInt(" " + gameFile + "Character")];
                if (availableSpawnLocations.Count <= PlayerPrefs.GetInt(" " + gameFile + "SpawnReference"))
                {
                    startingLocation = availableSpawnLocations[0].position;
                    playerIndicatorLocation = playerIndicatorSpawnLocations[0].position;
                }
            }
            else
            {
                //If the scene is not being loaded from a save, it grabs the starting location that was set by the NextScene script that runs when you enter a door
                startingLocation = availableSpawnLocations[PlayerPrefs.GetInt("SpawnReference")].position;
                playerIndicatorLocation = playerIndicatorSpawnLocations[PlayerPrefs.GetInt("SpawnReference")].position;
                initialPlayer = initialPlayer.GetComponent<CharacterManager>().characters[PlayerPrefs.GetInt("Character")];
                if (availableSpawnLocations.Count <= PlayerPrefs.GetInt("SpawnReference"))
                {
                    startingLocation = availableSpawnLocations[0].position;
                    playerIndicatorLocation = playerIndicatorSpawnLocations[0].position;
                }
            }
            //If for some reason the SpawnReference value is higher than it is allowed to be, rather than crashing the game, it automatically sets it at 0 so the game can still be tested; this would be an error on your part that you can fix when not playtesting
            //Creates the Player at the correct starting location
            CreatePlayer(initialPlayer, startingLocation);
            //Adds the Fog Of War in the scene at the correct position
            Instantiate(fogOfWar, fogSpawnLocation.position, Quaternion.identity);
            //Sets up the Fog array
            fog = FindObjectsOfType<FogOfWar>();
        }

        protected override void Initialization()
        {
            base.Initialization();
            CharacterManager.CharacterUpdate += NewCharacter;
            //Makes sure gameFile is still correct
            int[] numberArray;
            //Moves the Player Indicator to the correct position
            playerIndicator.transform.position = playerIndicatorLocation;
            //Fades the screen in as the camera and everything moves to get setup, this helps make sure everything is in the correct place when scene loads
            StartCoroutine(FadeIn());
            //Manages all the different fog tiles to find out which need to be removed based on the PlayerPrefsX script
            for (int i = 0; i < fog.Length; i++)
            {
                fogTiles.Add(fog[i]);
            }
            if (loadFromSave)
            {
                //Based on previous save data that happens everytime you run into a Fog Of War tile, the correct tiles are removed at Start
                numberArray = PlayerPrefsX.GetIntArray(" " + gameFile + "TilesToRemove");
            }
            else
            {
                numberArray = PlayerPrefsX.GetIntArray("TilesToRemove");
            }
            //The actual foreach loop and method that destroys the Fog tiles already found
            foreach (int number in numberArray)
            {
                id.Add(number);
                Destroy(fogTiles[number].gameObject);
            }
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
            PlayerPrefs.SetInt("FacingLeft", character.isFacingLeft ? 1 : 0);
            PlayerPrefs.SetInt("SpawnReference", spawnReference);
            PlayerPrefs.SetInt("CurrentHealth", player.GetComponent<PlayerHealth>().healthPoints);
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

        protected virtual void NewCharacter()
        {
            UpdateCharacter();
        }

        private void CancelLoadFromSave()
        {
            //At this point, loadFromSave needs to be set to false so as we go to the next scene, it manages the logic correctly
            loadFromSave = false;
            //Ensures the PlayerPrefs that manages the LoadFromSave value has the correct value
            PlayerPrefs.SetInt(" " + gameFile + "LoadFromSave", levelManager.loadFromSave ? 1 : 0);
        }

        //Shows visually the bounds for the Level
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(levelSize.center, levelSize.size);
        }
    }
}