using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostileState : FavorabilityState
{
    public HostileState(FavorabilitySystem favorabilitySystem, EnemyView enemy) 
        : base(favorabilitySystem, enemy) { }
    public override int MinFavorability => 0;
    public override int MaxFavorability => 30;

    public override void OnStateEnter()
    {
        Debug.Log($"½øÈë HostileState - µĞÈË: {enemy.name}");
        TraitSystem.Instance.SetTraitCoverVisibility(enemy, true);
    }

    public override void OnStateExit()
    {
        // No specific exit actions needed for HostileState
    }

    public override FavorabilityState CheckTransition()
    {
        if(favorabilitySystem.GetFavorabilityValue(enemy) > MaxFavorability)
        {
            return new NeutralState(favorabilitySystem, enemy);
        }
        return this;
    }

    public override float GetAttackMultiplier()
    {
        return 1.0f;
    }

    public override bool CanPerformAction(EnemyActionType enemyActionType)
    {
        return enemyActionType switch
        {
            EnemyActionType.Attack => true,
            EnemyActionType.Trade => false,
            EnemyActionType.Assist => false,
            EnemyActionType.Ally => false,
            EnemyActionType.OfferCard => false,
            _ => false,
        };
    }
}
