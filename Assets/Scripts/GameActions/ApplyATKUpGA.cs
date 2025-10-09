using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyATKUpGA : GameAction
{
    public int ATKUpAmount { get; private set; }
    public CombatantView Target { get; private set; }
    public ApplyATKUpGA(int atkUpAmount, CombatantView target)
    {
        ATKUpAmount = atkUpAmount;
        Target = target;
    }
}
