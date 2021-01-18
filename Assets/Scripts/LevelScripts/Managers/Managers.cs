using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //A quick reference to the different objects that will manage levels and overall data that should persist between plays
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
