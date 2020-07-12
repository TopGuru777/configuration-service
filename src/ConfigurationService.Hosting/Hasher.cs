﻿using System.Security.Cryptography;
using System.Text;

namespace ConfigurationService.Hosting
{
    public static class Hasher
    {
        public static string CreateMD5Hash(byte[] bytes)
        {
            using (var hash = MD5.Create())
            {
                var hashBytes = hash.ComputeHash(bytes);

                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}