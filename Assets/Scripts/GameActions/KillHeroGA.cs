using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillHeroGA : GameAction
{
    public HeroView HeroView { get; private set; }
    public KillHeroGA(HeroView heroView)
    {
        HeroView = heroView;
    }
}
