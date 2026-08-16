using System;
using System.Collections;
using System.Collections.Generic;
using CardGame.Extensions;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCardSystem : Singleton<EnemyCardSystem>
{
    private readonly Dictionary<EnemyView, EnemyDeckSet> enemyDecks = new();
    public class EnemyDeckSet
    {
        public readonly List<Card> DrawPile = new();
        public readonly List<Card> Hand = new();
        public EnemyHandView EnemyHandView;
        public Transform DrawPilePoint;
    }

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyDrawCardGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<EnemyPlayCardGA>(PlayCardPerformer);
        //Debug.Log("EnemyCardSystem OnEnable - Attach Performer for EnemyDrawCardGA");
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyDrawCardGA>();
        ActionSystem.DetachPerformer<EnemyPlayCardGA>();
    }

    public void Setup(List<CardData> deckData, EnemyView enemyView)
    {
        if(enemyView == null)
        {
            Debug.Log("enemyView为空");
            return;
        }
        var enemyDeckSet = new EnemyDeckSet();
        enemyDeckSet.DrawPilePoint = enemyView.DrawPilePoint;
        if (!enemyDeckSet.DrawPilePoint)
        {
            Debug.Log("drawPilePoint不存在");
        }
        enemyDeckSet.EnemyHandView = enemyView.EnemyHandView;
        if (!enemyDeckSet.EnemyHandView)
        {
            Debug.Log("enemyHandView不存在");
        }
        enemyDeckSet.DrawPile.Clear();
        enemyDeckSet.Hand.Clear();
        foreach (var cardData in deckData)
        {
            if (cardData == null)
            {
                Debug.LogError("尝试添加空卡牌数据到卡组！");
                continue;
            }
            enemyDeckSet.DrawPile.Add(new Card(cardData));
        }
        Debug.Log($"卡组初始化完成，卡牌数量: {enemyDeckSet.DrawPile.Count}");
        enemyDecks[enemyView] = enemyDeckSet;

    }

    public IEnumerator DrawCardsPerformer(EnemyDrawCardGA drawCardGA)
    {
        if(drawCardGA == null)
        {
            Debug.Log("drawCardGA为空！");
            yield break;
        }
        EnemyView target = drawCardGA.EnemyView;
        if(target == null)
        {
            Debug.Log("目标敌人为空！");
            yield break;
        }
        if(!enemyDecks.TryGetValue(target, out var enemyDeckSet))
        {
            Debug.Log($"EnemyCardSystem:未为目标Enemy({target.name})初始化!");
            yield break;
        }
        int actualAmount = Mathf.Min(drawCardGA.Amount, enemyDeckSet.DrawPile.Count);
        for (int i = 0; i < actualAmount; i++)
        {
            //Debug.Log("确认DrawCard执行");
            yield return DrawCard(enemyDeckSet);
        }
    }

    private IEnumerator DrawCard(EnemyDeckSet enemyDeckSet)
    {
        Debug.Log($"开始抽卡，当前抽牌堆数量: {enemyDeckSet.DrawPile.Count}");
        if (enemyDeckSet.DrawPile.Count == 0)
        {
            Debug.Log("敌人抽牌堆为空！");
        }
        Card card = enemyDeckSet.DrawPile.Draw();
        //Debug.Log($"抽到卡牌: {card?.Title ?? "null"}");
        if (card == null)
        {
            Debug.Log("敌人抽取到空牌！");
            yield break;
        }
        enemyDeckSet.Hand.Add(card);
        Vector3 spawnPoint = enemyDeckSet.DrawPilePoint != null ? enemyDeckSet.DrawPilePoint.position : Vector3.zero;
        //Quaternion spawnRot = enemyDeckSet.DrawPilePoint != null ? enemyDeckSet.DrawPilePoint.rotation : Quaternion.identity;
        EnemyCardView enemyCardView = EnemyCardViewCreator.Instance.CreateCardView(card, spawnPoint, Quaternion.identity);
        if (enemyCardView == null)
        {
            Debug.LogError("创建敌人卡牌视图失败！");
            yield break;
        }
        if (enemyDeckSet.EnemyHandView != null)
        {
            enemyCardView.transform.SetParent(enemyDeckSet.DrawPilePoint.transform, worldPositionStays: false);
            enemyCardView.transform.localEulerAngles = Vector3.zero;
            enemyCardView.transform.rotation = Quaternion.identity;
            enemyCardView.transform.localScale = Vector3.one;
            //Debug.Log(enemyCardView.transform.rotation);
            yield return enemyDeckSet.EnemyHandView.AddCard(enemyCardView);
        }
        else
        {
            Debug.Log("EnemyCardSystem:HandVew为空");
            Destroy(enemyCardView.gameObject);
            yield break;
        }
    }

    private IEnumerator PlayCardPerformer(EnemyPlayCardGA playCardGA)
    {
        foreach(var effectWrapper in playCardGA.Card.OtherEffects)
        {
            List<CombatantView> targets = effectWrapper.TargetMode.GetTargets();
            PerformEffectGA performEffectGA = new(effectWrapper.Effect, targets, playCardGA.Card.ConfigId);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
        yield return null;
    }

    public IEnumerator EnemyPlayCard(EnemyView enemy)
    {
        if(enemy == null)
        {
            yield break;
        }
        if (!enemyDecks.TryGetValue(enemy, out var deckSet))
        {
            Debug.Log($"EnemyCardSystem:未为({enemy.name})找到对应卡组数据!");
            yield break;
        }
        if(deckSet.Hand.Count == 0)
        {
            Debug.Log("敌人手牌为空，无法出牌！");
            yield break;
        }
        int index = UnityEngine.Random.Range(0, deckSet.Hand.Count);
        Card cardToPlay = deckSet.Hand[index];
        foreach(var effectWrapper in cardToPlay.OtherEffects)
        {
            List<CombatantView> targets = effectWrapper.TargetMode.GetTargets();
            Debug.Log($"敌人使用卡牌: {cardToPlay.Title}");
            PerformEffectGA performEffectGA = new(effectWrapper.Effect, targets, cardToPlay.ConfigId);
            ActionSystem.Instance.AddReaction(performEffectGA);
            yield return new WaitForSeconds(0.15f);
        }
        yield return null;
    }

    public void RemoveDeckForEnemy(EnemyView enemyView)
    {
        if (enemyView == null)
        {
            return;
        }
        enemyDecks.Remove(enemyView);
    }

    public List<Card> GetEnemyDeck(EnemyView enemy)
    {
        if(enemy == null)
        {
            Debug.LogError("GetEnemyDeck: EnemyView is null");
        }
        return enemyDecks.TryGetValue(enemy, out var enemyDeckSet) ? enemyDeckSet.Hand : null;
    }

    public void ConvertTargetMode(List<Card> deck)
    {
        foreach(var card in deck)
        {
            foreach(var otherEffect in card.OtherEffects)
            {
                if(otherEffect == null)
                {
                    Debug.LogError("ConvertTargetMode: OtherEffect is null");
                    return;
                }
                var targetMode = otherEffect.TargetMode;
                if (targetMode == null)
                {
                    Debug.LogError("ConvertTargetMode: TargetMode is null");
                    return;
                }
                if (targetMode is HeroTM)
                {
                    otherEffect.TargetMode = new RandomEnemyTM();
                }
                else if (targetMode is RandomEnemyTM)
                {
                    otherEffect.TargetMode = new HeroTM();
                }
                else if (targetMode is AllEnemyTM)
                {
                    otherEffect.TargetMode = new HeroTM();
                }
                else
                {
                    Debug.LogWarning("ConvertTargetMode: Unknown TargetMode type");
                }
            }
            
        }
    }
}
