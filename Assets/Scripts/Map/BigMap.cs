using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class BigMap : Managers
    {
        protected virtual void LateUpdate()
        {
            if(!uiManager.bigMapOn)
            {
                transform.position = new Vector3(playerIndicator.transform.position.x, playerIndicator.transform.position.y, -10);
            }
        }
    }
}