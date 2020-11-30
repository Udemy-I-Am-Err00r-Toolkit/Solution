using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MetroidvaniaTools
{
    public class LevelManager : Managers
    {
        public Bounds levelSize;
        public GameObject initialPlayer;
        public Image fadeScreen;
        public Image deadScreen;

        protected FogOfWar[] fog;
        protected List<FogOfWar> fogTiles = new List<FogOfWar>();
        protected List<int> id = new List<int>();
        public int[] tileID;

        public Transform fogSpawnLocation;
        public GameObject fogOfWar;
        public List<Transform> availableSpawnLocations = new List<Transform>();
        public List<Transform> playerIndicatorSpawnLocations = new List<Transform>();

        private Vector3 startingLocation;
        private Vector3 playerIndicatorLocation;

        [HideInInspector]
        public bool loadFromSave;

        protected virtual void Awake()
        {
            int gameFile = PlayerPrefs.GetInt("GameFile");
            loadFromSave = PlayerPrefs.GetInt(" " + gameFile + "LoadFromSave") == 1 ? true : false;
            if(loadFromSave)
            {
                startingLocation = availableSpawnLocations[PlayerPrefs.GetInt(" " + gameFile + "SaveSpawnReference")].position; 
                playerIndicatorLocation = playerIndicatorSpawnLocations[PlayerPrefs.GetInt(" " + gameFile + "SaveSpawnReference")].position; 
            }
            if (availableSpawnLocations.Count <= PlayerPrefs.GetInt(" " + gameFile + "SpawnReference"))
            {
                startingLocation = availableSpawnLocations[0].position;
                playerIndicatorLocation = playerIndicatorSpawnLocations[0].position;
            }
            else
            {
                if (!loadFromSave)
                {
                    startingLocation = availableSpawnLocations[PlayerPrefs.GetInt("SpawnReference")].position;
                    playerIndicatorLocation = playerIndicatorSpawnLocations[PlayerPrefs.GetInt("SpawnReference")].position;
                }
                CreatePlayer(initialPlayer, startingLocation);
                Instantiate(fogOfWar, fogSpawnLocation.position, Quaternion.identity);
                fog = FindObjectsOfType<FogOfWar>();
            }
        }

        protected override void Initialization()
        {
            base.Initialization();
            int gameFile = PlayerPrefs.GetInt("GameFile");
            loadFromSave = false;
            PlayerPrefs.SetInt(" " + gameFile + "LoadFromSave", levelManager.loadFromSave ? 1 : 0);
            playerIndicator.transform.position = playerIndicatorLocation;
            StartCoroutine(FadeIn());
            for(int i = 0; i < fog.Length; i++)
            {
                fogTiles.Add(fog[i]);
            }
            int[] numberArray = PlayerPrefsX.GetIntArray(" " + gameFile + "TilesToRemove");
            foreach(int number in numberArray)
            {
                id.Add(number);
                Destroy(fogTiles[number].gameObject);
            }
        }

        public virtual void RemoveFog(FogOfWar fogTile)
        {
            id.Add(fogTiles.IndexOf(fogTile));
            Destroy(fogTile.gameObject);
        }

        protected virtual void OnDisable()
        {
            tileID = id.ToArray();
            PlayerPrefsX.SetIntArray(" " + character.gameFile + "TilesToRemove", tileID);
            PlayerPrefs.SetInt(" " + character.gameFile + "FacingLeft", character.isFacingLeft ? 1 : 0);
            PlayerPrefs.SetString(" " + character.gameFile + "LoadGame", SceneManager.GetActiveScene().name);
        }

        public virtual void NextScene(SceneReference scene, int spawnReference)
        {
            tileID = id.ToArray();
            PlayerPrefsX.SetIntArray(" " + character.gameFile + "TilesToRemove", tileID);
            PlayerPrefs.SetInt(" " + character.gameFile + "FacingLeft", character.isFacingLeft ? 1 : 0);
            PlayerPrefs.SetInt(" " + character.gameFile + "SpawnReference", spawnReference);
            PlayerPrefs.SetInt(" " + character.gameFile + "CurrentHealth", player.GetComponent<PlayerHealth>().healthPoints);
            StartCoroutine(FadeOut(scene));
        }

        protected virtual IEnumerator FadeIn()
        {
            float timeStarted = Time.time;
            float timeSinceStarted = Time.time - timeStarted;
            float percentageComplete = timeSinceStarted / .5f;
            Color currentColor = fadeScreen.color;
            while(true)
            {
                timeSinceStarted = Time.time - timeStarted;
                percentageComplete = timeSinceStarted / .5f;
                currentColor.a = Mathf.Lerp(1, 0, percentageComplete);
                fadeScreen.color = currentColor;
                if(percentageComplete >= 1)
                { 
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }

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

        protected virtual IEnumerator FadeOut(SceneReference scene)
        {
            float timeStarted = Time.time;
            float timeSinceStarted = Time.time - timeStarted;
            float percentageComplete = timeSinceStarted / .5f;
            Color currentColor = fadeScreen.color;
            while(true)
            {
                timeSinceStarted = Time.time - timeStarted;
                percentageComplete = timeSinceStarted / .5f;
                currentColor.a = Mathf.Lerp(0, 1, percentageComplete);
                fadeScreen.color = currentColor;
                if(percentageComplete >= 1)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            SceneManager.LoadScene(scene);
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(levelSize.center, levelSize.size);
        }
    }
}