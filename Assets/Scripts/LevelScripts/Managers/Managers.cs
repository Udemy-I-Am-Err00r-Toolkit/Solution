using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class Managers : GameManager
    {
        protected GameManager gameManager;

        protected override void Initialization()
        {
            base.Initialization();
            gameManager = FindObjectOfType<GameManager>();
        }
    }
}
