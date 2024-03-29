﻿using System.Security.Cryptography;

namespace Common.Infrastructure;

public static class PasswordHasher
{
    private const int Interations = 10000;
    private const int SaltBytesSize = 64;
    private const int HashBytesSize = 128;

    public static byte[] CreateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var rand = new byte[SaltBytesSize];
        rng.GetBytes(rand);
        return rand;
    }

    public static string GetHashString(byte[] hashBytes)
    {
        return Convert.ToBase64String(hashBytes);
    }

    public static byte[] CreateHash(string password, byte[] salt)
    {
        using var rfc2898 = new Rfc2898DeriveBytes(password, salt, Interations, HashAlgorithmName.SHA1);
        return rfc2898.GetBytes(HashBytesSize);
    }

    public static bool Validate(string password, byte[] passwordHash, byte[] salt)
    {
        byte[] hash = CreateHash(password, salt);
        return SlowEquals(hash, passwordHash);
    }

    private static bool SlowEquals(byte[] a, byte[] b)
    {
        uint diff = (uint)a.Length ^ (uint)b.Length;
        for (var i = 0; i < a.Length && i < b.Length; i++)
            diff |= (uint)(a[i] ^ b[i]);

        return diff == 0;
    }
}
