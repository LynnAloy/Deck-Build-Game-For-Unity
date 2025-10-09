using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllEnemyTM : TargetMode
{
    public override List<CombatantView> GetTargets()
    {
        return new(EnemySystem.Instance.Enemies);
    }
}
