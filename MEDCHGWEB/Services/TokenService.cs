using MEDCHGClassLibrary.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MEDCHGWEB.Services
{
    public class AppSettingJwt
    {
        public Jwt Jwt { get; set; }
    }

    public class Jwt
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double TokenDurationInMinutes { get; set; }
        public double SignatureDurationInMinutes { get; set; }
        public string PrivateKeyFileName { get; set; }
        public string PublicKeyFileName { get; set; }
        public string RSAKeysFolderName { get; set; }
    }

    public class AccessTokenModel 
    {
        [JsonProperty("tokenId")]
        public string TokenId { get; set; }
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
        [JsonProperty("validFrom")]
        public DateTime ValidFrom { get; set; }
        [JsonProperty("validTo")]
        public DateTime ValidTo { get; set; }
    }
    public class SignTokenModel
    {
        [JsonProperty("tokenId")]
        public string TokenId { get; set; }
        [JsonProperty("signature")]
        public string Signature { get; set; }
        [JsonProperty("validFrom")]
        public DateTime ValidFrom { get; set; }
        [JsonProperty("validTo")]
        public DateTime ValidTo { get; set; }
    }
    public class TokenService
    {
        private static AppSettingJwt AppSettingJwt = JsonConvert.DeserializeObject<AppSettingJwt>(File.ReadAllText(Directory.GetCurrentDirectory() + "/appsettings.json").ToString());

        /// <summary>
        /// Generate private and public key pairs.
        /// </summary>
        /// <param name="ExportPath">Path where to export keys, default application root path.</param>
        /// <param name="KeySize">RSA key size, default key size is 4096</param>
        /// <param name="ExportKeys">Export keys on app root path, default yes(true)</param>
        private static void GenerateRsaKeyPair(string? ExportPath = null, int KeySize = 4096, bool ExportKeys = true, bool ForcelyGenerateKeys = false)
        {
            if (string.IsNullOrEmpty(ExportPath))
            {
                ExportPath = $"{Directory.GetCurrentDirectory()}//{AppSettingJwt.Jwt.RSAKeysFolderName}";
            }
            if (!Directory.Exists(ExportPath))
            {
                Directory.CreateDirectory(ExportPath);
            }
            using RSA rsa = RSA.Create();
            rsa.KeySize = KeySize;
            if (ExportKeys)
            {
                string privateKeyPath = $"{ExportPath}\\{AppSettingJwt.Jwt.PrivateKeyFileName}.pem";
                if (!(File.Exists(privateKeyPath)) || ForcelyGenerateKeys)
                {
                    ExportPrivateKey(rsa, privateKeyPath);
                    ExportPublicKey(rsa, $"{ExportPath}\\{AppSettingJwt.Jwt.PublicKeyFileName}.pem");
                }
            }
        }
        /// <summary>
        /// The method to export private key as pem file.
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="ExportPath">Path where to export keys, default application root path.</param>
        private static void ExportPrivateKey(RSA rsa, string ExportPath)
        {
            RSACryptoServiceProvider rsacsp = new();
            rsacsp.ImportParameters(rsa.ExportParameters(true));
            StringWriter outputStream = new();
            ExportPrivateKeyPem(rsacsp, outputStream);
            File.WriteAllText(ExportPath, outputStream.GetStringBuilder().ToString(), Encoding.ASCII);
        }
        /// <summary>
        /// The method to export public key as pem file.
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="ExportPath">Path where to export keys, default application root path.</param>
        private static void ExportPublicKey(RSA rsa, string ExportPath)
        {
            RSACryptoServiceProvider rsacsp = new();
            rsacsp.ImportParameters(rsa.ExportParameters(true));
            StringWriter outputStream = new();
            ExportPublicKeyPem(rsacsp, outputStream);
            File.WriteAllText(ExportPath, outputStream.GetStringBuilder().ToString(), Encoding.ASCII);
        }
        public static RSACryptoServiceProvider LoadRsaPrivateKey(string? ImportPath = null, bool fromCommon = false)
        {
            RSACryptoServiceProvider rsa = new();
            //if (fromCommon)
            //{
            //    rsa.ImportFromPem(Common.PrivateKey);
            //    return rsa;
            //}
            if (string.IsNullOrEmpty(ImportPath))
            {
                ImportPath = $"{Directory.GetCurrentDirectory()}//{AppSettingJwt.Jwt.RSAKeysFolderName}";
            }
            // Load private key from file or string
            string privateKeyPath = $"{ImportPath}\\{AppSettingJwt.Jwt.PrivateKeyFileName}.pem";
            if (!(File.Exists(privateKeyPath)))
            {
                GenerateRsaKeyPair();
            }
            string privateKey = File.ReadAllText(privateKeyPath, Encoding.ASCII);
            rsa.ImportFromPem(privateKey);
            return rsa;

        }
        /// <summary>
        /// Get newly generated JSON Web Token (JWT).
        /// </summary>
        /// <param name="claims">Little unconfidential data can be store as cliams.</param>
        /// <param name="tokenDurationInMinutes">The token will be expired after specified minutes.</param>
        /// <returns></returns>
        public static AccessTokenModel GenerateJWTToken(List<Claim> claims, double tokenDurationInMinutes = 0)
        {
            if (tokenDurationInMinutes <= 0)
            {
                tokenDurationInMinutes = AppSettingJwt.Jwt.TokenDurationInMinutes;
            }
            // Create signing credentials
            RsaSecurityKey key = new(LoadRsaPrivateKey());
            SigningCredentials credentials = new(key, SecurityAlgorithms.RsaSha512);
            // Create JWT header
            JwtHeader header = new(credentials);
            // Create JWT payload
            claims ??= [];
            var utcNow = DateTime.UtcNow;
            string tokenId = Guid.NewGuid().ToString("N").ToUpper();
            List<Claim> claims2 =
            [
                // the JTI is used for our refresh token which we will be convering in the next video
                // jti (JWT ID): Unique identifier; 
                new Claim("tokenId", tokenId),
                //new Claim("issuedAt", (utcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString()),
                //new Claim("expires", (utcNow.AddMinutes(tokenDurationInMinutes) - new DateTime(1970, 1, 1)).TotalSeconds.ToString()),
                //new Claim("notBefore", (utcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString()),
            ];
            claims.AddRange(claims2);
            JwtPayload payload = new(
                issuer: AppSettingJwt.Jwt.Issuer,
                audience: AppSettingJwt.Jwt.Audience,
                claims: claims,
                issuedAt: utcNow,
                expires: utcNow.AddMinutes(tokenDurationInMinutes),
                notBefore: utcNow
            );
            // Create JWT security token
            var securityToken = new JwtSecurityToken(header, payload);
            JwtSecurityTokenHandler handler = new();
            // Write JWT to string
            string jwt = handler.WriteToken(securityToken);
            return new AccessTokenModel { TokenId = tokenId, AccessToken = jwt, ValidFrom = TimeZoneInfo.ConvertTimeFromUtc(securityToken.ValidFrom, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")), ValidTo = TimeZoneInfo.ConvertTimeFromUtc(securityToken.ValidTo, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")) };
        }
        public static SignTokenModel? SignPayload(string jsonPayload, double signatureDurationInMinutes = 0)
        {
            if (string.IsNullOrEmpty(jsonPayload))
            {
                return null;
            }
            if (signatureDurationInMinutes <= 0)
            {
                signatureDurationInMinutes = AppSettingJwt.Jwt.SignatureDurationInMinutes;
            }
            // Create signing credentials
            string PrivateKeyImportPath = $"{Directory.GetCurrentDirectory()}//{AppSettingJwt.Jwt.RSAKeysFolderName}";
            RsaSecurityKey key = new(LoadRsaPrivateKey(PrivateKeyImportPath));
            SigningCredentials credentials = new(key, SecurityAlgorithms.RsaSha512);
            // Create JWT header
            JwtHeader header = new(credentials);
            // Create JWT payload
            List<Claim> claims = [];
            var utcNow = DateTime.UtcNow;
            string tokenId = Guid.NewGuid().ToString("N").ToUpper();
            List<Claim> claims2 =
            [
                new Claim("tokenId", tokenId),
                new Claim("payload", jsonPayload ?? ""),
            ];
            claims.AddRange(claims2);
            JwtPayload payload = new(
                issuer: AppSettingJwt.Jwt.Issuer,
                audience: AppSettingJwt.Jwt.Audience,
                claims: claims,
                issuedAt: utcNow,
                expires: utcNow.AddMinutes(signatureDurationInMinutes),
                notBefore: utcNow
            );
            // Create JWT security token
            var securityToken = new JwtSecurityToken(header, payload);
            JwtSecurityTokenHandler handler = new();
            // Write JWT to string
            string jwt = handler.WriteToken(securityToken);
            return new SignTokenModel { TokenId = tokenId, Signature = jwt, ValidFrom = TimeZoneInfo.ConvertTimeFromUtc(securityToken.ValidFrom, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")), ValidTo = TimeZoneInfo.ConvertTimeFromUtc(securityToken.ValidTo, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")) };
        }
        public static string EncryptData(string data, bool fromCommon = false)
        {
            using RSACryptoServiceProvider rsa = new();
            //if (fromCommon)
            //{
            //    rsa.ImportFromPem(Common.PublicKey);
            //    return Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(data), true));
            //}
            string ImportPath = null;
            if (string.IsNullOrEmpty(ImportPath))
            {
                ImportPath = $"{Directory.GetCurrentDirectory()}//{AppSettingJwt.Jwt.RSAKeysFolderName}";
            }
            // Load public key from file or string
            string publicKeyPath = $"{ImportPath}\\{AppSettingJwt.Jwt.PublicKeyFileName}.pem";
            string publicKey = File.ReadAllText(publicKeyPath, Encoding.ASCII);
            // Import the recipient's public key
            rsa.ImportFromPem(publicKey);
            // Encrypt the data using the public key
            return Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(data), true));
        }
        public static string DecryptData(string data)
        {
            // Create an instance of RSACryptoServiceProvider
            using RSACryptoServiceProvider rsa = LoadRsaPrivateKey();
            // Decrypt the data using the private key
            byte[] decryptedData = rsa.Decrypt(Convert.FromBase64String(data), true);
            // Convert the decrypted data back to its original form
            return Encoding.UTF8.GetString(decryptedData);
        }
        private static void ExportPrivateKeyPem(RSACryptoServiceProvider csp, TextWriter outputStream)
        {
            if (csp.PublicOnly) throw new ArgumentException("CSP does not contain a private key", "csp");
            var parameters = csp.ExportParameters(true);
            using var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write((byte)0x30); // SEQUENCE
            using (var innerStream = new MemoryStream())
            {
                var innerWriter = new BinaryWriter(innerStream);
                EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                EncodeIntegerBigEndian(innerWriter, parameters.D);
                EncodeIntegerBigEndian(innerWriter, parameters.P);
                EncodeIntegerBigEndian(innerWriter, parameters.Q);
                EncodeIntegerBigEndian(innerWriter, parameters.DP);
                EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                var length = (int)innerStream.Length;
                EncodeLength(writer, length);
                writer.Write(innerStream.GetBuffer(), 0, length);
            }

            var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
            outputStream.WriteLine("-----BEGIN RSA PRIVATE KEY-----");
            // Output as Base64 with lines chopped at 64 characters
            for (var i = 0; i < base64.Length; i += 64)
            {
                outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
            }
            outputStream.WriteLine("-----END RSA PRIVATE KEY-----");
        }
        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }
        private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }
        private static void ExportPublicKeyPem(RSACryptoServiceProvider csp, TextWriter outputStream)
        {
            var parameters = csp.ExportParameters(false);
            using var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write((byte)0x30); // SEQUENCE
            using (var innerStream = new MemoryStream())
            {
                var innerWriter = new BinaryWriter(innerStream);
                innerWriter.Write((byte)0x30); // SEQUENCE
                EncodeLength(innerWriter, 13);
                innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
                var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
                EncodeLength(innerWriter, rsaEncryptionOid.Length);
                innerWriter.Write(rsaEncryptionOid);
                innerWriter.Write((byte)0x05); // NULL
                EncodeLength(innerWriter, 0);
                innerWriter.Write((byte)0x03); // BIT STRING
                using (var bitStringStream = new MemoryStream())
                {
                    var bitStringWriter = new BinaryWriter(bitStringStream);
                    bitStringWriter.Write((byte)0x00); // # of unused bits
                    bitStringWriter.Write((byte)0x30); // SEQUENCE
                    using (var paramsStream = new MemoryStream())
                    {
                        var paramsWriter = new BinaryWriter(paramsStream);
                        EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); // Modulus
                        EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); // Exponent
                        var paramsLength = (int)paramsStream.Length;
                        EncodeLength(bitStringWriter, paramsLength);
                        bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                    }
                    var bitStringLength = (int)bitStringStream.Length;
                    EncodeLength(innerWriter, bitStringLength);
                    innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
                }
                var length = (int)innerStream.Length;
                EncodeLength(writer, length);
                writer.Write(innerStream.GetBuffer(), 0, length);
            }
            var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
            outputStream.WriteLine("-----BEGIN PUBLIC KEY-----");
            for (var i = 0; i < base64.Length; i += 64)
            {
                outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
            }
            outputStream.WriteLine("-----END PUBLIC KEY-----");
        }
    }
}
