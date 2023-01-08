using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CryptographyExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read the plain text from the console
            Console.WriteLine("Enter the plain text:");
            string plainText = Console.ReadLine();

            // Generate a random key
            byte[] key = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }

            // Initialize the encrypted text variable with an empty byte array
            byte[] encryptedText = new byte[0];

            // Display the menu and handle the user's choice
            while (true)
            {
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Encrypt the text");
                Console.WriteLine("2. Save the encrypted text to a file");
                Console.WriteLine("3. Load the encrypted text from a file");
                Console.WriteLine("4. Decrypt the text");
                Console.WriteLine("5. Exit the program");
                Console.Write("Enter your choice: ");

                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        // Encrypt the plain text
                        encryptedText = EncryptText(plainText, key);
                        Console.WriteLine("Encrypted text: " + Convert.ToBase64String(encryptedText));
                        break;
                    case 2:
                        // Save the encrypted text to a file
                        string filePath = "encrypted.txt";
                        File.WriteAllBytes(filePath, encryptedText);
                        Console.WriteLine("Encrypted text saved to file.");
                        break;
                    case 3:
                        // Load the encrypted text from a file
                        filePath = "encrypted.txt";
                        encryptedText = File.ReadAllBytes(filePath);
                        Console.WriteLine("Encrypted text loaded from file: " + Convert.ToBase64String(encryptedText));
                        break;
                    case 4:
                        // Decrypt the text
                        string decryptedText = DecryptText(encryptedText, key);
                        Console.WriteLine("Decrypted text: " + decryptedText);
                        break;
                    case 5:
                        // Exit the program
                        return;
                }
            }
        }

        static byte[] EncryptText(string plainText, byte[] key)
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.GenerateIV();

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                    byte[] encryptedText = new byte[aes.IV.Length + encryptedBytes.Length];
                    Array.Copy(aes.IV, 0, encryptedText, 0, aes.IV.Length);
                    Array.Copy(encryptedBytes, 0, encryptedText, aes.IV.Length, encryptedBytes.Length);

                    return encryptedText;
                }
            }
        }
        static string DecryptText(byte[] encryptedText, byte[] key)
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                byte[] iv = new byte[aes.IV.Length];
                Array.Copy(encryptedText, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] encryptedBytes = new byte[encryptedText.Length - aes.IV.Length];
                    Array.Copy(encryptedText, aes.IV.Length, encryptedBytes, 0, encryptedBytes.Length);
                    byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    string plainText = Encoding.UTF8.GetString(plainBytes);
                    return plainText;
                }
            }
        }
    }
}

