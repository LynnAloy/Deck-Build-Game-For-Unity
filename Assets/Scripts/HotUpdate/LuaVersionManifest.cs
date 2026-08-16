using System;

[Serializable]
public sealed class LuaVersionManifest
{
    public int version;
    public string luaUrl;
    public string sha256;
    public long fileSize;
}