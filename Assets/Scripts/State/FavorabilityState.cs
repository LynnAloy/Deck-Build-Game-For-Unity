using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FavorabilityState
{ 
    protected readonly FavorabilitySystem favorabilitySystem;
    protected readonly EnemyView enemy;

    protected FavorabilityState(FavorabilitySystem favorabilitySystem, EnemyView enemy)
    {
        this.favorabilitySystem = favorabilitySystem;
        this.enemy = enemy;
    }

    public abstract void OnStateEnter();
    public abstract void OnStateExit();
    public abstract FavorabilityState CheckTransition();
    public abstract int MinFavorability { get; }
    public abstract int MaxFavorability { get; }

    public abstract float GetAttackMultiplier();

    public abstract bool CanPerformAction(EnemyActionType enemyActionType);
}

public enum EnemyActionType
{
    Attack,
    Trade,
    Assist,
    Ally,
    OfferCard
}
