namespace Bit
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;


    public class encodedString
    {
        public string encodedJsonString { get; set; }
    }

    public static class AesEncryptor
    {
        // Инициализируем ключ для шифрования и расшифрования данных
        private static readonly byte[] Key = new byte[]
        {
        0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
         0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF,
         0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
         0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
        };
        

        public static string Encrypt(string plainText)
        {
            // Инициализируем объект класса Aes для шифрования данных
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Получаем IV, используемый для шифрования
                byte[] iv = aes.IV;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Добавляем IV в начало потока
                    memoryStream.Write(iv, 0, iv.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        // Шифруем данные и записываем в поток
                        streamWriter.Write(plainText);
                    }

                    // Возвращаем зашифрованные данные в виде Base64-строки
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Получаем IV из начала массива зашифрованных данных
                byte[] iv = new byte[aes.IV.Length];
                Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);

                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes, iv.Length, encryptedBytes.Length - iv.Length))
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (StreamReader streamReader = new StreamReader(cryptoStream))
                {
                    // Расшифровываем данные и возвращаем их
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
