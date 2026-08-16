using System.Collections;
using UnityEngine;

public class MatchEndSystem : MonoBehaviour
{
    [SerializeField] private MatchSetupSystem matchSetupSystem;
    [SerializeField] private EnemyBoardView enemyBoardView;
    [SerializeField] private HeroView heroView;
    [SerializeField] private float delayDuration;
    [SerializeField] private float endDelayDuration;
    [SerializeField] private GameObject endGameScreen;

    private Coroutine pendingGameOverCheck;
    private bool endCheck;

    private void Update()
    {
        if (matchSetupSystem == null || !matchSetupSystem.IsInitialized)
        {
            return;
        }

        if (endCheck || pendingGameOverCheck != null)
        {
            return;
        }

        if (IsGameOver())
        {
            pendingGameOverCheck = StartCoroutine(ConfirmGameOver());
        }
    }

    private IEnumerator ConfirmGameOver()
    {
        yield return new WaitForSeconds(delayDuration);
        pendingGameOverCheck = null;

        if (endCheck || !IsGameOver())
        {
            yield break;
        }

        endCheck = true;
        Debug.Log("Game Over");
        HandleGameOver();
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
        else if (hasEnemies && !hasHero)
        {
            Debug.Log("You Lose!");
            RewardSystem.Instance.ApplyLose();
            StartCoroutine(EndDelay());
        }
    }

    private bool IsGameOver()
    {
        bool hasEnemies = enemyBoardView != null &&
                          enemyBoardView.EnemyViews != null &&
                          enemyBoardView.EnemyViews.Count > 0;

        bool hasHero = heroView != null && heroView.CurrentHealth > 0;
        return !hasEnemies || !hasHero;
    }

    private IEnumerator EndDelay()
    {
        yield return new WaitForSeconds(endDelayDuration);
        EnableEndGameScreen();
        Time.timeScale = 0f;
    }

    private void EnableEndGameScreen()
    {
        endGameScreen.SetActive(true);
    }

    public void ResetState()
    {
        if (pendingGameOverCheck != null)
        {
            StopCoroutine(pendingGameOverCheck);
            pendingGameOverCheck = null;
        }

        endCheck = false;
    }
}