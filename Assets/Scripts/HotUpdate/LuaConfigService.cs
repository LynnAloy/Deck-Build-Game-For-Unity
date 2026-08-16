using System;
using System.IO;
using UnityEngine;
using XLua;

[DefaultExecutionOrder(-900)]
public sealed class LuaConfigService : MonoBehaviour
{
    public static LuaConfigService Instance { get; private set; }

    private LuaTable rootConfig;
    private LuaTable cardsConfig;

    public bool IsLoaded => rootConfig != null;
    public int LoadedVersion { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        Reload();
    }


    public bool Reload()
    {
        if (LuaRuntime.Instance == null || !LuaRuntime.Instance.IsInitialized)
        {
            Debug.LogError("[LuaConfigService] LuaRuntime is not initialized.");
            return false;
        }

        LuaTable candidateRoot = null;
        LuaTable candidateCards = null;

        try
        {
            LuaEnv env = LuaRuntime.Instance.Env;

            // require 会缓存模块。清除缓存后才能重新读取下载的新文件。
            env.DoString("package.loaded['skill'] = nil", "ClearSkillModuleCache");

            object[] results = env.DoString("return require('skill')", "LoadSkillConfig");

            if (results == null || results.Length == 0)
            {
                throw new InvalidDataException("skill.lua did not return a value.");
            }

            candidateRoot = results[0] as LuaTable;

            if (candidateRoot == null)
            {
                throw new InvalidDataException("skill.lua must return a Lua table.");
            }

            candidateCards = candidateRoot.Get<LuaTable>("cards");

            if (candidateCards == null)
            {
                throw new InvalidDataException("skill.lua does not contain a cards table.");
            }

            int candidateVersion = candidateRoot.Get<int>("version");

            ReleaseCurrentConfig();

            rootConfig = candidateRoot;
            cardsConfig = candidateCards;
            LoadedVersion = candidateVersion;

            candidateRoot = null;
            candidateCards = null;

            Debug.Log($"[LuaConfigService] Loaded skill config version {LoadedVersion}.");
            return true;
        }
        catch (Exception exception)
        {
            candidateCards?.Dispose();
            candidateRoot?.Dispose();

            Debug.LogWarning(
                $"[LuaConfigService] Failed to load skill.lua. " +
                $"ScriptableObject defaults will be used.\n{exception}"
            );

            return false;
        }
    }

    public int GetManaOrDefault(string configId, int defaultMana)
    {
        if (cardsConfig == null || string.IsNullOrWhiteSpace(configId))
        {
            return defaultMana;
        }

        LuaTable cardConfig = null;

        try
        {
            cardConfig = cardsConfig.Get<LuaTable>(configId);

            if (cardConfig == null)
            {
                return defaultMana;
            }

            object value = cardConfig.Get<object>("mana");

            if (value == null)
            {
                return defaultMana;
            }

            return Mathf.Max(0, Convert.ToInt32(value));
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"[LuaConfigService] Invalid mana for '{configId}'. " +
                $"Using default {defaultMana}.\n{exception.Message}"
            );

            return defaultMana;
        }
        finally
        {
            cardConfig?.Dispose();
        }
    }

    public int CalculateDamageOrDefault(string configId, int baseDamage, float targetHpPercent)
    {
        if (cardsConfig == null || string.IsNullOrWhiteSpace(configId))
        {
            return baseDamage;
        }

        LuaTable cardConfig = null;

        try
        {
            cardConfig = cardsConfig.Get<LuaTable>(configId);

            if (cardConfig == null)
            {
                return baseDamage;
            }

            CalculateDamageDelegate formula = cardConfig.Get<CalculateDamageDelegate>("CalculateDamage"); //Lua全局查找函数，返回一个委托

            if (formula == null)
            {
                return baseDamage;
            }

            double result = formula(baseDamage, Mathf.Clamp01(targetHpPercent));

            if (double.IsNaN(result) ||
                double.IsInfinity(result) ||
                result > int.MaxValue)
            {
                return baseDamage;
            }

            return Mathf.Max(0, Mathf.RoundToInt((float)result));
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"[LuaConfigService] Damage formula failed for '{configId}'. " +
                $"Using base damage {baseDamage}.\n{exception.Message}"
            );

            return baseDamage;
        }
        finally
        {
            cardConfig?.Dispose();
        }
    }

    private void ReleaseCurrentConfig()
    {
        cardsConfig?.Dispose(); //LuaTable底层是引用类型，Dispose会释放LuaTable的引用计数，避免内存泄漏
        rootConfig?.Dispose();

        cardsConfig = null;
        rootConfig = null;
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        if (LuaRuntime.Instance != null && LuaRuntime.Instance.IsInitialized)
        {
            ReleaseCurrentConfig();
        }

        Instance = null;
    }
}