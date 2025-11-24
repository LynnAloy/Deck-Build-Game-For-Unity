using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayCardGA : GameAction
{
    public Card Card {get; set;}

    public EnemyPlayCardGA(Card card)
    {
        Card = card;
    }
}
