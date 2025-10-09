using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardsEffect : Effect
{
    [SerializeField] private int drawAmount;

    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        DrawCardGA drawCardGA = new(drawAmount);
        return drawCardGA;
    }
}
