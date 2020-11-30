using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class Hook : MonoBehaviour
    {
        protected GameObject player;
        protected GrapplingHook grapplingHook;
        [SerializeField]
        protected LayerMask layers;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            player = GameObject.FindWithTag("Player");
            grapplingHook = player.GetComponent<GrapplingHook>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if((1 << collision.gameObject.layer & layers) != 0 && !grapplingHook.connected)
            {
                grapplingHook.connected = true;
                grapplingHook.objectConnectedTo = collision.gameObject;
                collision.GetComponent<HingeJoint2D>().enabled = true;
                collision.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
            }
        }
    }
}