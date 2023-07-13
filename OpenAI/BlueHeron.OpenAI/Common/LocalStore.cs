using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using BlueHeron.OpenAI.Interfaces;

namespace BlueHeron.OpenAI;

/// <summary>
/// Object that handles secure local persistence of data.
/// </summary>
public static class LocalStore
{
    #region Objects and variables

    private const string fmtFile = "{0}.dat";

    #endregion

    #region IO

    /// <summary>
    /// Returns an object of type <typeparamref name="T"/> loaded from the given file.
    /// </summary>
    /// <typeparam name="T">The type of the object to load, which must implement <see cref="IJsonSerializable"/></typeparam>
    /// <param name="fileName">The name of the file, without extension</param>
    /// <returns>An object of type <typeparamref name="T"/></returns>
    public static T Load<T>(string fileName) where T : IJsonSerializable, new()
    {
        var path = CreatePath(fileName);
        if (File.Exists(path))
        {
            try
            {
                return IJsonSerializable.FromJson<T>(Decrypt(Decompress(File.ReadAllBytes(path))));
            }
            catch { } // ignore
        }
        return default;
    }

    /// <summary>
    /// Saves the given object  of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to save, which must implement <see cref="IJsonSerializable"/></typeparam>
    /// <param name="fileName">The name of the file, without extension</param>
    /// <param name="data">The object of type <typeparamref name="T"/> to save</param>
    public static void Save<T>(string fileName, T data) where T : IJsonSerializable
    {
        try
        {
            File.WriteAllBytes(CreatePath(fileName), Compress(Encrypt(data.ToJson())));
        }
        catch { } // ignore
    }

    /// <summary>
    /// Returns the full path to the file with the given name.
    /// </summary>
    /// <param name="fileName">The name of the file, without extension</param>
    /// <returns>The full path to the file</returns>
    private static string CreatePath(string fileName) => Path.Combine(FileSystem.Current.AppDataDirectory, string.Format(fmtFile, fileName));

    #endregion

    #region ZLib

    /// <summary>
    /// Compresses the given string into a byte array (UTF8 encoding is used).
    /// </summary>
    /// <param name="input">Input</param>
    /// <returns>Byte array containing compressed output</returns>
    [DebuggerStepThrough()]
    public static byte[] Compress(string input)
    {
        var uncompressed = Encoding.UTF8.GetBytes(input);
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new ZLibStream(memoryStream, CompressionLevel.SmallestSize))
        {
            gzipStream.Write(uncompressed, 0, uncompressed.Length);
        }
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Decompresses the given byte array into a string (UTF8 encoding is used).
    /// </summary>
    /// <param name="input">Byte array containing input input</param>
    /// <returns>Decompressed string output</returns>
    [DebuggerStepThrough()]
    public static string Decompress(byte[] input)
    {
        using var memoryStream = new MemoryStream(input);
        using var outputStream = new MemoryStream();
        using (var decompressStream = new ZLibStream(memoryStream, CompressionMode.Decompress))
        {
            decompressStream.CopyTo(outputStream);
        }
        return Encoding.UTF8.GetString(outputStream.ToArray());
    }

    #endregion

    #region MD5

    /// <summary>
    /// Encrypts the given unencrypted string.
    /// </summary>
    /// <param name="input">The unencrypted input string</param>
    /// <returns>An encrypted output string</returns>
    public static string Encrypt(string input)
    {
        var userBytes = Encoding.UTF8.GetBytes(input); // UTF8 saves Space
        var userHash = MD5.HashData(userBytes);
        var crypt = Aes.Create(); // (Default: AES-CCM (Counter with CBC-MAC))

        crypt.Key = MD5.HashData(Encoding.UTF8.GetBytes(Assembly.GetExecutingAssembly().FullName)); // MD5: 128 Bit Hash
        crypt.IV = new byte[16]; // by Default. IV[] to 0.. is OK simple crypt
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write);
        cryptoStream.Write(userBytes, 0, userBytes.Length); // User Data
        cryptoStream.Write(userHash, 0, userHash.Length); // Add HASH
        cryptoStream.FlushFinalBlock();

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    /// <summary>
    /// Decrypts the given encrypted string.
    /// </summary>
    /// <param name="input">The encrypted input string</param>
    /// <returns>A decrypted output string</returns>
    /// <exception cref="Exception">Invalid format</exception>
    public static string Decrypt(string input)
    {
        var encryptedBytes = Convert.FromBase64String(input);
        var crypt = Aes.Create();
        crypt.Key = MD5.HashData(Encoding.UTF8.GetBytes(Assembly.GetExecutingAssembly().FullName));
        crypt.IV = new byte[16];
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Write);
        cryptoStream.Write(encryptedBytes, 0, encryptedBytes.Length);
        cryptoStream.FlushFinalBlock();
        var allBytes = memoryStream.ToArray();
        var userLen = allBytes.Length - 16;
        if (userLen < 0)
        {
            throw new Exception(nameof(input));   // No Hash?
        }

        var userHash = new byte[16];
        Array.Copy(allBytes, userLen, userHash, 0, 16); // Get the 2 Hashes
        var decryptHash = MD5.HashData(allBytes.AsSpan(0, userLen));
        if (userHash.SequenceEqual(decryptHash) == false)
        {
            throw new Exception(nameof(input));
        }

        return Encoding.UTF8.GetString(allBytes, 0, userLen);
    }

    #endregion
}