using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script loads the appropriate next scene based on whatever door you are going through; this requires correct spawn and scene references from the variables below
    public class NextScene : Managers
    {
        //The scene that needs to be loaded when going through a door
        [SerializeField]
        protected SceneReference nextScene;
        //The loacation reference in that scene from the LevelManager script that the player should spawn at when the scene loads
        [SerializeField]
        protected int locationReference;

        protected override void Initialization()
        {
            base.Initialization();
            //Adds the NewCharacter method to the event for the CharacterManager delegate
            CharacterManager.CharacterUpdate += NewCharacter;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == player)
            {
                levelManager.NextScene(nextScene, locationReference);
            }
        }

        //Runs the UpdateCharacter method on the GameManager script
         protected virtual void NewCharacter()
        {
            UpdateCharacter();
        }
    }
}