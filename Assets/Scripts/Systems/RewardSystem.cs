using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSystem : Singleton<RewardSystem>
{
    private int maxMana;
    private CombatantView combatantView;
    private List<EnemyView> Enemies => enemyBoardView.EnemyViews;
    [SerializeField] private EnemyBoardView enemyBoardView;
    [SerializeField] private HeroView heroView;
    [field: SerializeField] public float RewardScale { get; private set; } = 0.1f;

    private new void Awake()
    {
        base.Awake();
        StartCoroutine(InitializeAfterManaSystem());
    }

    private IEnumerator InitializeAfterManaSystem()
    {
        yield return null;
        while(ManaSystem.Instance == null)
        {
            yield return null;
        }
        yield return maxMana = ManaSystem.Instance.MaxMana;
    }

    public void ApplyWin()
    {
        Debug.Log("Apply Win Reward");
        //maxMana += (int)(heroView.CurrentHealth * RewardScale);
        ManaSystem.Instance.AddMaxMana((int)(heroView.CurrentHealth * RewardScale));
    }

    public void ApplyLose()
    {
        Debug.Log("Apply Lose Penalty");
        int remainHealthOfEnemies = 0;
        foreach (var enemy in Enemies)
        {
            remainHealthOfEnemies += enemy.CurrentHealth;
        }
        if (remainHealthOfEnemies > 50)
        {
            remainHealthOfEnemies = 50;
        }
        maxMana -= (int)(remainHealthOfEnemies * RewardScale);
    }
}
