using Sodium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidDaysServerLibrary.Services
{
    public class UserService
    {

        public string EncryptString(string message, byte[] key)
        {
            var nonce = SecretBox.GenerateNonce();
            var secretMessage = SecretBox.Create(message, nonce, key);

            byte[] rv = new byte[nonce.Length + secretMessage.Length];
            System.Buffer.BlockCopy(nonce, 0, rv, 0, nonce.Length);
            System.Buffer.BlockCopy(secretMessage, 0, rv, nonce.Length, secretMessage.Length);
            return Utilities.BinaryToHex(rv);
        }
        public string EncryptPassword(string password)
        {
            return PasswordHash.ScryptHashString(password, PasswordHash.Strength.Medium);
        }
        public string Decrypt(string cipher, byte[] key)
        {

            byte[] cipherBytes = Utilities.HexToBinary(cipher);
            var nonce = cipherBytes.Take(24).ToArray();
            var messageBytes = cipherBytes.Skip(24).ToArray();
            var message = Utilities.BinaryToHex(messageBytes);

            var secret = SecretBox.Open(messageBytes, nonce, key);
            var secretMessage = Encoding.UTF8.GetString(secret);
            return secretMessage;
        }

        public bool VerifyPasswordHash(string password, string hash)
        {
            try
            {
                if (PasswordHash.ScryptHashStringVerify(hash, password))
                {
                    return true;
                }
                return false;
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception after hash verifiy --");
                Console.WriteLine(e.Message);

            }
            return false;
        }
    }
}
