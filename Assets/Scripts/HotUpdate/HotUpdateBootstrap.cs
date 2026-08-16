using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-800)]
public sealed class HotUpdateBootstrap : MonoBehaviour
{
    public static HotUpdateBootstrap Instance { get; private set; }

    [SerializeField] private LuaUpdateService updateService;

    public bool IsReady { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        if (updateService == null)
        {
            updateService = GetComponent<LuaUpdateService>();
        }
    }

    private IEnumerator Start()
    {
        if (updateService != null)
        {
            yield return updateService.CheckAndUpdate();
        }
        else
        {
            Debug.LogWarning("[HotUpdateBootstrap] LuaUpdateService is missing.");
        }

        IsReady = true;
        Debug.Log("[HotUpdateBootstrap] Startup completed.");
    }

    public IEnumerator WaitUntilReady()
    {
        while (!IsReady)
        {
            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}