/*
 * (CryptoManager.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026 @ 20:18
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using System.Security.Cryptography;
using System.Text;

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
        private const int ChallengeNonceSize = 32; // 256 bits for auth challenge none

        /// <summary>
        /// Encrypts a byte array of plaintext data using AES-GCM encryption.
        /// Generates a unique nonce for each encryption operation, applies encryption,
        /// and combines the nonce, authentication tag, and ciphertext into a single byte array.
        /// </summary>
        /// <param name="dataToEncrypt">The plaintext data as a byte array that needs to be encrypted.</param>
        /// <returns>A byte array containing the nonce, authentication tag, and ciphertext.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided plaintext data is null or empty.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs during encryption.</exception>
        public static byte[] Encrypt(byte[] dataToEncrypt)
        {
            if (dataToEncrypt == null || dataToEncrypt.Length == 0)
                throw new ArgumentException("Payload cannot be null or empty");

            try
            {
                using var aesGcm = new AesGcm(MasterKey.AsSpan());

                // Generate the random nonce, which is unique for each encryption.
                var nonce = new byte[NonceSize];
                RandomNumberGenerator.Fill(nonce);

                var ciphertext = new byte[dataToEncrypt.Length];
                var tag = new byte[TagSize];

                aesGcm.Encrypt(nonce.AsSpan(),
                    dataToEncrypt.AsSpan(),
                    ciphertext.AsSpan(),
                    tag.AsSpan());

                var result = new byte[NonceSize + TagSize + ciphertext.Length];
                Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
                Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
                Buffer.BlockCopy(ciphertext, 0, result, NonceSize + TagSize,
                    ciphertext.Length);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CryptoManager]: ERROR ENCOUNTERED IN ENCRYPT:\n{ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Decrypts a byte array of data that was encrypted using AES-GCM encryption.
        /// Extracts the nonce, authentication tag, and ciphertext to perform decryption,
        /// and returns the original plaintext data.
        /// </summary>
        /// <param name="encryptedData">The encrypted data as a byte array, containing the nonce, authentication tag,
        /// and ciphertext.</param>
        /// <returns>A byte array representing the decrypted plaintext data.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided encrypted data is null, empty, or too short to
        /// contain valid data.</exception>
        /// <exception cref="CryptographicException">Thrown when decryption fails, such as when authentication fails or
        /// the data is corrupted.</exception>
        public static byte[] Decrypt(byte[] encryptedData)
        {
            if (encryptedData == null || encryptedData.Length < NonceSize + TagSize)
                throw new ArgumentException("Encrypted data is invalid or too short.", nameof(encryptedData));

            try
            {
                using var aesGcm = new AesGcm(MasterKey.AsSpan());

                // Extract nonce
                var nonce = new byte[NonceSize];
                // tag it
                var tag = new byte[TagSize];
                // and ciphertext
                var ciphertext = new byte[encryptedData.Length - NonceSize - TagSize];

                Buffer.BlockCopy(encryptedData, 0, nonce, 0, NonceSize);
                Buffer.BlockCopy(encryptedData, NonceSize, tag, 0, TagSize);
                Buffer.BlockCopy(encryptedData, NonceSize + TagSize, ciphertext, 0,
                    ciphertext.Length);

                var plainData = new byte[ciphertext.Length];
                aesGcm.Decrypt(nonce.AsSpan(), ciphertext.AsSpan(), tag.AsSpan(), plainData.AsSpan());

                return plainData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CryptoManager]: ERROR ENCOUNTERED IN DECRYPT:\n{ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Encrypts a plaintext password using AES-GCM encryption and returns the result as a byte array.
        /// Ensures secure encryption suitable for password storage or transmission.
        /// </summary>
        /// <param name="password">The plaintext password to be encrypted.</param>
        /// <returns>A byte array representing the encrypted password.</returns>
        /// <exception cref="ArgumentException">Thrown when the password is null or empty.</exception>
        public static byte[] EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            var passwordBytes = Encoding.UTF8.GetBytes(password);

            return Encrypt(passwordBytes);
        }

        /// <summary>
        /// Decrypts an encrypted password and returns the result as a plaintext string.
        /// Uses AES-GCM decryption to securely restore the original password.
        /// </summary>
        /// <param name="encryptedPassword">The encrypted password represented as a byte array.</param>
        /// <returns>The decrypted password as a plaintext string.</returns>
        public static string DecryptPassword(byte[] encryptedPassword)
        {
            var decryptedBytes = Decrypt(encryptedPassword);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// Generates a cryptographically secure random key.
        /// The generated key is 256 bits in length, suitable for AES encryption or other secure cryptographic
        /// applications.
        /// </summary>
        /// <returns>A 32-byte array containing the generated key.</returns>
        public static byte[] GenerateKey()
        {
            var key = new byte[32]; // 256 bits

            RandomNumberGenerator.Fill(key);

            return key;
        }

        public static byte[] GenerateNonce()
        {
            var nonce = new byte[ChallengeNonceSize];
            RandomNumberGenerator.Fill(nonce);

            return nonce;
        }

        public static byte[] ComputeHmac(byte[] key, byte[] data)
        {
            using var hmac = new HMACSHA256(key);

            return hmac.ComputeHash(data);
        }

        public static bool ValidateChallengeResponse(byte[] storedPasswordHash,
            byte[] nonce,
            byte[] clientResponse)
        {
            var expectedResponse = ComputeHmac(storedPasswordHash, nonce);

            return CompareHashedData(expectedResponse, clientResponse);
        }

        public static string ByteArrayToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public static bool CompareHashedData(byte[] dataOne, byte[] dataTwo)
        {
            if (dataOne.Length != dataTwo.Length)
                return false;

            var result = 0;

            for (var i = 0; i < dataOne.Length; i++)
            {
                result |= dataOne[i] ^ dataTwo[i];
            }

            return result == 0;
        }
    }
}
/*
 *------------------------------------------------------------
 * (CryptoManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */