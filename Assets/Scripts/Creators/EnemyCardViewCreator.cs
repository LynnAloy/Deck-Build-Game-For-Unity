using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyCardViewCreator : Singleton<EnemyCardViewCreator>
{
    [SerializeField] private EnemyCardView cardViewPrefab;
    public EnemyCardView CreateCardView(Card card, Vector3 position, Quaternion rotation)
    {
        EnemyCardView enemyCardView = Instantiate(cardViewPrefab, position, rotation);
        enemyCardView.transform.localScale = new Vector3(0.8f, 0.8f, 1);
        enemyCardView.transform.DOScale(Vector3.one * 0.8f, 0.15f);
        enemyCardView.Setup(card);
        return enemyCardView;
    }
}
