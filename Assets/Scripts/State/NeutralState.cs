using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralState : FavorabilityState
{
    public NeutralState(FavorabilitySystem favorabilitySystem, EnemyView enemy) 
        : base(favorabilitySystem, enemy) { }
    public override int MinFavorability => 31;
    public override int MaxFavorability => 70;

    public override void OnStateEnter()
    {
        Debug.Log($"½øÈë NeutralState - µÐÈË: {enemy.name}");
        TraitSystem.Instance.SetTraitCoverVisibility(enemy, true);
    }

    public override void OnStateExit()
    {

    }

    public override FavorabilityState CheckTransition()
    {
        var favor = favorabilitySystem.GetFavorabilityValue(enemy);
        if(favor > MaxFavorability)
        {
            return new FriendlyState(favorabilitySystem, enemy);
        }
        else if(favor < MinFavorability)
        {
            return new HostileState(favorabilitySystem, enemy);
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
            EnemyActionType.Trade => true,
            EnemyActionType.Assist => false,
            EnemyActionType.Ally => false,
            EnemyActionType.OfferCard => false,
            _ => false,
        };
    }
}
