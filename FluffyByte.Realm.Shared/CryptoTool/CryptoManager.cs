/*
 * (CryptoManager.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026 @ 20:18
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using System.Security.Cryptography;

namespace FluffyByte.Realm.Shared.CryptoTool
{
    /// <summary>
    /// Static manager for AES-GCM encryption/decryption operations.
    /// Used primarily for encrypting passwords in transit.
    /// </summary>
    public static class CryptoManager
    {
        // IMPORTANT: This key should be 32 bytes (256 bits) for AES-256
        // In production, this should be stored securely (config, environment variable, etc.)
        private static readonly byte[] MasterKey =
        {
            0x2B, 0x7E, 0x15, 0x16, 0x28, 0xAE, 0xD2, 0xA6,
            0xAB, 0xF7, 0x15, 0x88, 0x09, 0xCF, 0x4F, 0x3C,
            0x76, 0x2E, 0x71, 0x60, 0xF3, 0x8B, 0x4D, 0xA5,
            0x6A, 0x78, 0x4D, 0x90, 0x45, 0x19, 0x0C, 0xFE
        };

        private const int NonceSize = 12; // 96 bits - standard for AES-GCM
        private const int TagSize = 16; // 128 bits - authentication tag

        /// <summary>
        /// Encrypt data using AES-GCM with the master key.
        /// Returns: [nonce (12 bytes)][tag (16 bytes)][encrypted data]
        /// </summary>
        public static byte[] Encrypt(byte[] plaintext)
        {
            if (plaintext == null || plaintext.Length == 0)
                throw new ArgumentException("Plaintext cannot be null or empty", nameof(plaintext));

            try
            {
                using var aesGcm = new AesGcm(MasterKey.AsSpan());

                // Generate random nonce (must be unique for each encryption)
                var nonce = new byte[NonceSize];
                RandomNumberGenerator.Fill(nonce);

                // Allocate space for encrypted data and tag
                var ciphertext = new byte[plaintext.Length];
                var tag = new byte[TagSize];

                // Encrypt the data
                aesGcm.Encrypt(nonce.AsSpan(), plaintext.AsSpan(), ciphertext.AsSpan(), tag.AsSpan());

                // Combine: nonce + tag + ciphertext
                var result = new byte[NonceSize + TagSize + ciphertext.Length];
                Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
                Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
                Buffer.BlockCopy(ciphertext, 0, result, NonceSize + TagSize, ciphertext.Length);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CryptoManager] Encryption failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Decrypt data using AES-GCM with the master key.
        /// Expects the format: [nonce (12 bytes)][tag (16 bytes)][encrypted data]
        /// </summary>
        public static byte[] Decrypt(byte[] encryptedData)
        {
            if (encryptedData == null || encryptedData.Length < NonceSize + TagSize)
                throw new ArgumentException("Encrypted data is invalid or too short", nameof(encryptedData));

            try
            {
                using var aesGcm = new AesGcm(MasterKey.AsSpan());

                // Extract nonce, tag, and ciphertext
                var nonce = new byte[NonceSize];
                var tag = new byte[TagSize];
                var ciphertext = new byte[encryptedData.Length - NonceSize - TagSize];

                Buffer.BlockCopy(encryptedData, 0, nonce, 0, NonceSize);
                Buffer.BlockCopy(encryptedData, NonceSize, tag, 0, TagSize);
                Buffer.BlockCopy(encryptedData, NonceSize + TagSize, ciphertext, 0, ciphertext.Length);

                // Decrypt the data
                var plaintext = new byte[ciphertext.Length];
                aesGcm.Decrypt(nonce.AsSpan(), ciphertext.AsSpan(), tag.AsSpan(), plaintext.AsSpan());

                return plaintext;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine(
                    $"[CryptoManager] Decryption failed - authentication failed or corrupted data: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CryptoManager] Decryption failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Encrypt a string password and return encrypted bytes.
        /// </summary>
        public static byte[] EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            return Encrypt(passwordBytes);
        }

        /// <summary>
        /// Decrypt encrypted password bytes and return the original string.
        /// </summary>
        public static string DecryptPassword(byte[] encryptedPassword)
        {
            var decryptedBytes = Decrypt(encryptedPassword);
            return System.Text.Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// Generate a new random 32-byte AES key.
        /// Use this to generate a new master key if needed.
        /// </summary>
        public static byte[] GenerateKey()
        {
            var key = new byte[32]; // 256 bits
            RandomNumberGenerator.Fill(key);
            return key;
        }

        /// <summary>
        /// Print a byte array as a hex string for debugging/configuration.
        /// </summary>
        public static string ByteArrayToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
/*
 *------------------------------------------------------------
 * (CryptoManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */