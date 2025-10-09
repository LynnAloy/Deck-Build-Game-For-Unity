using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCardSystem : MonoBehaviour
{
    //private DisCardAllCardGA disCardAllCardGA;
    [SerializeField] private HeroData heroData;
    [SerializeField] private CardSystem cardSystem;

    public void ResetCards()
    {
        cardSystem.IsGameOver = false;
        //StartCoroutine(DiscardAllCardsPerformer(disCardAllCardGA));
        cardSystem.ClearDiscardPile();
        cardSystem.ClearDrawPile();
        ActionSystem.Instance.Perform(new DrawCardGA(5));
        CardSystem.Instance.Setup(heroData.Deck);
    }
}
