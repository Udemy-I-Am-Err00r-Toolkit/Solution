using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class CustomAnimations : Abilities
    {
        public Animation footTap;
        public Animation getDown;
        public Animation flip;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                anim.SetBool("FootTap", true);
                anim.SetBool("GetDown", false);
                anim.SetBool("Flip", false);
            }
            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                anim.SetBool("GetDown", true);
                anim.SetBool("FootTap", false);
                anim.SetBool("Flip", false);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                anim.SetBool("Flip", true);
                anim.SetBool("GetDown", false);
                anim.SetBool("FootTap", false);
                Invoke("NotFlipping", 1);
            }
        }

        void NotFlipping()
        {
            anim.SetBool("Flip", false);
            anim.SetBool("FootTap", true);
        }
    }
}