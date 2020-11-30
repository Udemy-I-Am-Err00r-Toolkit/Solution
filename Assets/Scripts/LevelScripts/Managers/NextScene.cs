using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class NextScene : Managers
    {
        [SerializeField]
        protected SceneReference nextScene;
        [SerializeField]
        protected int locationReference;

        protected override void Initialization()
        {
            base.Initialization();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject == player)
            {
                levelManager.NextScene(nextScene, locationReference);
            }
        }

    }
}