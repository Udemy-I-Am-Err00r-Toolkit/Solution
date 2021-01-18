using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script manages what slot a new game and load game should reference
    public class MenuSlot : MonoBehaviour
    {
        //This needs to be setup in the inspector when making UI for the New Game slots and Load Game slots; in the course, we set these as either 1, 2, or 3
        public int slotNumber;
        //A quick reference to the Main Menu so the UI can load the correct screens depending on what button is pressed
        protected TitleScreen titleScreen;

        private void Start()
        {
            titleScreen = FindObjectOfType<TitleScreen>();
        }

        public virtual void NewGameSlot()
        {
            titleScreen.NewGame(slotNumber);
        }

        public virtual void LoadGameSlot()
        {
            titleScreen.LoadGame(slotNumber);
        }

        public virtual void GoBackToMainMenu()
        {
            titleScreen.MainMenu();
        }
    }
}