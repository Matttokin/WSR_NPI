using System;
using VTDev.Libraries.CEXEngine.Crypto.Cipher.Asymmetric.Encrypt.McEliece;
using VTDev.Libraries.CEXEngine.Crypto.Cipher.Asymmetric.Interfaces;
using VTDev.Libraries.CEXEngine.Tools;
using VTDev.Projects.CEX.Test.Tests.Asymmetric.McEliece;
using System.Diagnostics;
using VTDev.Libraries.CEXEngine.Crypto.Prng;
using System.Text;
using Newtonsoft.Json;

namespace Test
{
    static class Program
    {
        const int CYCLE_COUNT = 10;
        const string CON_TITLE = "MPKC> ";

        static void Main(string[] args)
        {
            

            MPKCParameters encParams = (MPKCParameters)MPKCParamSets.MPKCFM11T40S256.DeepCopy();
            MPKCKeyGenerator keyGen = new MPKCKeyGenerator(encParams);
            IAsymmetricKeyPair keyPair = keyGen.GenerateKeyPair();

            byte[] enc, dec, data;
            string message = "Привер Валера";
            // encrypt an array
            using (MPKCEncrypt cipher = new MPKCEncrypt(encParams))
            {
                cipher.Initialize(keyPair.PublicKey);
                data = Encoding.Default.GetBytes(message);

                enc = cipher.Encrypt(data);
            }
            Console.WriteLine("Исходный текст " + message);
            var b64encStr = Convert.ToBase64String(enc);
            Console.WriteLine("Зашифрованный текст " + b64encStr);
            var b64encArr = Convert.FromBase64String(b64encStr);
            // decrypt the cipher text
            using (MPKCEncrypt cipher = new MPKCEncrypt(encParams))
            {
                cipher.Initialize(keyPair.PrivateKey);
                dec = cipher.Decrypt(b64encArr);
            }

            Console.WriteLine("Расшифрованный текст " + Encoding.Default.GetString(dec));
            Console.ReadKey();

        }

        

        }
}
