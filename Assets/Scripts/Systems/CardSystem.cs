using System;
using System.Collections;
using System.Collections.Generic;
using CardGame.Extensions;
using DG.Tweening;
using UnityEngine;

public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;
    protected readonly List<Card> drawPile = new();
    protected readonly List<Card> discardPile = new();
    protected readonly List<Card> hand = new();
    public bool IsGameOver { get; set; } = false;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DisCardAllCardGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardGA>();
        ActionSystem.DetachPerformer<DisCardAllCardGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
        
    }

    public void Setup(List<CardData> deckData)
    {
        foreach (var cardData in deckData)
        {
            if(cardData == null)
            {
                Debug.LogError("尝试添加空卡牌数据到卡组！");
                continue;
            }
            Card card = new(cardData);
            drawPile.Add(card);
        }
        Debug.Log($"卡组初始化完成，卡牌数量: {drawPile.Count}");
    }

    private IEnumerator DrawCardsPerformer(DrawCardGA drawCardGA)
    {
        int actualAmount = Mathf.Min(drawCardGA.Amount, drawPile.Count);
        int notDrawnAmount = drawCardGA.Amount - actualAmount;
        for (int i = 0; i < actualAmount; i++)
        {
            yield return DrawCard();
        }
        if(notDrawnAmount > 0)
        {
            RefillDeck();
            for(int i = 0; i < notDrawnAmount; i++)
            {
                yield return DrawCard();
            }
        }
    }

    protected IEnumerator DiscardAllCardsPerformer(DisCardAllCardGA discardAllCardsGA)
    {
        foreach(var card in hand)
        {
            CardView cardView = handView.RemoveCard(card);
            yield return DiscardCard(cardView);
        }
        hand.Clear();
    }

    public IEnumerator DrawCard()
    {
        if(IsGameOver)
        {
            yield break;
        }
        if (drawPile.Count == 0)
        {
            Debug.LogWarning("抽牌堆已空，无法抽牌！");
            yield break;
        }
        Card card = drawPile.Draw();
        if (card == null)
        {
            Debug.LogError("抽牌时得到空卡牌！");
            yield break;
        }
        hand.Add(card);
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);
        yield return handView.AddCard(cardView);
    }

    private void RefillDeck()
    {
        if (discardPile.Count == 0)
        {
            Debug.LogWarning("弃牌堆已空，无法补充抽牌堆！");
            return;
        }
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        Debug.Log("抽牌堆已补充，当前卡牌数量: " + drawPile.Count);
    }

    private IEnumerator DiscardCard(CardView cardView)
    {
        discardPile.Add(cardView.Card);
        cardView.transform.DOScale(Vector3.zero, 0.15f);
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);
    }

    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        hand.Remove(playCardGA.Card);
        CardView cardView = handView.RemoveCard(playCardGA.Card);
        yield return DiscardCard(cardView); 
        SpendManaGA spendManaGA = new(playCardGA.Card.Mana);
        ActionSystem.Instance.AddReaction(spendManaGA);
        if(playCardGA.Card.ManualTargetEffect != null)
        {
            PerformEffectGA performEffectGA = new(playCardGA.Card.ManualTargetEffect, new() { playCardGA.ManualTarget });
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
        foreach (var effectWrapper in playCardGA.Card.OtherEffects)
        {
            List<CombatantView> targets = effectWrapper.TargetMode.GetTargets();
            PerformEffectGA performEffectGA = new(effectWrapper.Effect, targets);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
    }

    public void ClearDrawPile()
    {
        drawPile.Clear();
    }

    public void ClearDiscardPile()
    {
        discardPile.Clear();
    }

}

