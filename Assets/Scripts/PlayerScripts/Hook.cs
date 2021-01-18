using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script will go on the GameObject that will act as the end of the GrapplingHook
    public class Hook : MonoBehaviour
    {
        //A quick reference to the player
        protected GameObject player;
        //A quick reference to the GrapplingHook
        protected GrapplingHook grapplingHook;
        //What layers the Hook will interact with and allow the player to connect with as a grappling hook
        [SerializeField]
        protected LayerMask layers;

        protected virtual void Start()
        {
            player = GameObject.FindWithTag("Player");
            grapplingHook = player.GetComponent<GrapplingHook>();
        }

        //Checks to see if the Hook is connected with a layer that would allow the player to enter a Grappling state
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if ((1 << collision.gameObject.layer & layers) != 0 && !grapplingHook.connected)
            {
                grapplingHook.connected = true;
                grapplingHook.objectConnectedTo = collision.gameObject;
                collision.GetComponent<HingeJoint2D>().enabled = true;
                collision.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
            }
        }
    }
}