using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
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
            if(goingUp)
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