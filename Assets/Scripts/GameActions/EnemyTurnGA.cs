using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnGA : GameAction
{
    public void OnClick()
    {
        EnemyTurnGA enemyTurnGA = new();
        ActionSystem.Instance.Perform(enemyTurnGA);
    }
}
