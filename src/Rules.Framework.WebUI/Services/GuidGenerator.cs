namespace Rules.Framework.WebUI.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    internal static class GuidGenerator
    {
        private static HashAlgorithm hashAlgorithm = SHA256.Create();

        public static Guid GenerateFromString(string source)
        {
            var hashedBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(source));
            return new Guid(hashedBytes[..16]);
        }
    }
}