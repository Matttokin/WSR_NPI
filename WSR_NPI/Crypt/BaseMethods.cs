using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using VTDev.Libraries.CEXEngine.Crypto.Cipher.Asymmetric.Encrypt.McEliece;
using VTDev.Libraries.CEXEngine.Crypto.Cipher.Asymmetric.Interfaces;

namespace WSR_NPI.Crypt
{
    public class BaseMethods
    {
        byte[] enc, dec, data;
        MPKCParameters encParams;
        MPKCKeyGenerator keyGen;
        IAsymmetricKeyPair keyPair;
        HelpMethods hM = new HelpMethods();
        public BaseMethods()
        {
            encParams = (MPKCParameters)MPKCParamSets.MPKCFM11T40S256.DeepCopy();
            keyGen = new MPKCKeyGenerator(encParams);
            keyPair = keyGen.GenerateKeyPair();
        }
        public string Encrypt(string plainText)
        {
            try
            {
                return hM.Encrypt(plainText);
            }
            catch
            {
                return "";
            }
        }
        public string Decrypt(string encText)
        {
            try
            {
                return hM.Decrypt(encText);
            } catch
            {
                return "";
            }
        }




        
        
    }
}