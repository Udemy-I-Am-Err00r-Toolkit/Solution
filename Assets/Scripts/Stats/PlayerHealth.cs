using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MetroidvaniaTools
{
    public class PlayerHealth : Health
    {
        [SerializeField]
        protected float iFrameTime;
        [SerializeField]
        protected float verticalDamageForce;
        [SerializeField]
        protected float horizontalDamageForce;
        [SerializeField]
        protected float slowDownTimeAmount;
        [SerializeField]
        protected float slowDownSpeed;
        protected SpriteRenderer[] sprites;
        protected Rigidbody2D rb;
        protected Image deadScreenImage;
        protected Text deadScreenText;
        protected float originalTimeScale;
        [HideInInspector]
        public bool invulnerable;
        [HideInInspector]
        public bool hit;
        [HideInInspector]
        public bool left;




        protected override void Initialization()
        {
            base.Initialization();
            sprites = GetComponentsInChildren<SpriteRenderer>();
            deadScreenImage = uiManager.deadScreen.GetComponent<Image>();
            deadScreenText = uiManager.deadScreen.GetComponentInChildren<Text>();
            rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void FixedUpdate()
        {
            HandleIFrames();
            HandleDamageMovement();
        }

        public override void DealDamage(int amount)
        {
            if (!character.isDead)
            {
                if (invulnerable || character.isDashing)
                {
                    return;
                }
                base.DealDamage(amount);
                if (healthPoints <= 0)
                {
                    character.isDead = true;
                    healthPoints = 0;
                    player.GetComponent<Animator>().SetBool("Dying", true);
                    StartCoroutine(Dead());
                }
                originalTimeScale = Time.timeScale;
                hit = true;
                invulnerable = true;
                Invoke("Cancel", iFrameTime);
            }
        }

        public virtual void HandleDamageMovement()
        {
            if (hit)// && !character.isDead)
            {
                Time.timeScale = slowDownSpeed;
                rb.AddForce(Vector2.up * verticalDamageForce);
                if (!left)
                {
                    rb.AddForce(Vector2.right * horizontalDamageForce);
                }
                else
                {
                    rb.AddForce(Vector2.left * horizontalDamageForce);
                }
                Invoke("HitCancel", slowDownTimeAmount);
            }
        }

        protected virtual void HandleIFrames()
        {
            Color spriteColors = new Color();
            if(invulnerable)
            {
                foreach(SpriteRenderer sprite in sprites)
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

        protected virtual void Cancel()
        {
            invulnerable = false;
        }

        protected virtual void HitCancel()
        {
            hit = false;
            Time.timeScale = originalTimeScale;
        }

        public virtual void GainCurrentHealth(int amount)
        {
            healthPoints += amount;
            if(healthPoints > maxHealthPoints)
            {
                healthPoints = maxHealthPoints;
            }
        }

        protected virtual IEnumerator Dead()
        {
            uiManager.deadScreen.SetActive(true);
            float timeStarted = Time.time;
            float timeSinceStarted = Time.time - timeStarted;
            float percentageComplete = timeSinceStarted / 2;
            Color currentColor = deadScreenImage.color;
            Color currentTextColor = deadScreenText.color;
            Color spriteColors = new Color();
            foreach(SpriteRenderer sprite in sprites)
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
                foreach(SpriteRenderer sprite in sprites)
                {
                    spriteColors.a = Mathf.Lerp(0, 1, percentageComplete);
                    sprite.color = spriteColors;
                }
                if (percentageComplete >= 1)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            Invoke("LoadGame", 2);
        }

        public virtual void LoadGame()
        {
            levelManager.loadFromSave = true;
            PlayerPrefs.SetInt(" " + character.gameFile + "LoadFromSave", levelManager.loadFromSave ? 1 : 0);
            string scene = PlayerPrefs.GetString(" " + character.gameFile + "LoadGame");
            SceneManager.LoadScene(scene);
        }
    }
}