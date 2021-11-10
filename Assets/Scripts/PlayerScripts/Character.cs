using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
/*
 * This script is placed on the player and handles many of the bools that would manage the different
 * states the character can be in as well as setup common compononents so all the other scripts
 * on the player have similar references.
 */

namespace MetroidvaniaTools
{
    public class Character : MonoBehaviour
    {
        //These different bools will be managed on the individual child scripts and manage the different
        //states the player can be in.
        [HideInInspector]
        public bool isFacingLeft;
        [HideInInspector]
        public bool isJumping;
        [HideInInspector]
        public bool isJumpingThroughPlatform;
        [HideInInspector]
        public bool isSwimming;
        [HideInInspector]
        public bool isOnLadder;
        [HideInInspector]
        public bool isGrounded;
        [HideInInspector]
        public bool isCrouching;
        [HideInInspector]
        public bool isDashing;
        [HideInInspector]
        public bool isWallSliding;
        [HideInInspector]
        public bool isDead;
        [HideInInspector]
        public int gameFile;
        [HideInInspector]
        public int currentWeaponSelected;
        [HideInInspector]
        public bool grabbingLedge;
        [HideInInspector]
        public bool meleeAttacking;
        [HideInInspector]
        public bool sprintingMeleeAttack;
        [HideInInspector]
        public bool isSprinting;

        //These are common component refernces so other scripts can talk to each other if they need to.
        protected Collider2D col;
        protected Rigidbody2D rb;
        protected Animator anim;
        protected HorizontalMovement movement;
        protected Jump jump;
        protected InputManager input;
        protected ObjectPooler objectPooler;
        protected AimManager aimManager;
        protected Weapon weapon;
        protected GrapplingHook grapplingHook;
        protected Dash dash;
        protected GameObject currentPlatform;
        protected GameObject player;
        protected GameManager gameManager;
        public RaycastHit2D[] hits;

        private Vector2 facingLeft;

        // Start is called before the first frame update
        void Start()
        {
            Initialization();
        }


        //This is essentially the Start() method for all the child scripts of the Character script.
        protected virtual void Initialization()
        {
            gameFile = PlayerPrefs.GetInt("GameFile");
            col = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            movement = GetComponent<HorizontalMovement>();
            jump = GetComponent<Jump>();
            input = GetComponent<InputManager>();
            objectPooler = ObjectPooler.Instance;
            aimManager = GetComponent<AimManager>();
            weapon = GetComponent<Weapon>();
            grapplingHook = GetComponent<GrapplingHook>();
            dash = GetComponent<Dash>();
            gameManager = FindObjectOfType<GameManager>();
            facingLeft = new Vector2(-transform.localScale.x, transform.localScale.y);
        }

        //This method manages having the player face the correct direction
        protected virtual void Flip()
        {
            if (isFacingLeft || (!isFacingLeft && isWallSliding))
            {
                transform.localScale = facingLeft;
            }
            if (!isFacingLeft || (isFacingLeft && isWallSliding))
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
        }

        //This method is called by the child scripts anytime we need to ensure the player is touching somethin;
        //this method will require a direction it needs to check in, a distance from the player it needs to check,
        //and what layers the player will need to check for.
        protected virtual bool CollisionCheck(Vector2 direction, float distance, LayerMask collision)
        {
            hits = new RaycastHit2D[10];
            int numHits = col.Cast(direction, hits, distance);
            for (int i = 0; i < numHits; i++)
            {
                if ((1 << hits[i].collider.gameObject.layer & collision) != 0)
                {
                    currentPlatform = hits[i].collider.gameObject;
                    return true;
                }
            }
            return false;
        }

        //This method will check to see if the player should enter the Falling state; requires how fast the player should
        //be falling in order to officially enter the Fall state.
        public virtual bool Falling(float velocity)
        {
            if (!isGrounded && rb.velocity.y < velocity)
            {
                return true;
            }
            else
                return false;
        }

        //This method will have the Player fall at a certain speed if it needs to be adjusted; for example when gliding or wall sliding.
        protected virtual void FallSpeed(float speed)
        {
            rb.velocity = new Vector2(rb.velocity.x, (rb.velocity.y * speed));
        }

        //This method will be handled by the LevelManager and GameManager scripts, will instantiate the player into the scene where the player needs to be.
        public void InitializePlayer(int characterSelected)
        {
            //Refernce to the current game file
            gameFile = PlayerPrefs.GetInt("GameFile");
            //Reference to the current character loaded into the scene
            player = FindObjectOfType<Character>().gameObject;
            //Checks to see if the game is loading from a save or a scene change
            bool loadFromSave = PlayerPrefs.GetInt(" " + gameFile + "LoadFromSave") == 1 ? true : false;
            //If loading from save, the if statement logic runs
            if (loadFromSave)
            {
                for(int i = 0; i < player.GetComponent<CharacterManager>().characters.Length; i ++)
                {
                    //Creates a temporary GameObject variable for convenience naming
                    GameObject character = player.GetComponent<CharacterManager>().characters[i];
                    //Checks if the iteration value is not the current character
                    if (characterSelected != i)
                    {
                        //Sets the current weapon for this character
                        PlayerPrefs.SetInt(character.name + "(Clone)" + "CurrentWeapon", PlayerPrefs.GetInt(" " + gameFile + character.name + "(Clone)" + "CurrentWeapon"));
                        //Sets the current health for this character
                        PlayerPrefs.SetInt(character.name + "(Clone)" + "CurrentHealth", PlayerPrefs.GetInt(" " + gameFile + character.name + "(Clone)" + "CurrentHealth"));
                    }
                    //Checks if the iteration value is the current character
                    else
                    {
                        //Sets the current weapon for this character
                        PlayerPrefs.SetInt(character.name + "CurrentWeapon", PlayerPrefs.GetInt(" " + gameFile + character.name + "CurrentWeapon"));
                        //Sets the current health for this character
                        PlayerPrefs.SetInt(character.name + "CurrentHealth", PlayerPrefs.GetInt(" " + gameFile + character.name + "CurrentHealth"));
                    }
                }
                //Has the character facing the direction from last save
                player.GetComponent<Character>().isFacingLeft = PlayerPrefs.GetInt(" " + gameFile + "FacingLeft") == 1 ? true : false;
                
            }
            //If not loading from a save
            else
            {
                //Sets the player in the correct direction when based on the PlayerPref "FacingLeft"
                player.GetComponent<Character>().isFacingLeft = PlayerPrefs.GetInt("FacingLeft") == 1 ? true : false;
            }
            //Sets the current weapon for the player based on the previous load data; if no load data exists, it goes with the first iteration by default
            player.GetComponent<Character>().currentWeaponSelected = PlayerPrefs.GetInt(player.name + "CurrentWeapon");
            //Sets the current health for the player based on the previous load data; if no load data exists, it goes with the first iteration by default
            player.GetComponent<Health>().healthPoints = PlayerPrefs.GetInt(player.name + "CurrentHealth");
            //If the character is facing left
            if (player.GetComponent<Character>().isFacingLeft)
            {
                //Has the player face the left direction and ensures they can Flip the correct direction
                player.transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
        }
    }
}