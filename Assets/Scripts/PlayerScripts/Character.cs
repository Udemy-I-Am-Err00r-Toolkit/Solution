using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MetroidvaniaTools
{
    public class Character : MonoBehaviour
    {
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
        public bool grabbingLedge;

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

        private Vector2 facingLeft;

        // Start is called before the first frame update
        void Start()
        {
            Initialization();
        }

        

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

        protected virtual void Flip()
        {
            if(isFacingLeft || (!isFacingLeft && isWallSliding))
            {
                transform.localScale = facingLeft;
            }
            if(!isFacingLeft || (isFacingLeft && isWallSliding))
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
        }

    protected virtual bool CollisionCheck(Vector2 direction, float distance, LayerMask collision)
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        int numHits = col.Cast(direction, hits, distance);
        for(int i = 0; i < numHits; i ++)
        {
            if ((1 << hits[i].collider.gameObject.layer & collision) != 0)
            {
                currentPlatform = hits[i].collider.gameObject;
                return true;
            }
        }
        return false;
    }

        public virtual bool Falling(float velocity)
        {
            if (!isGrounded && rb.velocity.y < velocity)
            {
                return true;
            }
            else
                return false;
        }

        protected virtual void FallSpeed(float speed)
        {
            rb.velocity = new Vector2(rb.velocity.x, (rb.velocity.y * speed));
        }

        void NotHi()
        {
            anim.SetBool("Hi", false);
        }

        public void InitializePlayer()
        {
            player = FindObjectOfType<Character>().gameObject;
            player.GetComponent<Character>().isFacingLeft = PlayerPrefs.GetInt(" " + gameFile + "FacingLeft") == 1 ? true : false;
            if(player.GetComponent<Character>().isFacingLeft)
            {
                player.transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
        }
    }
}