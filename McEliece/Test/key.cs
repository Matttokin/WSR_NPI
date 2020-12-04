using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VTDev.Libraries.CEXEngine.Crypto.Cipher.Asymmetric.Interfaces;

namespace Test
{
    public class key : IAsymmetricKeyPair
    {
        public string Name => throw new NotImplementedException();

        public IAsymmetricKey PublicKey => throw new NotImplementedException();

        public IAsymmetricKey PrivateKey => throw new NotImplementedException();

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
