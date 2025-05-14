using System.IO;
using System.Security.Cryptography;
using System.Text;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// example script to demonstrate the simple usage of the generated Protobuf classes
    /// </summary>
    public class ProtobufUsageExample : MonoBehaviour
    {
        //this is only for demonstration, save your key and iv in a safe place in the production environment
        private const string AESKey = "kWQcEF2edS9Qui8LE9ZF%6$8ui*maF!E";
        private const string AESIV = "LqbX9En72%Z0qiEj";

        public InputField InputTextField;
        public Button SaveToMemoryButton;
        public Button LoadFromMemoryButton;
        public Button SaveToFileButton;
        public Button LoadFromFileButton;
        public Button SaveWithCryptoButton;
        public Button LoadWithCryptoButton;

        private byte[] _memory;
        private ProtobufExampleCache _memoryCache;

        private string _savePath;
        private ProtobufExampleCache _fileCache;

        private byte[] _cryptoMemory;
        private ProtobufExampleCache _cryptoCache;

        private void Start()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "ProtobufManager", "protobuf_example_cache.pb");
            string directory = Path.GetDirectoryName(_savePath);
            //ensure the directory exist before you save anything
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            SaveToMemoryButton.onClick.AddListener(OnSaveToMemory);
            LoadFromMemoryButton.onClick.AddListener(OnLoadFromMemory);
            SaveToFileButton.onClick.AddListener(OnSaveToFile);
            LoadFromFileButton.onClick.AddListener(OnLoadFromFile);
            SaveWithCryptoButton.onClick.AddListener(OnSaveWithCrypto);
            LoadWithCryptoButton.onClick.AddListener(OnLoadWithCrypto);
        }

        private void OnSaveToMemory()
        {
            if (!string.IsNullOrEmpty(InputTextField.text))
            {
                if (_memoryCache == null)
                {
                    _memoryCache = new ProtobufExampleCache();
                }
                //fill the protobuf instance
                _memoryCache.InputText = InputTextField.text;
                //get the serialized bytes using ToByteArray()
                _memory = _memoryCache.ToByteArray();
                InputTextField.text = string.Empty;
            }
        }

        private void OnLoadFromMemory()
        {
            if (_memory != null)
            {
                //parse the saved data to a protobuf instance using Parser.ParseFrom()
                ProtobufExampleCache cache = ProtobufExampleCache.Parser.ParseFrom(_memory);
                InputTextField.text = cache.InputText;
            }
        }

        private void OnSaveToFile()
        {
            if (!string.IsNullOrEmpty(InputTextField.text))
            {
                if (_fileCache == null)
                {
                    _fileCache = new ProtobufExampleCache();
                }
                _fileCache.InputText = InputTextField.text;
                using (FileStream stream = File.Create(_savePath))
                {
                    //save the protobuf instance to a stream directly using WriteTo()
                    _fileCache.WriteTo(stream);
                }
                InputTextField.text = string.Empty;
            }
        }

        private void OnLoadFromFile()
        {
            if (File.Exists(_savePath))
            {
                using (FileStream stream = File.OpenRead(_savePath))
                {
                    //parse the saved data to a protobuf instance using Parser.ParseFrom()
                    ProtobufExampleCache cache = ProtobufExampleCache.Parser.ParseFrom(stream);
                    InputTextField.text = cache.InputText;
                }
            }
        }

        private void OnSaveWithCrypto()
        {
            if (!string.IsNullOrEmpty(InputTextField.text))
            {
                if (_cryptoCache == null)
                {
                    _cryptoCache = new ProtobufExampleCache();
                }
                _cryptoCache.InputText = InputTextField.text;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(AESKey);
                    aes.IV = Encoding.UTF8.GetBytes(AESIV);
                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                //protobuf instance can write to a CryptoStream as well
                                _cryptoCache.WriteTo(cryptoStream);
                            }
                            _cryptoMemory = memoryStream.ToArray();
                        }
                    }
                }

                InputTextField.text = string.Empty;
            }
        }

        private void OnLoadWithCrypto()
        {
            if (_cryptoMemory != null)
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(AESKey);
                    aes.IV = Encoding.UTF8.GetBytes(AESIV);
                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    {
                        using (MemoryStream memoryStream = new MemoryStream(_cryptoMemory))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                //protobuf instance can parsed from a CryptoStream as well
                                ProtobufExampleCache cache = ProtobufExampleCache.Parser.ParseFrom(cryptoStream);
                                InputTextField.text = cache.InputText;
                            }
                        }
                    }
                }
            }
        }
    }
}