using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace RepositoryLayer.Helper
{
    public class PasswordHash
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const int Iterartions = 10000;

        public string passwordHashing(string userPass)
        {
            try
            {
                byte[] salt;
                new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);
                var pkdf2 = new Rfc2898DeriveBytes(userPass, salt, Iterartions); //(Password-Based Key Derivation Function 2) 
                byte[] hash = pkdf2.GetBytes(HashSize);

                // Combine salt and hash into a single byte array
                byte[] hashBytes = new byte[SaltSize + HashSize]; //source arr ,srource idx ,dest arr,dest idx ,dest length
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

                // Convert the combined byte array to a base64-encoded string for storage
                string savedPasswordHash = Convert.ToBase64String(hashBytes);

                return savedPasswordHash;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool VerifyPassword(string userPass, string storedHashPass)
        {
            // Convert the stored password hash from base64 back to bytes
            byte[] hashBytes = Convert.FromBase64String(storedHashPass);

            // Extract salt from the stored password hash
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Compute the hash of the entered password using the same salt and iterations
            var pbkdf2 = new Rfc2898DeriveBytes(userPass, salt, Iterartions);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Compare the computed hash with the stored hash
            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }

    }
}
