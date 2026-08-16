using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [SerializeField] private GameObject damageVFX;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
    }

    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        foreach (var target in dealDamageGA.Targets)
        {
            float targetHpPercent = target.MaxHealth > 0 ? (float)target.CurrentHealth / target.MaxHealth : 1f;
            int finalDamage = dealDamageGA.Amount;
            if (LuaConfigService.Instance != null)
            {
                finalDamage = LuaConfigService.Instance.CalculateDamageOrDefault(
                    dealDamageGA.SourceConfigId,
                    dealDamageGA.Amount,
                    targetHpPercent
                );
            }
            Debug.Log(
                $"[DamageSystem] id={dealDamageGA.SourceConfigId}, " +
                $"base={dealDamageGA.Amount}, final={finalDamage}, " +
                $"targetHp={targetHpPercent:P0}"
            );
            target.Damage(finalDamage);
            Instantiate(damageVFX, target.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.25f);
            if(target.CurrentHealth <= 0)
            {
                if(target is EnemyView enemyView)
                {
                    KillEnemyGA killenemyGA = new(enemyView);
                    ActionSystem.Instance.AddReaction(killenemyGA);
                }
                // Handle hero death if target is hero 
            }
        }
    }
}

