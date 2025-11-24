using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyView : CombatantView
{
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private Transform traitPoint_1;
    [SerializeField] private Transform traitPoint_2;
    [SerializeField] private EnemyHandView enemyHandView;
    [SerializeField] private List<float> rendererWeight;
    [SerializeField] private Transform drawPilePoint;
    public Transform DrawPilePoint => drawPilePoint;
    public EnemyHandView EnemyHandView => enemyHandView;
    public Transform TraitPoint_1 => traitPoint_1;
    public Transform TraitPoint_2 => traitPoint_2;
    public int AttackPower { get; set; }
    public int Health { get; set; }

    public void Setup(EnemyData enemyData)
    {
        AttackPower = enemyData.AttackPower;
        UpdateAttackText();
        SetupBase(enemyData.Health, enemyData.Image, enemyData.BaseATK);
        //RandomRendererSetup(enemyData);
        //enemyHandView.AddCard(3);
    }

    /*
    private void RandomRendererSetup(EnemyData enemyData)
    {
        int rendererIndex = GetRandomRendererIndex(rendererWeight) + 1;
        if(rendererIndex == 1)
        {
            if(UnityEngine.Random.value < 0.5f)
            {
                enemyRenderer_1.sprite = enemyData.Properity_1;
                enemyRenderer_1.gameObject.SetActive(true);
                enemyRenderer_2.gameObject.SetActive(false);
            }
            else
            {
                enemyRenderer_2.sprite = enemyData.Properity_2;
                enemyRenderer_1.gameObject.SetActive(false);
                enemyRenderer_2.gameObject.SetActive(true);
            }
        }
        else
        {
            enemyRenderer_1.sprite = enemyData.Properity_1;
            enemyRenderer_2.sprite = enemyData.Properity_2;
            enemyRenderer_1.gameObject.SetActive(true);
            enemyRenderer_2.gameObject.SetActive(true);
        }
    }
    */

    private int GetRandomRendererIndex(List<float> rendererWeight)
    {
        float totalWeight = 0f;
        foreach (float weight in rendererWeight)
        {
            totalWeight += weight;
        }
        float randomValue = UnityEngine.Random.value * totalWeight;
        for(int i = 0; i < rendererWeight.Count; i++)
        {
            if(randomValue < rendererWeight[i])
            {
                return i;
            }
            randomValue -= rendererWeight[i];
        }
        return rendererWeight.Count - 1;
    }

    private void UpdateAttackText()
    {
        attackText.text = "ATK: " + AttackPower;
    }
}
