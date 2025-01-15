using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace MEDCHGClassLibrary.Services
{
    public class EncryptDecrypt
    {
        private static string StrictSecurity(string data, string salt = "mEdChg", bool encrypt = true)
        {
            if (string.IsNullOrEmpty(data))
            {
                return "";
            }
            if (encrypt)
            {
                byte[] bytes = Encoding.UTF8.GetBytes($"{salt}:{data}");
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToBase64String(bytes)));
            }
            else
            {
                var blob = Convert.FromBase64String(Encoding.UTF8.GetString(Convert.FromBase64String(data)));
                return Encoding.UTF8.GetString(blob).Replace($"{salt}:", "");
            }
        }
        public static string EncryptDecryptData(string data, bool encrypt = true, string salt = "mEdChg")
        {
            if (string.IsNullOrEmpty(data))
            {
                return "";
            }
            if (encrypt) // Encrypt data.
            {
                string saltHash = StrictSecurity(StrictSecurity(StrictSecurity(salt, salt, true), salt, true), salt, true)
                , dataHash = StrictSecurity(data, salt, true)
                , enc = StrictSecurity(StrictSecurity($"{saltHash}|{dataHash}", salt, true), salt, true);
                return enc;
            }
            else // Decrypt data.
            {
                string dec = StrictSecurity(StrictSecurity(data, salt, false), salt, false);
                if (!dec.Contains('|'))
                {
                    return "";
                }
                string[] dataEnc = dec.Split("|");
                if (dataEnc.Length != 2)
                {
                    return "";
                }
                string saltHashDec = StrictSecurity(StrictSecurity(StrictSecurity(dataEnc[0] ?? "", salt, false), salt, false), salt, false)
                , dataHashDec = StrictSecurity(dataEnc[1] ?? "", salt, false);
                if (!salt.Equals(saltHashDec))
                {
                    return "";
                }
                return dataHashDec;
            }
        }

        public static string? HashStringWithSalt(string? input, string? salt = "mEdChg")
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(salt))
            {
                return null;
            }
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            //using Rfc2898DeriveBytes pbkdf2 = new(input, saltBytes, 10000);
            //byte[] hashBytes = pbkdf2.GetBytes(32); // 32 bytes for a 256-bit hash
            //return Convert.ToBase64String(hashBytes);


            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: input,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}
