using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageGA : GameAction, IHaveCaster
{
    public int Amount { get; private set; }
    public List<CombatantView> Targets { get; private set; }
    public CombatantView Caster { get; set; }
    public string SourceConfigId { get; }
    public DealDamageGA(int amount, List<CombatantView> targets, CombatantView caster, string sourceConfigId = null)
    {
        Amount = amount;
        Targets = new(targets);
        Caster = caster;
        SourceConfigId = sourceConfigId;

    }
}
