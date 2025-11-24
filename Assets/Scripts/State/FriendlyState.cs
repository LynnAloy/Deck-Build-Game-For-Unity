using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyState : FavorabilityState
{
    public FriendlyState(FavorabilitySystem favorabilitySystem, EnemyView enemy) 
        : base(favorabilitySystem, enemy) { }
    public override int MinFavorability => 71;
    public override int MaxFavorability => 100;

    public override void OnStateEnter()
    {
        Debug.Log($"Ω¯»Î FriendlyState - µ–»À: {enemy.name}");
        TraitSystem.Instance.SetTraitCoverVisibility(enemy, true);
        //enemy's cards added to player's deck
    }

    public override void OnStateExit()
    {

    }

    public override FavorabilityState CheckTransition()
    {
        if(favorabilitySystem.GetFavorabilityValue(enemy) < MinFavorability)
        {
            return new NeutralState(favorabilitySystem, enemy);
        }
        return this;
    }

    public override float GetAttackMultiplier()
    {
        return 0f;
    }

    public override bool CanPerformAction(EnemyActionType enemyActionType)
    {
        return enemyActionType switch
        {
            EnemyActionType.Attack => false,
            EnemyActionType.Trade => true,
            EnemyActionType.Assist => true,
            EnemyActionType.Ally => true,
            EnemyActionType.OfferCard => true,
            _ => false,
        };
    }

    /*
    private void ShareCards()
    {
        if(EnemyCardSystem.Instance == null || CardSystem.Instance == null)
            return;
        var enemyDeck = EnemyCardSystem.Instance.GetEnemyDeck(enemy);
        if(enemyDeck?.Hand == null)
            return;
        List<Card> sharedCards = new(enemyDeck.Hand);
        foreach(var card in sharedCards)
        {
            CardSystem.Instance.AddCardToDeck(card);
        }
    }
    */
}
