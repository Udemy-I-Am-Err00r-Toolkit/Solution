using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script resets the position of the BigMap when it is disabled so that if you move the BigMapCamera away from the Player Indicator position, it snaps back to it when the BigMap is closed
    public class BigMap : Managers
    {
        protected virtual void LateUpdate()
        {
            if (!uiManager.bigMapOn)
            {
                transform.position = new Vector3(playerIndicator.transform.position.x, playerIndicator.transform.position.y, -10);
            }
        }
    }
}