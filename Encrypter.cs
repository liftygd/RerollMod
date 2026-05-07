using System;
using System.Security.Cryptography;
using System.Text;

public class Encrypter
{
    private static readonly byte[] Salt = Encoding.UTF8.GetBytes("5ba8d866031195e28cd8dbb4bc2ce5bc");
    private const int Iterations = 100000;

    public string Encrypt(string plainText, string password)
    {
        using var deriveBytes = new Rfc2898DeriveBytes(password, Salt, Iterations, HashAlgorithmName.SHA256);
        byte[] key = deriveBytes.GetBytes(32);

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();
        byte[] iv = aes.IV;

        using var encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        
        byte[] result = new byte[iv.Length + cipherBytes.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, iv.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText, string password)
    {
        try
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            
            using var deriveBytes = new Rfc2898DeriveBytes(password, Salt, Iterations, HashAlgorithmName.SHA256);
            byte[] key = deriveBytes.GetBytes(32);

            using Aes aes = Aes.Create();
            aes.Key = key;

            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] cipherBytes = new byte[fullCipher.Length - iv.Length];
            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);

            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (CryptographicException)
        {
            return string.Empty;
        }
    }
}