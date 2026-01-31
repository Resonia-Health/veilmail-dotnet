using System;
using System.Security.Cryptography;
using System.Text;

namespace VeilMail
{
    /// <summary>
    /// Utility for verifying webhook signatures.
    /// </summary>
    public static class Webhook
    {
        /// <summary>
        /// Verify a webhook signature using constant-time HMAC-SHA256 comparison.
        /// </summary>
        /// <param name="body">The raw request body</param>
        /// <param name="signature">The signature from the X-Signature-Hash header</param>
        /// <param name="secret">The webhook signing secret</param>
        /// <returns>True if the signature is valid</returns>
        public static bool VerifySignature(string body, string signature, string secret)
        {
            try
            {
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
                var expected = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                return CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(expected),
                    Encoding.UTF8.GetBytes(signature)
                );
            }
            catch
            {
                return false;
            }
        }
    }
}
