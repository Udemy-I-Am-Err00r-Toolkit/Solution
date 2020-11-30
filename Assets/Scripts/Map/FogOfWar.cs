using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class FogOfWar : Managers
    {
        protected override void Initialization()
        {
            base.Initialization();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerIndicatorMovement>())
            {
                levelManager.RemoveFog(this);
            }
        }
    }
}