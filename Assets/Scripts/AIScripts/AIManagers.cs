using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
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