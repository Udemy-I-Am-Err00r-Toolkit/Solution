using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script should be placed on the FogOfWar tiles and once the Player Indicator runs into those tiles, it removes them; the LevelManager script that creates a list of the removed tiles based on specific IDs created by Unity to ensure they stay removed until you reset PlayerPrefs
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