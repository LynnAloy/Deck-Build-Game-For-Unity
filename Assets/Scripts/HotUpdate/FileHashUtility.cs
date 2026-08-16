using System;
using System.Security.Cryptography;

public static class FileHashUtility
{
    public static string ComputeSha256(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(data);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }

    public static bool MatchesSha256(byte[] data, string expectedHash)
    {
        if (string.IsNullOrWhiteSpace(expectedHash))
        {
            return false;
        }

        string actualHash = ComputeSha256(data);
        return string.Equals(actualHash, expectedHash.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}