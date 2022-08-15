using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webIEA.Contracts
{
    public interface IHashManager
    {
        List<byte[]> HashWithSalt(string password);
        bool VerifyPasswordWithSaltAndStoredHash(string password, byte[] storedHash, byte[] storedSalt);
        string EncryptPlainText(string plainText);
        string DecryptCipherText(string cipherText);
    }
}
