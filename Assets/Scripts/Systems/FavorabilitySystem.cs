using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FavorabilityLevel
{
    Hostile,
    Neutral,
    Friendly
}

public class FavorabilitySystem : Singleton<FavorabilitySystem>
{
    private Dictionary<EnemyView, FavorabilityState> states = new();
    private Dictionary<EnemyView, int> favorabilityValues = new();

    public event Action<EnemyView> OnFavorabilityInitialized;
    public event Action<EnemyView, int> OnFavorabilityChanged;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<AttachFavorabilityGA>(AttachFavorabilityPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<AttachFavorabilityGA>();
    }


    private IEnumerator AttachFavorabilityPerformer(AttachFavorabilityGA attachFavorabilityGA)
    {
        var enemy = attachFavorabilityGA.Enemy;
        var initialFavorability = attachFavorabilityGA.InitialFavorability;

        if (enemy == null)
        {
            Debug.LogError("AttachFavorabilityGA: Enemy is null");
            yield break;
        }
        Setup(enemy, initialFavorability);
        // 确保特质系统也正确链接
        if (TraitSystem.Instance != null)
        {
            TraitSystem.Instance.UpdateTraitClickability(enemy);
        }

        OnFavorabilityInitialized?.Invoke(enemy);

        Debug.Log($"FavorabilitySystem attached to enemy: {enemy.name} with initial favorability: {initialFavorability}");
        yield return null;
    }

    public void Setup(EnemyView enemy, int initalFavorability)
    {
        favorabilityValues[enemy] = initalFavorability;
        states[enemy] = new HostileState(this, enemy);
        states[enemy].OnStateEnter();
    }

    public void ModifyFavorability(EnemyView enemy, int delta)
    {
        if (enemy == null)
        {
            Debug.LogError("ModifyFavorability: enemy 为 null");
            return;
        }

        bool needsInitialization = false;

        // 如果敌人不在字典中，先初始化
        if (!favorabilityValues.ContainsKey(enemy))
        {
            Debug.LogWarning($"敌人 {enemy.name} 不在好感度字典中，初始化为 0");
            favorabilityValues[enemy] = 0;
            needsInitialization = true;
        }

        Debug.Log($"FAVS 修改好感度 {enemy.name}：{delta}，修改前: {favorabilityValues[enemy]}");
        favorabilityValues[enemy] += delta;
        Debug.Log($"FAVS 修改后: {favorabilityValues[enemy]}");

        // 如果是新初始化的敌人，创建初始状态
        if (needsInitialization)
        {
            states[enemy] = new HostileState(this, enemy);
            states[enemy].OnStateEnter();
        }

        CheckStateTransition(enemy);
        OnFavorabilityChanged?.Invoke(enemy, favorabilityValues[enemy]);
    }

    private void CheckStateTransition(EnemyView enemy)
    {
        if(!states.ContainsKey(enemy))
            return;
        var currentState = states[enemy].CheckTransition();
        if(currentState.GetType() != states[enemy].GetType())
        {
            states[enemy].OnStateExit();
            states[enemy] = currentState;
            currentState.OnStateEnter();
        }
    }

    public int GetFavorabilityValue(EnemyView enemy)
    {
        return favorabilityValues.TryGetValue(enemy, out var value) ? value : 0;
    }

    public FavorabilityState GetFavorabilityState(EnemyView enemy)
    {
        return states.TryGetValue(enemy, out var state) ? state : null;
    }
}
