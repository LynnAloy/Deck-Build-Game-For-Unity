using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitEventSystem : Singleton<TraitEventSystem>
{
    public event EventHandler<TraitEventArgs> OnTraitTrigger;

    public class TraitEventArgs : EventArgs
    {
        public TraitData.FavorabilityEffect.TriggerType TriggerType { get; set; }
        public EnemyView Enemy { get; set; }
        public HeroView Hero { get; set; }
        public object AdditionalData { get; set; }
    }

    public void TriggerTraitEffect(TraitData.FavorabilityEffect.TriggerType triggerType, EnemyView enemy)
    {
        OnTraitTrigger?.Invoke(this, new TraitEventArgs
        {
            TriggerType = triggerType,
            Enemy = enemy
        });
    }

    public void TriggerTraitEffect(TraitData.FavorabilityEffect.TriggerType triggerType, EnemyView enemy, object additionalData = null)
    {
        OnTraitTrigger?.Invoke(this, new TraitEventArgs
        {
            TriggerType = triggerType,
            Enemy = enemy,
            AdditionalData = additionalData
        });
    }

    public void TriggerTraitEffect(TraitData.FavorabilityEffect.TriggerType triggerType, EnemyView enemy, HeroView hero, object additionalData = null)
    {
        OnTraitTrigger?.Invoke(this, new TraitEventArgs
        {
            TriggerType = triggerType,
            Enemy = enemy,
            Hero = hero,
            AdditionalData = additionalData
        });
    }

    public void TriggerTraitEffectForAllEnemies(TraitData.FavorabilityEffect.TriggerType triggerType)
    {
        if(TraitSystem.Instance == null)
        {
            Debug.LogError("TraitEventSystem.TriggerTrait: TraitSystem is null!");
            return;
        }
        /*
        var enemies = TraitSystem.Instance.GetEnemiesWithTraits();
        foreach(var enemy in enemies)
        {
            TriggerTraitEffect(triggerType, enemy);
        }
        */
    }
}
