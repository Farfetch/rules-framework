namespace Rules.Framework.WebUI.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    internal static class GuidGenerator
    {
        private static readonly HashAlgorithm hashAlgorithm = SHA256.Create();

        public static Guid GenerateFromString(string source)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentException.ThrowIfNullOrWhiteSpace(source);

            var hashedBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(source));
            return new Guid(hashedBytes[..16]);
        }
    }
}