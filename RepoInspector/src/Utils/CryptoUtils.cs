using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace RepoInspector.src.Utils
{
    public static class CryptoUtils
    {
        /// <summary>
        /// Converts a byte array to a hexadecimal string representation.
        /// </summary>
        /// <param name="bytes">The byte array to be converted.</param>
        /// <returns>
        ///   A hexadecimal string representation of the byte array.
        /// </returns>
        /// <remarks>
        ///   This method takes a byte array as input and returns a hexadecimal string representation
        ///   of the bytes. Each byte is converted to a two-character hexadecimal value, resulting
        ///   in a string with twice the length of the input byte array.
        /// </remarks>
        public static string ToHexString(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                builder.AppendFormat("{0:x2}", b);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Computes the HMAC-SHA256 hash of a JSON payload using the provided secret.
        /// </summary>
        /// <param name="payload">The payload to hash, typically a JSON object or string.</param>
        /// <param name="secret">The secret key used for hashing.</param>
        /// <returns>The computed HMAC-SHA256 hash as a byte array.</returns>
        public static byte[] ComputeHMACSHA256Hash(object payload, string secret)
        {
            // Convert the secret to a byte array to use as the key for HMAC-SHA256.
            byte[] key = Encoding.UTF8.GetBytes(secret);

            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                // Compute the hash by converting the payload to a UTF-8 byte array.
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
                return computedHash;
            }
        }
    }
}
