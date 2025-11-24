using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HeroView : CombatantView
{
    public void Setup(HeroData heroData)
    {
        SetupBase(heroData.Health, heroData.Image, heroData.BaseATK);
    }

    public IEnumerator Remove(HeroView heroView)
    {
        Tween tween = heroView.transform.DOScale(Vector3.zero, 0.25f);
        yield return tween.WaitForCompletion();
        DOTween.Kill(heroView.transform);
        Destroy(heroView.gameObject);
    }
    
}
