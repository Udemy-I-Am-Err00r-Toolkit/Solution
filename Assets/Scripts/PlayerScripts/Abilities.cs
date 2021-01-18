using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script helps define the Character script for other scripts that would inherit from this one, but later in the course it manages whether or not certain abilities should be active or inactive depending on if the power up has been found
    public class Abilities : Character
    {
        //One of the examples I provide at the end of this course on how we can turn on an entire component based on whether or not pickup was found
        [HideInInspector]
        public bool dashAbility;
        //One of the examples I provide at the end of this course on how we can toggle certain parts of a script to allow small portions of logic to flow if pickup was found
        [HideInInspector]
        public bool wallJumpAbility;

        protected Character character;

        protected override void Initialization()
        {
            base.Initialization();
            character = GetComponent<Character>();
            //Checks to see if a pickup was found based on a bool that is derived from PlayerPrefs; this value persists even after the game is shutdown so once you find the pickup, this value will remain true
            dashAbility = PlayerPrefs.GetInt(" " + character.gameFile + "DashAbility") == 1 ? true : false;
            //Checks to see if a pickup was found based on a bool that is derived from PlayerPrefs; this value persists even after the game is shutdown so once you find the pickup, this value will remain true
            wallJumpAbility = PlayerPrefs.GetInt(" " + character.gameFile + "WallJumpAbility") == 1 ? true : false;
            //Checks to see if any ability bools are true and allows those abilities to be used at Start
            TurnOnAbilities();
        }

        //This method is called by the pickup that allows the Player to dash
        public virtual void DashAbility()
        {
            dashAbility = true;
            dash.enabled = true;
            PlayerPrefs.SetInt(" " + character.gameFile + "DashAbility", dashAbility ? 1 : 0);
        }

        //This method is called by the pickup that allows the Player to wall jump
        public virtual void WallJumpAbility()
        {
            jump.wallJumpAbility = true;
            PlayerPrefs.SetInt(" " + character.gameFile + "WallJumpAbility", jump.wallJumpAbility ? 1 : 0);
        }

        //This method runs at Start and lets components know if they should be active at Start, allowing that ability, or if bools found in certain scripts should be true to allow that ability
        public virtual void TurnOnAbilities()
        {
            if (dashAbility)
            {
                dash.enabled = true;
            }
            if (wallJumpAbility)
            {
                jump.wallJumpAbility = true;
            }
        }
    }
}