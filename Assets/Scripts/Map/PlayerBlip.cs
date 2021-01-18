using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This method will give the Player Indicator some effects to throb and change colors to it is more visible on the mini-map and big map
    public class PlayerBlip : Managers
    {
        //How much time should wait before the transition change starts; this is different than changeSpeed which manages how quickly it goes from black to yellow and big to small
        [SerializeField]
        protected float throbSpeed;
        //How fast the Player Indicator changes goes from black to yellow and big to small; this is different than throbSpeed, which manages how long the blip should wait before transitioning
        [SerializeField]
        protected float changeSpeed;
        //How much the Player Indicator grows when it throbs
        [SerializeField]
        protected float blipSizeMultiplier;
        //What color the Player Indicator should be when it is large; in the course this color is yellow
        [SerializeField]
        protected Color bigColor;
        //What color the Player Indicator should be when it is small; in the course this color is black
        [SerializeField]
        protected Color littleColor;
        //A quick reference of the size it needs to throb to and from
        protected Vector3 throbSize;
        //A quick reference of what the original size of the Player Indicator is
        protected Vector3 originalSize;
        //A needed reference to change the color of the sprite between the big and little color
        protected SpriteRenderer sprite;

        protected override void Initialization()
        {
            base.Initialization();
            originalSize = transform.localScale;
            throbSize = new Vector3(transform.localScale.x * blipSizeMultiplier, transform.localScale.y * blipSizeMultiplier);
            sprite = GetComponent<SpriteRenderer>();
        }

        protected virtual void FixedUpdate()
        {
            Throb();
        }

        //A somewhat complicated solution I found online to grow and shrink the Player Indicator based on calculus math and then lerping the results of this math between the original state and the throbbed state; never took cal
        protected virtual void Throb()
        {
            float t = Mathf.Sin((Time.time - Time.deltaTime) * throbSpeed) * changeSpeed;
            transform.localScale = Vector3.Lerp(throbSize, originalSize, t);
            sprite.color = Color.Lerp(bigColor, littleColor, t);
        }
    }
}