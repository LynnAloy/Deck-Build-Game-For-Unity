using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyBoardView : MonoBehaviour
{
    [SerializeField] private List<Transform> slots;
    public List<EnemyView> EnemyViews { get; private set; } = new();

    public void AddEnemy(EnemyData enemyData)
    {
        Transform slot = slots[EnemyViews.Count];
        EnemyView enemyView = EnemyViewCreator.Instance.CreateEnemyView(enemyData, slot.position, slot.rotation);
        enemyView.transform.parent = slot;
        EnemyViews.Add(enemyView);
    }
    public IEnumerator RemoveEnemy(EnemyView enemyView)
    {
        EnemyViews.Remove(enemyView);
        Tween tween = enemyView.transform.DOScale(Vector3.zero, 0.25f);
        yield return tween.WaitForCompletion();
        //Added
        DOTween.Kill(enemyView.transform);
        Destroy(enemyView.gameObject);
    }

    /*
    public void RemoveAllEnemies()
    {
        foreach(var enemyView in EnemyViews)
        {
            EnemyViews.Remove(enemyView);
            Destroy(enemyView.gameObject);
        }
        
    }
    */

    public void RemoveAllEnemies()
    {
        for (int i = EnemyViews.Count - 1; i >= 0; i--)
        {
            var enemyView = EnemyViews[i];
            DOTween.Kill(enemyView.transform);
            EnemyViews.RemoveAt(i);
            Destroy(enemyView.gameObject);
        }
    }
}
