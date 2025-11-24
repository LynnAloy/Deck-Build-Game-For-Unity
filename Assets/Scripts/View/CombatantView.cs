using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CombatantView : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    //[SerializeField] private TMP_Text atkText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private StatusEffectsUI statusEffectsUI;
    public int MaxHealth { get; private set; }
    public int BaseATK { get; private set; }
    public int CurrentHealth { get; set; }
    //public int CurrentATK { get; private set; }
    private Dictionary<StatusEffectType, int> statusEffects = new();

    protected void SetupBase(int health, Sprite image,int baseATK) 
    {
        MaxHealth = CurrentHealth = health;
        BaseATK = baseATK;
        spriteRenderer.sprite = image;
        UpdateHealthText();
        //UpdateATKText();
    }

    //Overload SetupBase for enemies with a cardview and two special characters

    public int CheckATKStauts()
    {
        int atkUpAmount = GetStatusEffectStacks(StatusEffectType.ATKUP);
        return atkUpAmount > 0 ? BaseATK + atkUpAmount : BaseATK;
    }
    protected void UpdateHealthText()
    {
        healthText.text = "HP: " + CurrentHealth; 
    }

    /*
    private void UpdateATKText()
    {
        atkText.text = "ATK: " + CurrentATK;
    }
    */

    public void Damage(int damageAmount)
    {
        int remainingDamage = damageAmount;
        int currentArmor = GetStatusEffectStacks(StatusEffectType.ARMOR);
        if(currentArmor > 0)
        {
            if(currentArmor >= damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, remainingDamage);
                remainingDamage = 0;
            }
            else if(currentArmor < damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, currentArmor);
                remainingDamage -= currentArmor;
            }
        }
        if(remainingDamage > 0)
        {
            CurrentHealth -= remainingDamage;
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }
        }
        transform.DOShakePosition(0.5f, 0.25f);
        UpdateHealthText();
    }

    public void AddStatusEffect(StatusEffectType type, int stackCount)
    {
        if(statusEffects.ContainsKey(type))
        {
            statusEffects[type] += stackCount;
        }
        else
        {
            statusEffects.Add(type, stackCount);
        }
        statusEffectsUI.UpdateStatusEffectUI(type, GetStatusEffectStacks(type));
    }

    public void RemoveStatusEffect(StatusEffectType type, int stackCount)
    {
        if(statusEffects.ContainsKey(type))
        {
            statusEffects[type] -= stackCount;
            if(statusEffects[type] <= 0)
            {
                statusEffects.Remove(type);
            }
            statusEffectsUI.UpdateStatusEffectUI(type, GetStatusEffectStacks(type)); 
        }
    }

    public int GetStatusEffectStacks(StatusEffectType type)
    {
        if (statusEffects.ContainsKey(type))
        {
            return statusEffects[type];
        }
        return 0;
    }
}
