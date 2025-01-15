using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Utilities
{

    public static class EncryptionHelper
    {
        public static byte[] EncryptPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPassword(string inputPassword, byte[] storedHash)
        {
            byte[] inputHash = EncryptPassword(inputPassword);
            return inputHash.SequenceEqual(storedHash);
        }
    }
}
