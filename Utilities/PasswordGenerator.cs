using System.Security.Cryptography;

namespace PerformanceSurvey.Utilities
{
    public class PasswordGenerator
    {
        private static readonly char[] PasswordCharacters =
     "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*".ToCharArray();


        public static string GenerateSecurePassword(int length = 12)
        {
            if (length < 8)
            {
                throw new ArgumentException("Password length should be at least 8 characters.");
            }

            using (var rng = new RNGCryptoServiceProvider())
            {
                var password = new char[length]; 
                var byteArray = new byte[4]; 

                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(byteArray);
                    var randomIndex = BitConverter.ToUInt32(byteArray, 0) % PasswordCharacters.Length;
                    password[i] = PasswordCharacters[randomIndex];
                }

                return new string(password);
            }
        }
    }
}