using System.Security.Cryptography;
using System.Text;

namespace Personal_Sitios.Helpers
{
    public class EncryptionHelper
    {
        private readonly IConfiguration _configuration;

        public EncryptionHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Encriptar(string textoPlano)
        {
            string keyString = _configuration["Encryption:Key"];

            byte[] key = Encoding.UTF8.GetBytes(keyString);
            byte[] nonce = RandomNumberGenerator.GetBytes(12);
            byte[] textoBytes = Encoding.UTF8.GetBytes(textoPlano);
            byte[] cipherText = new byte[textoBytes.Length];
            byte[] tag = new byte[16];

            using var aes = new AesGcm(key, 16);

            aes.Encrypt(
                nonce,
                textoBytes,
                cipherText,
                tag
            );

            return "AESGCM:" +
                   Convert.ToBase64String(nonce) + ":" +
                   Convert.ToBase64String(tag) + ":" +
                   Convert.ToBase64String(cipherText);
        }

        public bool Verificar(string textoPlano, string textoEncriptado)
        {
            if (string.IsNullOrWhiteSpace(textoEncriptado))
            {
                return false;
            }

            /*
             * Esto permite seguir usando usuarios de prueba viejos
             * que estén guardados como texto plano.
             * Los nuevos usuarios sí se guardan con AES GCM.
             */
            if (!textoEncriptado.StartsWith("AESGCM:"))
            {
                return textoPlano == textoEncriptado;
            }

            string textoDesencriptado = Desencriptar(textoEncriptado);

            return textoPlano == textoDesencriptado;
        }

        private string Desencriptar(string textoEncriptado)
        {
            string keyString = _configuration["Encryption:Key"];

            byte[] key = Encoding.UTF8.GetBytes(keyString);

            string limpio = textoEncriptado.Replace("AESGCM:", "");

            string[] partes = limpio.Split(':');

            byte[] nonce = Convert.FromBase64String(partes[0]);
            byte[] tag = Convert.FromBase64String(partes[1]);
            byte[] cipherText = Convert.FromBase64String(partes[2]);

            byte[] textoPlano = new byte[cipherText.Length];

            using var aes = new AesGcm(key, 16);

            aes.Decrypt(
                nonce,
                cipherText,
                tag,
                textoPlano
            );

            return Encoding.UTF8.GetString(textoPlano);
        }
    }
}