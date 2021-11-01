﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //New script inspired by a student; will allow you to switch characters with the press of a button you decide in the inspector
    public class CharacterManager : Abilities
    {
        //List of playable characters that you can switch between
        public GameObject[] characters;
        //Current selection on the characters list that is the character
        //protected int currentSelection; 

        protected LevelManager levelManager;

        //Delegate void that lets other scripts know when this button is pressed so they can assign new player
        public delegate void UpdateCharacter();
        public static event UpdateCharacter CharacterUpdate;

        
        protected override void Initialization()
        {
            base.Initialization();
            levelManager = FindObjectOfType<LevelManager>();
            /*
            //Checks if the game is being loaded from a save to bring in the correct character from last save
            bool loadFromSave = PlayerPrefs.GetInt(" " + gameFile + "LoadFromSave") == 1 ? true : false;
            //If the game is being loaded from a save
            if (loadFromSave)
            {
                //Sets the current selection to the saved game file
                currentSelection = PlayerPrefs.GetInt(" " + gameFile + "Character");
            }
            //If the game is not being loaded from a save
            else
            {
                //Sets the current selection to the previous scene
                currentSelection = PlayerPrefs.GetInt("Character");
            }
            if(currentSelection >= characters.Length || currentSelection < 0)
            {
                currentSelection = 0;
            }
            */
        }
        

        // Update is called once per frame
        void Update()
        {
            //Checks for input and handles logic
            CheckChangeCharacter();
        }

        void CheckChangeCharacter()
        {
            //If the button setup in the inspector to change characters is pressed
            if(input.ChangeCharacterPressed())
            {
                //Increase currentSelection by 1
                levelManager.currentPlayerSelection++;
                //Checks to see if currentSelection is outside of the amount
                if(levelManager.currentPlayerSelection == characters.Length)
                {
                    //If it is, sets currentSelection to 0
                    levelManager.currentPlayerSelection = 0;
                }
                //Sets the PlayerPrefs so if the scene changes, the new scene has the correct value for the current Character
                PlayerPrefs.SetInt("Character", levelManager.currentPlayerSelection);
                //Runs a method in the Game Manager script to change the character
                gameManager.ChangeCharacter(characters[levelManager.currentPlayerSelection]);
                //Lets the other scripts listening to this event know the character has changed
                CharacterUpdate();
            }
        }
    }
}