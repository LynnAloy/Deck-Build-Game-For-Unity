using System;
using System.IO;
using UnityEngine;
using XLua;

[DefaultExecutionOrder(-1000)]
public sealed class LuaRuntime : MonoBehaviour
{
    public static LuaRuntime Instance { get; private set; } //单例模式

    public LuaEnv Env { get; private set; }

    public bool IsInitialized => Env != null;

    public string PersistentLuaDirectory => Path.Combine(Application.persistentDataPath, "Lua");

    public string BuiltInLuaDirectory => Path.Combine(Application.streamingAssetsPath, "Lua");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    public void Initialize()
    {
        if (Env != null)
        {
            return;
        }

        Directory.CreateDirectory(PersistentLuaDirectory);

        Env = new LuaEnv();
        Env.AddLoader(LoadLuaFile);

        Debug.Log($"[LuaRuntime] Initialized. Persistent Lua path: {PersistentLuaDirectory}");
    }

    private byte[] LoadLuaFile(ref string moduleName)
    {
        string relativePath = moduleName.Replace('.', Path.DirectorySeparatorChar) + ".lua";

        string persistentPath = Path.Combine(PersistentLuaDirectory, relativePath);

        if (TryReadFile(persistentPath, out byte[] persistentBytes))
        {
            moduleName = persistentPath;
            return persistentBytes;
        }

        string builtInPath = Path.Combine(BuiltInLuaDirectory, relativePath);

        if (TryReadFile(builtInPath, out byte[] builtInBytes))
        {
            moduleName = builtInPath;
            return builtInBytes;
        }

        return null;
    }

    private static bool TryReadFile(string path, out byte[] bytes)
    {
        bytes = null;

        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            bytes = File.ReadAllBytes(path);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[LuaRuntime] Failed to read Lua file: {path}\n{exception.Message}");
            return false;
        }
    }

    private void Update()
    {
        Env?.Tick();
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        Env?.Dispose();
        Env = null;
        Instance = null;
    }
}