using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformEffectGA : GameAction
{
    public Effect Effect { get; set; }
    public List<CombatantView> Targets { get; set; }
    public string SourceConfigId { get; }
    public PerformEffectGA(Effect effect, List<CombatantView> targets, string sourceConfigId)
    {
        Effect = effect;
        Targets = targets == null ? null : new(targets);
        SourceConfigId = sourceConfigId;
    }
}
