using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //A quick reference to the EnemyCharacter script for all the different AI scripts
    public class AIManagers : EnemyCharacter
    {
        protected EnemyCharacter enemyCharacter;

        protected override void Initialization()
        {
            base.Initialization();
            enemyCharacter = GetComponent<EnemyCharacter>();
        }
    }
}