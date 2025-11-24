using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] protected EnemyBoardView enemyBoardView;
    [SerializeField] private int enemyDrawCardAmount;
    [SerializeField] private FavorabilitySystem favorabilitySystem;
    public List<EnemyView> Enemies => enemyBoardView.EnemyViews;
    private Dictionary<EnemyView, FavorabilityState> enemyStates = new();
    private bool initOnce = false;
    

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackHeroGA>(AttackHeroPerformer);
        ActionSystem.AttachPerformer<KillEnemyGA>(KillEnemyPerformer);
        //ActionSystem.AttachPerformer<EnemyDrawCardGA>(DrawCardsPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
        ActionSystem.DetachPerformer<KillEnemyGA>();
        //ActionSystem.DetachPerformer<EnemyDrawCardGA>();
    }

    public void Setup(List<EnemyData> enemyDatas)
    {
        initOnce = false;
        foreach(var enemyData in enemyDatas)
        {
            var enemyView = enemyBoardView.AddEnemy(enemyData);
            ActionSystem.Instance.AddReaction(new AttachFavorabilityGA(enemyView, FavorabilitySystem.Instance.GetFavorabilityValue(enemyView)));
            if (!initOnce) 
                ActionSystem.Instance.AddReaction(new EnemyDrawTraitGA(enemyView));
            
            // 为该敌人初始化独立牌堆（使用 EnemyData.Deck）
            if (EnemyCardSystem.Instance != null)
            {
                EnemyCardSystem.Instance.Setup(enemyData.Deck, enemyView);
            }
            else
            {
                Debug.LogError("EnemyCardSystem.Instance is null when initializing enemy deck!");
            }
        }
        initOnce = true; 
    }


    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        foreach (var enemy in enemyBoardView.EnemyViews)
        {
            EnemyDrawCardGA enemyDrawCardGA = new(enemyDrawCardAmount, enemy);
            ActionSystem.Instance.AddReaction(enemyDrawCardGA);
            EnemyDrawTraitGA enemyDrawTraitGA = new(enemy);
            ActionSystem.Instance.AddReaction(enemyDrawTraitGA);
            yield return null;
        }

        foreach (var enemy in enemyBoardView.EnemyViews)
        {
            var enmeyState = GetEnemyFavorabilityState(enemy);
            if (enmeyState == null) continue;
            int burnStacks = enemy.GetStatusEffectStacks(StatusEffectType.BURN);
            if(burnStacks > 0)
            {
                ApplyBurnGA applyBurnGA = new(burnStacks, enemy);
                ActionSystem.Instance.AddReaction(applyBurnGA);
            }
            //Enemy attack
            if (EnemyCardSystem.Instance != null && enmeyState.CanPerformAction(EnemyActionType.Attack))
            {
                //Not implement multiplier attack yet.
                yield return EnemyCardSystem.Instance.EnemyPlayCard(enemy);
                Debug.Log("敌人使用卡牌攻击!");
            }
            else if(enmeyState.CanPerformAction(EnemyActionType.Attack))
            {
                Debug.Log("敌人无卡牌，使用基础攻击!");
                AttackHeroGA attackHeroGA = new(enemy);
                ActionSystem.Instance.AddReaction(attackHeroGA);
            }
        }
        //Give card to hero
        foreach (var enemy in enemyBoardView.EnemyViews)
        {
            var enmeyState = GetEnemyFavorabilityState(enemy);
            if (enmeyState == null) continue;
            if (enmeyState.CanPerformAction(EnemyActionType.OfferCard))
            {
                Debug.Log("Give Hero cards.");
                EnemyCardSystem.Instance.ConvertTargetMode(EnemyCardSystem.Instance.GetEnemyDeck(enemy));
                AddCardsToHero(enemy);
            }
        }
        yield return null;
    }

    private IEnumerator AttackHeroPerformer(AttackHeroGA attackHeroGA)
    {
        EnemyView attacker = attackHeroGA.Attacker;
        Tween tween = attacker.transform.DOMoveX(attacker.transform.position.x - 1f, 0.15f);
        yield return tween.WaitForCompletion();
        attacker.transform.DOMoveX(attacker.transform.position.x + 1f, 0.25f);
        DealDamageGA dealDamageGA = new(attacker.AttackPower, new() { HeroSystem.Instance.HeroView }, attackHeroGA.Caster);
        ActionSystem.Instance.AddReaction(dealDamageGA);
    }

    private IEnumerator KillEnemyPerformer(KillEnemyGA killEnemyGA)
    {
        yield return enemyBoardView.RemoveEnemy(killEnemyGA.EnemyView);
        EnemyCardSystem.Instance?.RemoveDeckForEnemy(killEnemyGA.EnemyView);
    }

    private void AddCardsToHero(EnemyView enemy)
    {
        if(favorabilitySystem.GetFavorabilityState(enemy).CanPerformAction(EnemyActionType.OfferCard))
        {
            CardSystem.Instance.AddCardsToDeck(EnemyCardSystem.Instance.GetEnemyDeck(enemy));
        }
    }

    private FavorabilityState GetEnemyFavorabilityState(EnemyView enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("GetEnemyFavorabilityState: EnemyView is null!");
            return null;
        }

        var enemyState = favorabilitySystem.GetFavorabilityState(enemy);

        if (enemyState == null)
        {
            Debug.LogWarning("GetEnemyFavorabilityState: EnemyFavorabilityState is null!");
            favorabilitySystem.Setup(enemy, 0);
            enemyState = favorabilitySystem.GetFavorabilityState(enemy);

            if (enemyState == null)
            {
                Debug.LogError("EnemyFavorabilityState is still null after setup!");
                return null;
            }
        }
        return enemyState;
    }
}
