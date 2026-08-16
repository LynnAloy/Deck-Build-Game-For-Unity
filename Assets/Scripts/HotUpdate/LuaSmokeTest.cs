using UnityEngine;
using XLua;

public sealed class LuaSmokeTest : MonoBehaviour
{
    private LuaEnv luaEnv;

    private void Start()
    {
        luaEnv = new LuaEnv();

        luaEnv.DoString("CS.UnityEngine.Debug.Log('[xLua] Smoke test passed. Version: ' .. _VERSION)");
    }

    private void OnDestroy()
    {
        luaEnv?.Dispose();
        luaEnv = null;
    }
}