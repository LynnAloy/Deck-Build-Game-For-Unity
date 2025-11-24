using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrawCardGA : GameAction
{
    public int Amount { get; set; }
    public EnemyView EnemyView { get; set; }
    public EnemyDrawCardGA(int amount, EnemyView targetEnemy)
    {
        Amount = amount;
        EnemyView = targetEnemy;
    }
}
