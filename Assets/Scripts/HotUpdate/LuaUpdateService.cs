using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using XLua;

public sealed class LuaUpdateService : MonoBehaviour
{
    [SerializeField] private string manifestUrl = "http://127.0.0.1:8000/version.json";
    [SerializeField, Min(1)] private int timeoutSeconds = 10;

    public IEnumerator CheckAndUpdate()
    {
        if (LuaRuntime.Instance == null || LuaConfigService.Instance == null)
        {
            Debug.LogError("[LuaUpdateService] Required Lua services are missing.");
            yield break;
        }

        LuaVersionManifest manifest;

        using (UnityWebRequest request = UnityWebRequest.Get(manifestUrl))
        {
            request.timeout = timeoutSeconds;
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[LuaUpdateService] Manifest request failed: {request.error}");
                yield break;
            }

            try
            {
                manifest = JsonUtility.FromJson<LuaVersionManifest>(request.downloadHandler.text);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[LuaUpdateService] Invalid version.json: {exception.Message}");
                yield break;
            }
        }

        if (!ValidateManifest(manifest))
        {
            yield break;
        }

        int localVersion = LuaConfigService.Instance.IsLoaded
            ? LuaConfigService.Instance.LoadedVersion
            : -1;

        if (manifest.version <= localVersion)
        {
            Debug.Log($"[LuaUpdateService] Lua config is current. Version: {localVersion}");
            yield break;
        }

        Debug.Log($"[LuaUpdateService] Updating Lua {localVersion} -> {manifest.version}");

        byte[] downloadedBytes;

        using (UnityWebRequest request = UnityWebRequest.Get(manifest.luaUrl))
        {
            request.timeout = timeoutSeconds;
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[LuaUpdateService] Lua download failed: {request.error}");
                yield break;
            }

            downloadedBytes = request.downloadHandler.data;
        }

        if (manifest.fileSize > 0 && downloadedBytes.LongLength != manifest.fileSize)
        {
            Debug.LogWarning(
                $"[LuaUpdateService] File size mismatch. " +
                $"Expected {manifest.fileSize}, received {downloadedBytes.LongLength}."
            );
            yield break;
        }

        if (!FileHashUtility.MatchesSha256(downloadedBytes, manifest.sha256))
        {
            Debug.LogWarning("[LuaUpdateService] SHA-256 verification failed.");
            yield break;
        }

        if (!TryValidateLua(downloadedBytes, manifest.version, out string validationError))
        {
            Debug.LogWarning($"[LuaUpdateService] Lua validation failed: {validationError}");
            yield break;
        }

        if (!TryInstallLua(downloadedBytes, out bool targetExisted, out string installError))
        {
            Debug.LogWarning($"[LuaUpdateService] Failed to install Lua: {installError}");
            yield break;
        }

        bool reloadSucceeded = LuaConfigService.Instance.Reload();
        bool versionMatches = LuaConfigService.Instance.LoadedVersion == manifest.version;

        if (!reloadSucceeded || !versionMatches)
        {
            Debug.LogWarning("[LuaUpdateService] Reload failed. Restoring previous Lua.");
            RestorePreviousLua(targetExisted);
            yield break;
        }

        Debug.Log($"[LuaUpdateService] Lua update completed. Version: {manifest.version}");
    }

    private static bool ValidateManifest(LuaVersionManifest manifest)
    {
        if (manifest == null)
        {
            Debug.LogWarning("[LuaUpdateService] Manifest is empty.");
            return false;
        }

        if (manifest.version < 0 || string.IsNullOrWhiteSpace(manifest.luaUrl))
        {
            Debug.LogWarning("[LuaUpdateService] Manifest version or luaUrl is invalid.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(manifest.sha256))
        {
            Debug.LogWarning("[LuaUpdateService] Manifest SHA-256 is missing.");
            return false;
        }

        return true;
    }

    private static bool TryValidateLua(byte[] bytes, int expectedVersion, out string error)
    {
        LuaEnv validationEnv = null;
        LuaTable root = null;
        LuaTable cards = null;

        try
        {
            validationEnv = new LuaEnv();
            object[] results = validationEnv.DoString(bytes, "DownloadedSkillValidation");

            if (results == null || results.Length == 0)
            {
                throw new InvalidDataException("skill.lua returned no value.");
            }

            root = results[0] as LuaTable;

            if (root == null)
            {
                throw new InvalidDataException("skill.lua must return a table.");
            }

            int scriptVersion = root.Get<int>("version");

            if (scriptVersion != expectedVersion)
            {
                throw new InvalidDataException(
                    $"Manifest version {expectedVersion} does not match Lua version {scriptVersion}."
                );
            }

            cards = root.Get<LuaTable>("cards");

            if (cards == null)
            {
                throw new InvalidDataException("skill.lua has no cards table.");
            }

            error = null;
            return true;
        }
        catch (Exception exception)
        {
            error = exception.Message;
            return false;
        }
        finally
        {
            cards?.Dispose();
            root?.Dispose();
            validationEnv?.Dispose();
        }
    }

    private static bool TryInstallLua(byte[] bytes, out bool targetExisted, out string error)
    {
        string directory = LuaRuntime.Instance.PersistentLuaDirectory;
        string targetPath = Path.Combine(directory, "skill.lua");
        string temporaryPath = targetPath + ".tmp";
        string backupPath = targetPath + ".bak";

        targetExisted = File.Exists(targetPath);

        try
        {
            Directory.CreateDirectory(directory);
            File.WriteAllBytes(temporaryPath, bytes);

            if (targetExisted)
            {
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                File.Replace(temporaryPath, targetPath, backupPath);
            }
            else
            {
                File.Move(temporaryPath, targetPath);
            }

            error = null;
            return true;
        }
        catch (Exception exception)
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }

            error = exception.Message;
            return false;
        }
    }

    private static void RestorePreviousLua(bool targetExisted)
    {
        string directory = LuaRuntime.Instance.PersistentLuaDirectory;
        string targetPath = Path.Combine(directory, "skill.lua");
        string backupPath = targetPath + ".bak";

        try
        {
            if (targetExisted && File.Exists(backupPath))
            {
                File.Copy(backupPath, targetPath, true);
            }
            else if (!targetExisted && File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }

            LuaConfigService.Instance.Reload();
        }
        catch (Exception exception)
        {
            Debug.LogError($"[LuaUpdateService] Restore failed: {exception}");
        }
    }
}