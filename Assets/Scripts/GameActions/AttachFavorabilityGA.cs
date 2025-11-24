using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachFavorabilityGA : GameAction
{
    public EnemyView Enemy { get; }
    public int InitialFavorability { get; }

    public AttachFavorabilityGA(EnemyView enemy, int initalFavorability)
    {
        Enemy = enemy;
        InitialFavorability = initalFavorability;
    }
}
