using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyHandView : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    private readonly List<EnemyCardView> cards = new();
    public IEnumerator AddCard(EnemyCardView enemyCardView)
    {
        cards.Add(enemyCardView);
        yield return UpdateCardPosition(0.15f);
    }

    private IEnumerator UpdateCardPosition(float duration)
    {
        if (cards.Count == 0) yield break;
        float cardSpacing = 1f / 10f;
        float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < cards.Count; i++)
        {

            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            //Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);
            Quaternion rotation = Quaternion.LookRotation(forward, up);
            Vector3 angles = rotation.eulerAngles;
            angles.y = 0;
            Quaternion target = Quaternion.Euler(angles);
            
            cards[i].transform.DOMove(splinePosition + transform.position + 0.01f * i * Vector3.back, duration);
            cards[i].transform.DORotateQuaternion(target, duration);

        }
        yield return new WaitForSeconds(duration);
    }
}
