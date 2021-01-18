using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //Simple script that is attached to any gameobject the player should be able to climb on as a ledge; the variables in this script will help in snapping the player to the most appropriate position when hanging. This script works best the platform is not a hookable platform or a one-way platform
    public class Ledge : MonoBehaviour
    {
        public float hangingHorizontalOffset;
        public float hangingVerticalOffset;
    }
}