using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using PSW.Common.Crypto;

namespace PSW.ITT.Common
{
    public static class Utility
    {
        private const string AesIV256 = @"!QAZ2WSX#EDC4RFV";
        private const string AesKey256 = @"5TGB&YHN7UJM(IK<5TGB&YHN7UJM(IK<";
        /// <summary>
        /// Checks if incoming type inherits from Generic Type 
        /// </summary>
        /// <param name="givenType"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        /// <summary>
        ///     Checks if requested user role found in list of roles in claims
        ///     if found then returns entity having role code and role id otherwise returns null
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="roleCode"></param>
        /// <returns>UserRoleDTO</returns>

        public static string DateFormatInbox(this DateTime dt)
        {
            var date = CultureInfo.CreateSpecificCulture("en-US").DateTimeFormat;
            date.ShortDatePattern = @"dd-MM-yyyy";
            return $"{dt.ToString("d", date)} - {dt.DayOfWeek.ToString().Substring(0, 3)}";
        }

        public static string GetDifferenceInHours(this DateTime dt)
        {
            return (DateTime.Now - dt).TotalHours.ToString("N0");
        }

         /// <summary>
        ///     Checks if requested user role found in list of roles in claims
        ///     if found then returns entity having role code and role id otherwise returns null
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="roleCode"></param>
        /// <returns>UserRoleDTO</returns>
        public static UserRoleDTO GetCurrentUserRole(IEnumerable<Claim> claims, string roleCode)
        {
            var userRoles = claims.Where(x => x.Type == "role").ToList();
            var userRoleEntities = new List<UserRoleDTO>();

            foreach (var userRole in userRoles)
            {
                var userRolesDto = System.Text.Json.JsonSerializer.Deserialize<List<UserRoleDTO>>(userRole.Value);
                userRoleEntities.AddRange(userRolesDto);
            }

            return userRoleEntities.FirstOrDefault(x => x.RoleCode == roleCode);
        }
        /// <summary>
        /// Encrypts the string using AES256
        /// </summary>
        /// <param name="input"></param>
        /// <returns>AES256 encrypted data</returns>
        public static string AESEncrypt256(string input)
        {
            byte[] encrypted;
            byte[] IV;
            byte[] Salt = GetSalt();
            byte[] Key = CreateKey(AesKey256, Salt);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.CBC;

                aesAlg.GenerateIV();
                IV = aesAlg.IV;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            byte[] combinedIvSaltCt = new byte[Salt.Length + IV.Length + encrypted.Length];
            Array.Copy(Salt, 0, combinedIvSaltCt, 0, Salt.Length);
            Array.Copy(IV, 0, combinedIvSaltCt, Salt.Length, IV.Length);
            Array.Copy(encrypted, 0, combinedIvSaltCt, Salt.Length + IV.Length, encrypted.Length);

            return Convert.ToBase64String(combinedIvSaltCt.ToArray());
        }
        /// <summary>
        /// Decrypts data using AES256
        /// </summary>
        /// <param name="input"></param>
        /// <returns>AES256 decrypted data</returns>
        public static string AESDecrypt256(string input)
        {
            byte[] inputAsByteArray;
            string plaintext = null;
            try
            {
                inputAsByteArray = Convert.FromBase64String(input);

                byte[] Salt = new byte[32];
                byte[] IV = new byte[16];
                byte[] Encoded = new byte[inputAsByteArray.Length - Salt.Length - IV.Length];

                Array.Copy(inputAsByteArray, 0, Salt, 0, Salt.Length);
                Array.Copy(inputAsByteArray, Salt.Length, IV, 0, IV.Length);
                Array.Copy(inputAsByteArray, Salt.Length + IV.Length, Encoded, 0, Encoded.Length);

                byte[] Key = CreateKey(AesKey256, Salt);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (var msDecrypt = new MemoryStream(Encoded))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                return plaintext;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Create key
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns>Key</returns>
        public static byte[] CreateKey(string password, byte[] salt)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt))
                return rfc2898DeriveBytes.GetBytes(32);
        }

        /// <summary>
        /// Return random salt
        /// </summary>
        /// <returns>salt</returns>
        private static byte[] GetSalt()
        {
            var salt = new byte[32];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }
            return salt;
        }

    public static string DecryptConnectionString(){
            try
        {
            string salt = Environment.GetEnvironmentVariable("ENCRYPTION_SALT");
            string password = Environment.GetEnvironmentVariable("ENCRYPTION_PASSWORD");
            string connection=  Environment.GetEnvironmentVariable("ConnectionStrings__ITTConnectionString");
            if (string.IsNullOrWhiteSpace(salt) || string.IsNullOrWhiteSpace(password))
            {
                throw new System.Exception("Please provide salt and password for Crypto Algorithm in Environment Variable");
            }

            var crypto = new CryptoFactory().Create<AesManaged>(password, salt);
            
            if (string.IsNullOrWhiteSpace(salt) || string.IsNullOrWhiteSpace(password))
            {
                throw new System.Exception("Please provide salt and password for Crypto Algorithm in Environment Variable");
            }
            if (string.IsNullOrWhiteSpace(connection) )
            {
                throw new System.Exception("Please provide connection string Crypto Algorithm in Environment Variable");
            }
            return  crypto.Decrypt(connection);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        }

    }
}