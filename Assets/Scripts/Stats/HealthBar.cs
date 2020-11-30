using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MetroidvaniaTools
{
    public class HealthBar : Managers
    {
        protected Slider slider;
        protected PlayerHealth playerHealth;

        protected override void Initialization()
        {
            base.Initialization();
            slider = GetComponent<Slider>();
            playerHealth = player.GetComponent<PlayerHealth>();
            slider.maxValue = playerHealth.maxHealthPoints;
            slider.value = PlayerPrefs.GetInt(" " + character.gameFile + "CurrentHealth");
        }

        private void LateUpdate()
        {
            slider.value = playerHealth.healthPoints;
        }
    }
}
