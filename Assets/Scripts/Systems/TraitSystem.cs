using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitSystem : Singleton<TraitSystem>
{
    [field: SerializeField] public List<TraitData> TraitDatas { get; private set; }
    [SerializeField] private TraitViewCreator traitViewCreator;
    private readonly Dictionary<EnemyView, List<Trait>> enemyTraits = new();
    private readonly Dictionary<EnemyView, List<TraitView>> enemyTraitViews = new();
    private readonly HashSet<EnemyView> enemiesWithTraits = new();

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyDrawTraitGA>(EnemyDrawTraitPerformer);
        Debug.Log("EnemyDrawTraitGA被使用");
        StartCoroutine(DelayedSubscribe());
        /*
        if(EnemyStatusSystem.Instance != null)
        {
            EnemyStatusSystem.Instance.OnStatusChanged += OnEnemyStatusChanged;
            Debug.Log("订阅EnemyStatusSystem的OnStatusChanged事件");
        }
        else
        {
            Debug.Log("EnemyStatusSystem.Instance为空，无法订阅OnStatusChanged事件");
        }
        */
    }

    private IEnumerator DelayedSubscribe()
    {
        // 等待一帧，确保其他单例已经初始化
        yield return null;

        if (EnemyStatusSystem.Instance != null)
        {
            EnemyStatusSystem.Instance.OnStatusChanged += OnEnemyStatusChanged;
            Debug.Log("订阅EnemyStatusSystem的OnStatusChanged事件");
        }
        else
        {
            Debug.LogError("EnemyStatusSystem.Instance仍为空，无法订阅OnStatusChanged事件");
        }
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyDrawTraitGA>();
        if(EnemyStatusSystem.Instance != null)
        {
            EnemyStatusSystem.Instance.OnStatusChanged -= OnEnemyStatusChanged;
        }
    }

    
    public IEnumerator EnemyDrawTraitPerformer(EnemyDrawTraitGA enemyDrawTraitGA)
    {
        if(enemyDrawTraitGA == null || enemyDrawTraitGA.EnemyView == null)
        {
            Debug.Log("enemyDrawTraitGA为空！");
            yield break;
        }

        EnemyView enemy = enemyDrawTraitGA.EnemyView;

        if(enemiesWithTraits.Contains(enemy))
        {
            Debug.Log("该敌人已拥有特质，跳过抽取特质步骤");
            yield break;
        }

        if (TraitDatas == null || TraitDatas.Count == 0)
        {
            Debug.Log("TraitDatas为空！");
            yield break;
        }

        if(!enemyTraits.ContainsKey(enemy))
        {
            enemyTraits[enemy] = new List<Trait>();
        }
        if(!enemyTraitViews.ContainsKey(enemy))
        {
            enemyTraitViews[enemy] = new List<TraitView>();
        }

        int count = UnityEngine.Random.Range(1, 3);

        List<int> chosenIndexes = new();
        for (int i = 0; i < count; i++)
        {
            int index;
            if(TraitDatas.Count <= count)
            {
                index = UnityEngine.Random.Range(0, TraitDatas.Count);
            }
            else
            {
                do
                {
                    index = UnityEngine.Random.Range(0, TraitDatas.Count);
                } while (chosenIndexes.Contains(index));
            }
            chosenIndexes.Add(index);
        }
        List<Transform> points = new();
        if(enemy.TraitPoint_1 != null) points.Add(enemy.TraitPoint_1);
        if(enemy.TraitPoint_2 != null) points.Add(enemy.TraitPoint_2);
        for(int i = 0; i < chosenIndexes.Count; i++)
        {
            int traitIndex = chosenIndexes[i];
            TraitData traitData = TraitDatas[traitIndex];
            Trait trait = new(traitData);
            enemyTraits[enemy].Add(trait);
            Transform parentPoint = points.Count > 0 ? points[i % points.Count] : null;
            Vector3 spawnPos = parentPoint != null ? parentPoint.position : Vector3.one;//
            Quaternion spawnRot = parentPoint != null ? parentPoint.rotation : Quaternion.identity;

            if(traitViewCreator != null)
            {
                TraitView traitView= traitViewCreator.CreateTraitView(trait, spawnPos, spawnRot);
                Debug.Log("Trait被实例化!");
                if(parentPoint != null)
                {
                    traitView.transform.SetParent(parentPoint, worldPositionStays: false);
                    traitView.transform.localPosition = Vector3.zero;
                }
                traitView.Setup(trait, enemy);
                SetTraitCoverVisibility(enemy, true);
                enemyTraitViews[enemy].Add(traitView);
                //Debug.Log($"{enemy}创建被添加trait!");
            }
            else
            {
                Debug.Log("TraitSystem:traitViewCreator为空，无法创建TraitView！");
            }
            yield return new WaitForSeconds(0.1f);
        }
        enemiesWithTraits.Add(enemy);
        yield return null;
    }

    public List<Trait> GetTraits(EnemyView enemy)
    {
        if(enemyTraits.TryGetValue(enemy, out var traits))
        {
            return traits;
        }
        return new List<Trait>();
    }

    public List<TraitView> GetTraitViews(EnemyView enemy)
    {
        if(enemyTraitViews.TryGetValue(enemy, out var traitViews))
        {
            return traitViews;
        }
        return new List<TraitView>();
    }

    public void SetTraitCoverVisibility(EnemyView enemy, bool isVisible)
    {
        //Debug.Log($"SetTraitCoverVisibility - 敌人: {enemy.name}, 显示Cover: {isVisible}");
        if (enemyTraitViews.TryGetValue(enemy, out var traitViews))
        {
            foreach(var traitView in traitViews)
            {
                traitView.Cover.SetActive(isVisible);
            }
        }
    }

    public void UpdateTraitClickability(EnemyView enemy)
    {
        if (enemyTraitViews.TryGetValue(enemy, out var traitViews))
        {
            if(FavorabilitySystem.Instance == null)
            {
                Debug.LogError("FavorabilitySystem.Instance 为 null");
                return;
            }
            int favorability = FavorabilitySystem.Instance.GetFavorabilityValue(enemy);
            //应当是enemyView而非traitView
            foreach (var traitView in traitViews)
            {
                traitView.OnFavorabilityValueChanged(enemy, favorability);
            }
        }
    }

    private void OnEnemyStatusChanged(object sender, EnemyStatusSystem.StatusChangeEventArgs e)
    {
        if (e == null)
        {
            Debug.LogError("StatusChangeEventArgs 为 null");
            return;
        }
        if (e.Enemy == null)
        {
            Debug.LogError("e.Enemy 为 null");
            return;
        }
        if (FavorabilitySystem.Instance == null)
        {
            Debug.LogError("FavorabilitySystem.Instance 为 null");
            return;
        }

        var traits = GetTraits(e.Enemy);
        if (traits == null)
        {
            Debug.LogWarning($"敌人 {e.Enemy.name} 没有 traits");
            return;
        }
        int count = 0;
        foreach (var trait in traits)
        {
            
            if (trait.FavorabilityEffects == null)
            {
                Debug.LogWarning($"特质 {trait} 的 FavorabilityEffects 为 null");
                continue;
            }
            
            foreach (var effect in trait.FavorabilityEffects)
            {
                bool shouldTrigger = e.StatusType switch
                {
                    StatusEffectType.BURN when effect.Trigger == TraitData.FavorabilityEffect.TriggerType.OnBurn => true,
                    _ => false
                };
                if(shouldTrigger)
                {
                    int favorabilityBefore = FavorabilitySystem.Instance.GetFavorabilityValue(e.Enemy);
                    Debug.Log($"TS {count} 特质触发前：{e.Enemy.name} 好感度为 {favorabilityBefore}");
                    FavorabilitySystem.Instance.ModifyFavorability(e.Enemy, effect.FavorabilityDelta);
                    int favorabilityAfter = FavorabilitySystem.Instance.GetFavorabilityValue(e.Enemy);
                    //Debug.Log($"TS {count} 特质触发后：{e.Enemy.name} 好感度变为 {favorabilityAfter}");
                    Debug.Log($"TS {count} 特质触发：{e.Enemy.name} 因{e.StatusType}效果获得{effect.FavorabilityDelta}好感度");
                }
                count++;
            }
        }
    }
}
