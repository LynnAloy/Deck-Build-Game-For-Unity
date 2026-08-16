using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageEffect : Effect
{
    [SerializeField] private int damageAmount;

    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster, string sourceConfigId = null)
    {
        DealDamageGA dealDamageGA = new(damageAmount, targets, caster, sourceConfigId);
        return dealDamageGA;
    }
}
