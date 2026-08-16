using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextRoundButtonUI : MonoBehaviour
{
    [SerializeField] private GameObject endGameScreen;
    [SerializeField] private ResetHeroSystem resetHeroSystem;
    [SerializeField] private ResetCardSystem resetCardSystem;
    [SerializeField] private ResetEnemySystem resetEnemySystem;
    [SerializeField] private MatchEndSystem matchEndSystem;
    public void OnClick()
    {
        endGameScreen.SetActive(false);
        Time.timeScale = 1;
        resetCardSystem.ResetCards();
        resetHeroSystem.ResetHero();
        resetEnemySystem.ResetEnemy();
        matchEndSystem.ResetState();
    }
}
