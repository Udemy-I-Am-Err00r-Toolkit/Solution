using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class PlayerBlip : Managers
    {
        [SerializeField]
        protected float throbSpeed;
        [SerializeField]
        protected float changeSpeed;
        [SerializeField]
        protected float blipSizeMultiplier;
        [SerializeField]
        protected Color  bigColor;
        [SerializeField]
        protected Color  littleColor;
        protected Vector3 throbSize;
        protected Vector3 originalSize;
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

        protected virtual void Throb()
        {
            float t = Mathf.Sin((Time.time - Time.deltaTime) * throbSpeed) * changeSpeed;
            transform.localScale = Vector3.Lerp(throbSize, originalSize, t);
            sprite.color = Color.Lerp(bigColor, littleColor, t);
        }
    }
}