/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<DrawCardGA>(DrawCardReaction, ReactionTiming.POST);
    }
    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<DrawCardGA>(DrawCardReaction, ReactionTiming.POST);
    }

    private void DrawCardReaction(DrawCardGA drawCardGA)
    {
        DealDamageGA dealDamageGA = new(3);
        ActionSystem.Instance.AddReaction(dealDamageGA);
    }


}
*/
