using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatusSystem : Singleton<EnemyStatusSystem>
{
    public class StatusChangeEventArgs : EventArgs
    {
        public EnemyView Enemy { get; }
        public StatusEffectType StatusType { get; }
        public int Stacks { get; }

        public StatusChangeEventArgs(EnemyView enemy, StatusEffectType statusEffect, int stacks)
        {
            Enemy = enemy;
            StatusType = statusEffect;
            Stacks = stacks;
        }
    }
    
    public event EventHandler<StatusChangeEventArgs> OnStatusChanged;

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<ApplyBurnGA>(OnBurnApplied, ReactionTiming.POST);
    }

        private void OnDisable()
        {
            ActionSystem.UnsubscribeReaction<ApplyBurnGA>(OnBurnApplied, ReactionTiming.POST);
        }

        private void OnBurnApplied(ApplyBurnGA burnGA)
        {
            if(burnGA.Target is EnemyView enemy)
            {
                OnStatusChanged?.Invoke(this, new StatusChangeEventArgs(
                    enemy, 
                    StatusEffectType.BURN, 
                    burnGA.BurnDamage
                ));
            }
        }
    
}
