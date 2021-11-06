using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MetroidvaniaTools
{
    //Health script specific to Player; it houses a lot of extra data that normally an Enemy wouldn't need
    public class PlayerHealth : Health
    {
        //How much time after the Player is hit that they can no longer receive damage; usually a brief amount of time like half a second
        [SerializeField]
        protected float iFrameTime;
        //How much vertical knockback needs to be applied to the Player when they are dealt damage
        [SerializeField]
        protected float verticalDamageForce;
        //How much horizontal knockback needs to be applied to the Player when they are dealt damage
        [SerializeField]
        protected float horizontalDamageForce;
        //How long the time value needs to be adjusted to better visualize when the player is hit; this is an effects feature, not needed for actual gameplay
        [SerializeField]
        protected float slowDownTimeAmount;
        //How much the time value needs to be adjusted to better visualize when the player is hit; this is an effects feature, not needed for actual gameplay
        [SerializeField]
        protected float slowDownSpeed;
        //A reference to all the different sprites that make up the Player; this is used to make the Player slightly transparent when hit to visualize the Player received damage
        protected SpriteRenderer[] sprites;
        //A reference to the Player's RigidBody component to apply knockback force
        protected Rigidbody2D rb;
        //A reference to a UI screen that would pop-up when the Player dies
        protected Image deadScreenImage;
        //A reference to the UI text that would display a "Game Over" type message when the Player dies
        protected Text deadScreenText;
        //The original timescale that needs to go back to after the time is slowed from damage
        protected float originalTimeScale;
        //A bool that prevents damage from happening if the player is either in an iFrameTime or dodge rolling from Dash
        [HideInInspector]
        public bool invulnerable;




        protected override void Initialization()
        {
            base.Initialization();
            sprites = GetComponentsInChildren<SpriteRenderer>();
            deadScreenImage = uiManager.deadScreen.GetComponent<Image>();
            deadScreenText = uiManager.deadScreen.GetComponentInChildren<Text>();
            rb = GetComponent<Rigidbody2D>();
            if(levelManager.loadFromSave)
            {
                healthPoints = PlayerPrefs.GetInt(" " + character.gameFile + "CurrentHealth");
            }
            else
            {
                healthPoints = PlayerPrefs.GetInt("CurrentHealth");
                if(healthPoints == 0)
                {
                    healthPoints = maxHealthPoints;
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            HandleIFrames();
            HandleDamageMovement();
        }

        public override void DealDamage(int amount)
        {
            //If the Player is alive
            if (!character.isDead)
            {
                //If invulnerable or dashing, we return out of this method and deal no damage
                if (invulnerable || character.isDashing)
                {
                    return;
                }
                //If not invulnerable or dashing, then damage is dealt
                base.DealDamage(amount);
                //If health is less than or equal to zero, we manage the Player death state
                if (healthPoints <= 0)
                {
                    character.isDead = true;
                    healthPoints = 0;
                    player.GetComponent<Animator>().SetBool("Dying", true);
                    StartCoroutine(Dead());
                }
                //Puts the Player into a damage state, and quickly sets everything up so we can handle all the damage effects
                originalTimeScale = Time.timeScale;
                hit = true;
                invulnerable = true;
                Invoke("Cancel", iFrameTime);
            }
        }

        //Manages all the damage effects that should happen when damage is dealt
        public virtual void HandleDamageMovement()
        {
            if (hit)
            {
                //Slows down time to the damage speed
                Time.timeScale = slowDownSpeed;
                //Handles vertical and horizontal knockback depending on what direction the Player is facing
                rb.AddForce(Vector2.up * verticalDamageForce);
                if (!left)
                {
                    rb.AddForce(Vector2.right * horizontalDamageForce);
                }
                else
                {
                    rb.AddForce(Vector2.left * horizontalDamageForce);
                }
                //Calls method that cancels damage state after the set amount of time
                Invoke("HitCancel", slowDownTimeAmount);
            }
        }

        //Special effect that makes Player transparent when hit
        protected virtual void HandleIFrames()
        {
            Color spriteColors = new Color();
            if (invulnerable)
            {
                foreach (SpriteRenderer sprite in sprites)
                {
                    spriteColors = sprite.color;
                    spriteColors.a = .5f;
                    sprite.color = spriteColors;
                }
            }
            else
            {
                foreach (SpriteRenderer sprite in sprites)
                {
                    spriteColors = sprite.color;
                    spriteColors.a = 1;
                    sprite.color = spriteColors;
                }
            }
        }

        //Allows Player to receive damage again
        protected virtual void Cancel()
        {
            invulnerable = false;
        }

        //Method that removes player from Damage state
        protected virtual void HitCancel()
        {
            hit = false;
            Time.timeScale = originalTimeScale;
        }

        //This method is called when the Player grabs a health item and it restores Player health; it is called from the HealthConsumable script when player enters trigger collider
        public virtual void GainCurrentHealth(int amount)
        {
            healthPoints += amount;
            if (healthPoints > maxHealthPoints)
            {
                healthPoints = maxHealthPoints;
            }
        }

        //This method handles a lot of UI and special effects that would need to be toggled on when Player dies
        protected virtual IEnumerator Dead()
        {
            uiManager.deadScreen.SetActive(true);
            float timeStarted = Time.time;
            float timeSinceStarted = Time.time - timeStarted;
            float percentageComplete = timeSinceStarted / 2;
            Color currentColor = deadScreenImage.color;
            Color currentTextColor = deadScreenText.color;
            Color spriteColors = new Color();
            foreach (SpriteRenderer sprite in sprites)
            {
                spriteColors = sprite.color;
            }
            while (true)
            {
                timeSinceStarted = Time.time - timeStarted;
                percentageComplete = timeSinceStarted / 2;
                currentColor.a = Mathf.Lerp(0, 1, percentageComplete);
                deadScreenImage.color = currentColor;
                currentTextColor.a = Mathf.Lerp(0, 1, percentageComplete);
                deadScreenText.color = currentTextColor;
                foreach (SpriteRenderer sprite in sprites)
                {
                    spriteColors.a = Mathf.Lerp(0, 1, percentageComplete);
                    sprite.color = spriteColors;
                }
                if (percentageComplete >= 1)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
                //Gets a reference of all the characters within the CharacterManager script
                int gameFile = PlayerPrefs.GetInt("GameFile");
                for (int i = 0; i < character.GetComponent<CharacterManager>().characters.Length; i++)
                {
                    //If the current character is not the being played; this is to add the "(Clone)" string to the character name to properly save the PlayerPref
                    if (levelManager.currentPlayerSelection != i)
                    {
                        //Sets the weapon for this character to whatever was the weapon the last time the game was saved
                        PlayerPrefs.SetInt(character.GetComponent<CharacterManager>().characters[i].name + "(Clone)" + "CurrentWeapon", PlayerPrefs.GetInt(" " + gameFile + character.GetComponent<CharacterManager>().characters[i].name + "(Clone)" + "CurrentWeapon"));
                    }
                    //If the current character is the one being played
                    else
                    {
                        //Sets the weapon for this character to the last weapon that the game file was saved with
                        PlayerPrefs.SetInt(character.name + "CurrentWeapon", PlayerPrefs.GetInt(" " + gameFile + character.name + "CurrentWeapon"));
                    }
                }
            }
            Invoke("LoadGame", 2);
        }

        //Loads Player back to the last point in which the game was saved; this is called when the Player dies as well as when you load a game from the main menu screen
        public virtual void LoadGame()
        {
            levelManager.loadFromSave = true;
            PlayerPrefs.SetInt(" " + character.gameFile + "LoadFromSave", levelManager.loadFromSave ? 1 : 0);
            string scene = PlayerPrefs.GetString(" " + character.gameFile + "LoadGame");
            SceneManager.LoadScene(scene);
        }
    }
}