using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualTargetSystem : Singleton<ManualTargetSystem>
{
    [SerializeField] private ArrowView arrowView;
    [SerializeField] private LayerMask targetLayerMask;
    public void StartTargeting(Vector3 startPosition)
    {
        Debug.Log($"开始手动目标选择，起始位置: {startPosition}");
        arrowView.gameObject.SetActive(true);
        arrowView.SetupArrow(startPosition);
    }

    public EnemyView EndTargeting(Vector3 endPosition)
    {
        Debug.Log($"结束手动目标选择，结束位置: {endPosition}");
        arrowView.gameObject.SetActive(false);
        /*
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.Log($"射线起点: {ray.origin}, 方向: {ray.direction}");
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, targetLayerMask)
            && hit.collider != null
            && hit.transform.TryGetComponent(out EnemyView enemyView))
        {
            Debug.Log($"命中目标: {enemyView.name} at position {hit.point}");
            return enemyView;
        }
        else
        {
            Debug.Log("[ManualTargetSystem] Raycast did not hit any target");
        }
        */

        if (Physics.Raycast(endPosition, Vector3.forward, out RaycastHit hit, 100f, targetLayerMask)
            && hit.collider != null
            && hit.transform.TryGetComponent(out EnemyView enemyView))
        {
            return enemyView;
        }
        else
        {
            Debug.Log("[ManualTargetSystem] Raycast did not hit anything");
        } 

        return null;
    }
}
