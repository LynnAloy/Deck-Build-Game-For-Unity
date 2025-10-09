using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetEnemySystem : MonoBehaviour
{
    [SerializeField] private List<EnemyData> enemyDatas;
    [SerializeField] private EnemySystem enemySystem;
    [SerializeField] private EnemyBoardView enemyBoardView;

    public void ResetEnemy()
    {
        Debug.Log("Reset Enemy");
        enemyBoardView.RemoveAllEnemies();
        enemySystem.Setup(enemyDatas);
    }
}
