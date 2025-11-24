using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trait 
{
    private readonly TraitData data;

    public string Title => data.name;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public List<TraitData.FavorabilityEffect> FavorabilityEffects => data.FavorabilityEffects;
    public List<TraitData.StatusEffect> StatusEffects => data.StatusEffects;

    public Trait(TraitData traitData)
    {
        data = traitData;
    }
}
