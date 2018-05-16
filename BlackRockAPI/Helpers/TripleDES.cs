using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BlackRockAPI.Helpers
{
    public class TripleDES
    {
        private byte[] key = {
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14,
        15,
        16,
        17,
        18,
        19,
        20,
        21,
        22,
        23,
        24
    };
        private byte[] iv = {
        65,
        110,
        68,
        26,
        69,
        178,
        200,
        219

    };
        public byte[] Encrypt(string plainText)
        {
            // Declare a UTF8Encoding object so we may use the GetByte 
            // method to transform the plainText into a Byte array. 
            UTF8Encoding utf8encoder = new UTF8Encoding();
            byte[] inputInBytes = utf8encoder.GetBytes(plainText);

            // Create a new TripleDES service provider 
            TripleDESCryptoServiceProvider tdesProvider = new TripleDESCryptoServiceProvider();

            // The ICryptTransform interface uses the TripleDES 
            // crypt provider along with encryption key and init vector 
            // information 
            ICryptoTransform cryptoTransform = tdesProvider.CreateEncryptor(this.key, this.iv);

            // All cryptographic functions need a stream to output the 
            // encrypted information. Here we declare a memory stream 
            // for this purpose. 
            MemoryStream encryptedStream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Write);

            // Write the encrypted information to the stream. Flush the information 
            // when done to ensure everything is out of the buffer. 
            cryptStream.Write(inputInBytes, 0, inputInBytes.Length);
            cryptStream.FlushFinalBlock();
            encryptedStream.Position = 0;

            // Read the stream back into a Byte array and return it to the calling 
            // method.
            byte[] result = new byte[encryptedStream.Length];
            encryptedStream.Read(result, 0, (int)encryptedStream.Length);
            cryptStream.Close();
            return result;
        }

        public string Decrypt(byte[] inputInBytes)
        {
            // UTFEncoding is used to transform the decrypted Byte Array 
            // information back into a string. 
            if (inputInBytes == null)
            {
                return null;
            }
            UTF8Encoding utf8encoder = new UTF8Encoding();
            TripleDESCryptoServiceProvider tdesProvider = new TripleDESCryptoServiceProvider();

            // As before we must provide the encryption/decryption key along with 
            // the init vector. 
            ICryptoTransform cryptoTransform = tdesProvider.CreateDecryptor(this.key, this.iv);

            // Provide a memory stream to decrypt information into 
            MemoryStream decryptedStream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(decryptedStream, cryptoTransform, CryptoStreamMode.Write);
            cryptStream.Write(inputInBytes, 0, inputInBytes.Length);
            cryptStream.FlushFinalBlock();
            decryptedStream.Position = 0;

            // Read the memory stream and convert it back into a string 
            byte[] result = new byte[decryptedStream.Length];
            decryptedStream.Read(result, 0, (int)decryptedStream.Length);
            cryptStream.Close();
            UTF8Encoding myutf = new UTF8Encoding();
            return myutf.GetString(result, 0, result.Length);
        }

        /// <summary>
        /// Convert byte array into a string of numbers padded to 3 spaces for each byte.
        /// </summary>
        /// <param name="inBytes"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string BytesToStringPadded(byte[] inBytes)
        {
            string result = "";
            string tmp = "";
            foreach (byte byt in inBytes)
            {
                tmp = Convert.ToString(byt).PadLeft(3);
                result = result + tmp;
            }
            return result;
        }

        /// <summary>
        /// Convert string that was padded to 3 spaces for each byte, into a byte array.
        /// </summary>
        /// <param name="inString"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] StringToBytesPadded(string inString)
        {
            int ct = -1;
            byte[] result = null;
            string tempString = inString;
            string tmp = "";
            do
            {
                if (string.IsNullOrEmpty(tempString))
                    break;
                tmp = tempString.Substring(0, 3).Replace(" ", "");
                tempString = tempString.Substring(3, tempString.Length - 3);
                ct = ct + 1;
                Array.Resize(ref result, ct + 1);
                result[ct] = Convert.ToByte(tmp);
            } while (true);
            return result;
        }
    }
}