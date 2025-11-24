using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Trait")]

public class TraitData : ScriptableObject
{
    //[field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }
    [System.Serializable]
    public class FavorabilityEffect
    {
        public enum TriggerType { OnBurn, OnDamage, OnHeal }
        public TriggerType Trigger;
        //public int ThresholdValue;
        public int FavorabilityDelta;
    }

    [System.Serializable]
    public class StatusEffect
    {
        public enum EffectType { Attack, Defense, Health }
        public EffectType Type;
        public int Value;
        public StatusEffectType RequiredStatus;
        public int RequiredStacks;
    }

    [field: SerializeField] public List<FavorabilityEffect> FavorabilityEffects { get; private set; }
    [field: SerializeField] public List<StatusEffect> StatusEffects { get; private set; }
}
