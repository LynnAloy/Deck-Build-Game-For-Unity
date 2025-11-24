using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TraitViewCreator : MonoBehaviour
{
    [SerializeField] private TraitView traitViewPrefab;
    
    public TraitView CreateTraitView(Trait trait, Vector3 position, Quaternion rotation)
    {
        TraitView traitView = Instantiate(traitViewPrefab, position, rotation);
        //traitView.transform.localPosition = Vector3.zero;
        traitView.transform.DOScale(Vector3.one * 0.6f, 0.15f);
        traitView.Setup(trait);
        return traitView;
    }
}
