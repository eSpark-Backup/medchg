using System.Text;

namespace MEDCHGWEB.Services
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
    }
}
