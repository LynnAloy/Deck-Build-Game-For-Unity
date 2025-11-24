using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class TraitView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject cover;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private LayerMask clickLayer;

    private BoxCollider traitCollider;
    private EnemyView owner;
    private bool isClickable = false;
    public Trait Trait { get; private set; }
    public GameObject Cover => cover;

    private void Start()
    {
        traitCollider = GetComponent<BoxCollider>();
        if (traitCollider == null)
        {
            Debug.LogWarning($"TraitView {Trait?.Title} 缺少 BoxCollider 组件，添加一个");
            //traitCollider = gameObject.AddComponent<BoxCollider>();
        }
        else
        {
            Debug.Log($"TraitView {Trait?.Title} 已有 BoxCollider 组件");
        }
        if (cover != null)
        {
            Collider coverCollider = cover.GetComponent<BoxCollider>();
            if (coverCollider != null)
            {
                coverCollider.isTrigger = true; // 设置为 Trigger，不阻挡鼠标事件
                Debug.Log($"Cover 的 Collider 已设置为 Trigger");
            }
        }
    }

    public void Setup(Trait trait)
    {
        if (trait == null)
        {
            Debug.LogError("尝试设置空特质视图！");
            return;
        }
        Trait = trait;
        if (title != null) title.text = trait.Title ?? "";
        if (description != null) description.text = trait.Description ?? "";
        if (imageSR != null) imageSR.sprite = trait.Image;

        Debug.Log($"Setup(trait) 完成，等待 Setup(trait, enemyOwner) 调用");
    }

    public void Setup(Trait trait, EnemyView enemyOwner)
    {
        if (trait == null)
        {
            Debug.LogError("尝试设置空特质视图！");
            return;
        }
        Trait = trait;
        owner = enemyOwner;
        if (title != null) title.text = trait.Title ?? "";
        if (description != null) description.text = trait.Description ?? "";
        if (imageSR != null) imageSR.sprite = trait.Image;
        if (FavorabilitySystem.Instance != null)
        {
            // 订阅初始化事件
            FavorabilitySystem.Instance.OnFavorabilityInitialized += OnFavorabilitySystemInitialized;

            // 订阅好感度改变事件
            FavorabilitySystem.Instance.OnFavorabilityChanged += OnFavorabilityValueChanged;

            // 如果已经初始化，立即更新
            if (FavorabilitySystem.Instance.GetFavorabilityValue(owner) >= 0)
            {
                Debug.Log($"好感度系统已初始化，立即更新 {Trait.Title} 的可点击状态");
                UpdateClickability();
            }
            else
            {
                Debug.Log($"等待好感度系统初始化 {Trait.Title}");
            }
        }
        else
        {
            Debug.Log($"等待好感度系统初始化 {Trait.Title}");
            // 订阅初始化完成事件
            //FavorabilitySystem.Instance.OnFavorabilityInitialized += OnFavorabilitySystemInitialized;
        }
    }

    private void OnFavorabilitySystemInitialized(EnemyView enemy)
    {
        if (enemy == owner)
        {
            Debug.Log($"好感度系统初始化完成，更新 {Trait.Title} 的可点击状态");
            UpdateClickability();
            // 取消订阅事件
            //FavorabilitySystem.Instance.OnFavorabilityInitialized -= OnFavorabilitySystemInitialized;
        }
    }

    public void OnFavorabilityValueChanged(EnemyView enemy, int newFavorability)
    {
        if (enemy == owner)
        {
            Debug.Log($"检测到 {Trait.Title} 所属敌人 {enemy.name} 的好感度变为 {newFavorability}，更新可点击状态");
            UpdateClickability();
        }
    }

    private void UpdateClickability()
    {
        Debug.Log("UpdateClickability called");
        if (owner == null || FavorabilitySystem.Instance == null)
        {
            Debug.Log("UpdateClickability: owner 或 FavorabilitySystem.Instance 为空");
            isClickable = false;
            return;
        }
        else
        {
            Debug.Log("owner 和 FavorabilitySystem.Instance 均不为空");
        }
        int favorability = FavorabilitySystem.Instance.GetFavorabilityValue(owner);
        bool wasClickable = isClickable;
        isClickable = favorability >= 31;
        if (wasClickable != isClickable)
        {
            Debug.Log($"UpdateClickability - 敌人: {owner.name}, 好感度: {favorability}, 可点击: {isClickable}");
        }
        else
        {
            Debug.Log("UpdateClickability: 可点击状态未改变");
        }
        
        if(cover != null)
        {
            cover.SetActive(!isClickable);
        }
        
    }

    /*
    public void OnMouseDown()
    {
        Debug.Log("TraitView OnMouseDown called");
        //UpdateClickability();

        Debug.Log($"=== TraitView 点击开始 ===");
        Debug.Log($"TraitView 被点击: {Trait?.Title}");

        if (owner == null)
        {
            Debug.Log("owner 为空");
            return;
        }

        if (FavorabilitySystem.Instance == null)
        {
            Debug.Log("FavorabilitySystem.Instance 为空");
            return;
        }

        int favorability = FavorabilitySystem.Instance.GetFavorabilityValue(owner);
        var state = FavorabilitySystem.Instance.GetFavorabilityState(owner);

        Debug.Log($"敌人好感度: {favorability}");
        Debug.Log($"敌人状态: {state?.GetType().Name ?? "null"}");
        Debug.Log($"可点击状态: {isClickable}");
        Debug.Log($"Cover 状态: {cover?.activeSelf}");
        Debug.Log($"owner: {owner.name}");

        if (!isClickable)
        {
            Debug.Log("不可点击，退出点击处理");
            return;
        }

        Debug.Log("可以点击，处理点击逻辑");

        if (cover != null && cover.activeSelf)
        {
            cover.SetActive(false);
            Debug.Log("Cover 被设置为 false");
        }
        else
        {
            Debug.Log("Cover 为 null 或已经处于非激活状态");
        }

        Debug.Log($"=== TraitView 点击结束 ===");
    }
    */

    /*
    public void OnFavorabilityChanged()
    {
        UpdateClickability();

        if (owner != null && TraitSystem.Instance != null)
        {
            if (!isClickable && cover != null)
            {
                cover.SetActive(true);
            }
            else if (isClickable && cover != null)
            {
                cover.SetActive(false);
            }
        }
    }
    */

    private void OnDestory()
    {
        if(FavorabilitySystem.Instance !=null)
        {
            FavorabilitySystem.Instance.OnFavorabilityInitialized -= OnFavorabilitySystemInitialized;
        }
    }
}
