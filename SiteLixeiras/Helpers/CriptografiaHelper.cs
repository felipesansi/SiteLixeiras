using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace SiteLixeiras.Helpers
{
    public class CriptografiaHelper
    {
        private readonly byte[] _chave;
        private readonly byte[] _iv;

        public CriptografiaHelper(IConfiguration configuracao)
        {
            _chave = Convert.FromBase64String(configuracao["Criptografia:ChaveBase64"]);
            _iv = Convert.FromBase64String(configuracao["Criptografia:IVBase64"]);
        }

        public string Criptografar(string textoClaro)
        {
            if (string.IsNullOrEmpty(textoClaro)) return string.Empty;

            using var aes = Aes.Create();
            aes.Key = _chave;
            aes.IV = _iv;

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(textoClaro);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Descriptografar(string textoCriptografado)
        {
            if (string.IsNullOrEmpty(textoCriptografado)) return string.Empty;

            try
            {
                var bytesCriptografados = Convert.FromBase64String(textoCriptografado);

                using var aes = Aes.Create();
                aes.Key = _chave;
                aes.IV = _iv;

                using var ms = new MemoryStream(bytesCriptografados);
                using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch (FormatException)
            {
                // String não estava em formato Base64 válido
                return textoCriptografado;
            }
            catch (CryptographicException)
            {
                // Os dados não foram criptografados corretamente com a chave/IV
                return textoCriptografado;
            }
        }
    }
}
