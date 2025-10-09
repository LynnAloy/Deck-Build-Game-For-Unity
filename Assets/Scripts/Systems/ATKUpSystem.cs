using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATKUpSystem : MonoBehaviour
{
    [SerializeField] private GameObject atkUpVFX;
    [SerializeField] private int atkUpAmount;
    void OnEnable()
    {
        ActionSystem.AttachPerformer<ApplyATKUpGA>(ATKUpPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<ApplyATKUpGA>();
    }

    private IEnumerator ATKUpPerformer(ApplyATKUpGA atkUpGA)
    {
        CombatantView target = atkUpGA.Target;
        Instantiate(atkUpVFX, target.transform.position, Quaternion.identity);
        target.AddStatusEffect(StatusEffectType.ATKUP, atkUpGA.ATKUpAmount);
        target.RemoveStatusEffect(StatusEffectType.ATKUP, 1);
        yield return new WaitForSeconds(1f);
    }
}
