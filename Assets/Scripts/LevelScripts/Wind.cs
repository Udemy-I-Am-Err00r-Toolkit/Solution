using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //A simple script that works with the AreaEffector2D component; this script just ensures that if you want the direction of wind to go up, it goes up regardless of it's rotation value; if you want a custom direction for wind however, just rotate the game object in that direction and ensure the goingUp bool is set to false
    [RequireComponent(typeof(BoxCollider2D), typeof(AreaEffector2D))]
    public class Wind : Managers
    {
        [SerializeField]
        protected bool goingUp;
        protected AreaEffector2D effector;

        protected override void Initialization()
        {
            base.Initialization();
            effector = GetComponent<AreaEffector2D>();
            if (goingUp)
            {
                effector.forceAngle = 90;
            }
            else
            {
                effector.useGlobalAngle = true;
            }
        }
    }
}