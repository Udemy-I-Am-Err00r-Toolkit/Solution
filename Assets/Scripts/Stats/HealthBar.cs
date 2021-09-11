using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MetroidvaniaTools
{
    //This manages the UI for a HealthBar on the Player
    public class HealthBar : Managers
    {
        //This is a UI element that would fill up when gaining health, and deplete when damage is dealt
        protected Slider slider;
        //A quick reference to the playerHealth script on the Player
        protected PlayerHealth playerHealth;

        protected override void Initialization()
        {
            base.Initialization();
            slider = GetComponent<Slider>();
            //Debug.Log(player.name);
            playerHealth = player.GetComponent<PlayerHealth>();
            //Gets an accurate value for how much the UI needs to fill up to when Player health is at max value
            slider.maxValue = playerHealth.maxHealthPoints;
            //The current value of the health that the UI should fill up to; this value is controlled through a PlayerPref so it can persist between scenes
            slider.value = PlayerPrefs.GetInt(" " + character.gameFile + "CurrentHealth");
        }

        //Updates the UI to match the health bar with the current health value depending on what the current health is on the Player
        private void LateUpdate()
        {
            slider.value = playerHealth.healthPoints;
        }
    }
}
