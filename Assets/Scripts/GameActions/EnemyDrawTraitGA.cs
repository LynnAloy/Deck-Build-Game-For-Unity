using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrawTraitGA : GameAction
{
    public EnemyView EnemyView { get; private set; }

    public EnemyDrawTraitGA(EnemyView enemyView)
    {
        EnemyView = enemyView;
    }
}
