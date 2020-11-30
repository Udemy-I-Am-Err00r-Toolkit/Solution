using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class Abilities : Character
    {
        [HideInInspector]
        public bool dashAbility;
        [HideInInspector]
        public bool wallJumpAbility;

        protected Character character;

        protected override void Initialization()
        {
            base.Initialization();
            character = GetComponent<Character>();
            dashAbility = true;
            wallJumpAbility = PlayerPrefs.GetInt(" " + character.gameFile + "WallJumpAbility") == 1 ? true : false;
            TurnOnAbilities();
        }

        public virtual void DashAbility()
        {
            dashAbility = true;
            dash.enabled = true;
            PlayerPrefs.SetInt(" " + character.gameFile + "DashAbility", dashAbility ? 1 : 0);
        }

        public virtual void WallJumpAbility()
        {
            jump.wallJumpAbility = true;
            PlayerPrefs.SetInt(" " + character.gameFile + "WallJumpAbility", jump.wallJumpAbility ? 1 : 0);
        }

        public virtual void TurnOnAbilities()
        {
            if(dashAbility)
            {
                dash.enabled = true;
            }
            if(wallJumpAbility)
            {
                jump.wallJumpAbility = true;
            }
        }
    }
}