using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchEndSystem : MonoBehaviour
{
    [SerializeField] private EnemyBoardView enemyBoardView;
    [SerializeField] private HeroView heroView;
    [SerializeField] private float delayDuration;
    [SerializeField] private float endDelayDuration;
    [SerializeField] private GameObject endGameScreen;
    private List<EnemyView> Enemies => enemyBoardView.EnemyViews;
    private EnemyView enemyView;
    private CombatantView combatantView;
    private int maxMana;
    public bool delayCheck = false;
    public bool endCheck = false;

    void Update()
    {
        if(!delayCheck)
        {
            StartCoroutine(Delay());
            delayCheck = true;
        }
        else
        {
            if (IsGameOver() && !endCheck)
            {
                endCheck = true;
                Debug.Log("Game Over");
                HandleGameOver();
            }
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(delayDuration);
    }

    
    private IEnumerator EndDelay()
    {
        yield return new WaitForSeconds(endDelayDuration);
        EnableEndGameScreen();
        Time.timeScale = 0;
    }

    private void HandleGameOver()
    {
        bool hasEnemies = enemyBoardView != null &&
                   enemyBoardView.EnemyViews != null &&
                   enemyBoardView.EnemyViews.Count > 0;
        bool hasHero = heroView != null && heroView.CurrentHealth > 0;
        CardSystem.Instance.IsGameOver = true;
        if (!hasEnemies && hasHero)
        {
            Debug.Log("You Win!");
            RewardSystem.Instance.ApplyWin();
            StartCoroutine(EndDelay());
        }
        if (hasEnemies && !hasHero)
        {
            Debug.Log("You Lose!");
            RewardSystem.Instance.ApplyLose();
            StartCoroutine(EndDelay());
        }
        
    }

    private bool IsGameOver()
    {
        //Debug.Log("Over");
        bool hasEnemies = enemyBoardView != null &&
                  enemyBoardView.EnemyViews != null &&
                  enemyBoardView.EnemyViews.Count > 0;
        bool hasHero = heroView != null && heroView.CurrentHealth > 0;
        return !hasEnemies || !hasHero;
    }

    private void EnableEndGameScreen()
    {
        endGameScreen.SetActive(true);
    }
}