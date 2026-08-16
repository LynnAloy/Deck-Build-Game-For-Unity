using System.Collections.Generic;
using UnityEngine;

public class Card 
{
    private readonly CardData data;

    public string ConfigId => data.ConfigId;
    public string Title => data.name;
    public string Description => data.Description;
    public Sprite Image => data.Image;

    public Effect ManualTargetEffect => data.ManualTargetEffect;
    public List<AutoTargetEffect> OtherEffects => data.OtherEffects;

    public int Mana
    {
        get
        {
            if (LuaConfigService.Instance == null)
            {
                return data.Mana;
            }
            return LuaConfigService.Instance.GetManaOrDefault(ConfigId, data.Mana);
        }
    }

    public Card(CardData cardData)
    {
        data = cardData;
    }
}
