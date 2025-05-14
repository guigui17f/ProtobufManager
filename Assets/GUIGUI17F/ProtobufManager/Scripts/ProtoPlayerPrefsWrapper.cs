using System.Collections.Generic;
using System.IO;
using Google.Protobuf;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// a wrapper used for save and load simple data, just like the built-in PlayerPrefs in Unity Engine
    /// data will hold in memory, until you call the Save() method
    /// </summary>
    public class ProtoPlayerPrefsWrapper
    {
        private Dictionary<string, int> _intDictionary;
        private Dictionary<string, float> _floatDictionary;
        private Dictionary<string, string> _stringDictionary;
        private Dictionary<string, ByteString> _byteStringDictionary;
        private ProtoPlayerPrefs _rawData;

        public static ProtoPlayerPrefsWrapper Load(string path)
        {
            ProtoPlayerPrefs playerPrefs = LoadRawData(path);
            if (playerPrefs != null)
            {
                return new ProtoPlayerPrefsWrapper(playerPrefs);
            }
            return null;
        }

        public static void Save(ProtoPlayerPrefsWrapper wrapper, string path)
        {
            SaveRawData(wrapper.GetRawData(), path);
        }

        public static ProtoPlayerPrefs LoadRawData(string path)
        {
            if (File.Exists(path))
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    return ProtoPlayerPrefs.Parser.ParseFrom(stream);
                }
            }
            return null;
        }

        public static void SaveRawData(ProtoPlayerPrefs playerPrefs, string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (FileStream stream = File.Create(path))
            {
                playerPrefs.WriteTo(stream);
            }
        }

        public ProtoPlayerPrefsWrapper() : this(null)
        {
        }

        public ProtoPlayerPrefsWrapper(ProtoPlayerPrefs playerPrefs)
        {
            if (playerPrefs == null)
            {
                _rawData = new ProtoPlayerPrefs();
            }
            else
            {
                _rawData = playerPrefs;
            }
            _intDictionary = new Dictionary<string, int>();
            for (int i = 0; i < _rawData.IntKeys.Count; i++)
            {
                _intDictionary[_rawData.IntKeys[i]] = _rawData.IntValues[i];
            }
            _floatDictionary = new Dictionary<string, float>();
            for (int i = 0; i < _rawData.FloatKeys.Count; i++)
            {
                _floatDictionary[_rawData.FloatKeys[i]] = _rawData.FloatValues[i];
            }
            _stringDictionary = new Dictionary<string, string>();
            for (int i = 0; i < _rawData.StringKeys.Count; i++)
            {
                _stringDictionary[_rawData.StringKeys[i]] = _rawData.StringValues[i];
            }
            _byteStringDictionary = new Dictionary<string, ByteString>();
            for (int i = 0; i < _rawData.BytesKeys.Count; i++)
            {
                _byteStringDictionary[_rawData.BytesKeys[i]] = _rawData.BytesValues[i];
            }
        }

        public ProtoPlayerPrefs GetRawData()
        {
            _rawData.IntKeys.Clear();
            _rawData.IntValues.Clear();
            foreach (KeyValuePair<string, int> pair in _intDictionary)
            {
                _rawData.IntKeys.Add(pair.Key);
                _rawData.IntValues.Add(pair.Value);
            }
            _rawData.FloatKeys.Clear();
            _rawData.FloatValues.Clear();
            foreach (KeyValuePair<string, float> pair in _floatDictionary)
            {
                _rawData.FloatKeys.Add(pair.Key);
                _rawData.FloatValues.Add(pair.Value);
            }
            _rawData.StringKeys.Clear();
            _rawData.StringValues.Clear();
            foreach (KeyValuePair<string, string> pair in _stringDictionary)
            {
                _rawData.StringKeys.Add(pair.Key);
                _rawData.StringValues.Add(pair.Value);
            }
            _rawData.BytesKeys.Clear();
            _rawData.BytesValues.Clear();
            foreach (KeyValuePair<string, ByteString> pair in _byteStringDictionary)
            {
                _rawData.BytesKeys.Add(pair.Key);
                _rawData.BytesValues.Add(pair.Value);
            }
            return _rawData;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            if (_intDictionary.TryGetValue(key, out int value))
            {
                return value;
            }
            return defaultValue;
        }

        public float GetFloat(string key, float defaultValue = 0)
        {
            if (_floatDictionary.TryGetValue(key, out float value))
            {
                return value;
            }
            return defaultValue;
        }

        public string GetString(string key, string defaultValue = "")
        {
            if (_stringDictionary.TryGetValue(key, out string value))
            {
                return value;
            }
            return defaultValue;
        }

        public ByteString GetByteString(string key, ByteString defaultValue = null)
        {
            if (_byteStringDictionary.TryGetValue(key, out ByteString value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// it's just a wrapper of GetByteString() and has extra cost, use GetByteString() directly if possible
        /// </summary>
        public byte[] GetBytes(string key, byte[] defaultValue = null)
        {
            if (_byteStringDictionary.TryGetValue(key, out ByteString value))
            {
                return value.ToByteArray();
            }
            return defaultValue;
        }

        public void SetInt(string key, int value)
        {
            _intDictionary[key] = value;
        }

        public void SetFloat(string key, float value)
        {
            _floatDictionary[key] = value;
        }

        public void SetString(string key, string value)
        {
            _stringDictionary[key] = value;
        }

        public void SetByteString(string key, ByteString value)
        {
            _byteStringDictionary[key] = value;
        }

        /// <summary>
        /// it's just a wrapper of SetByteString() and has extra cost, use SetByteString() directly if possible
        /// </summary>
        public void SetBytes(string key, byte[] value)
        {
            _byteStringDictionary[key] = ByteString.CopyFrom(value);
        }

        public bool HasInt(string key)
        {
            return _intDictionary.ContainsKey(key);
        }

        public bool HasFloat(string key)
        {
            return _floatDictionary.ContainsKey(key);
        }

        public bool HasString(string key)
        {
            return _stringDictionary.ContainsKey(key);
        }

        public bool HasByteString(string key)
        {
            return _byteStringDictionary.ContainsKey(key);
        }

        public void DeleteInt(string key)
        {
            _intDictionary.Remove(key);
        }

        public void DeleteFloat(string key)
        {
            _floatDictionary.Remove(key);
        }

        public void DeleteString(string key)
        {
            _stringDictionary.Remove(key);
        }

        public void DeleteByteString(string key)
        {
            _byteStringDictionary.Remove(key);
        }

        public void DeleteAll()
        {
            _intDictionary.Clear();
            _floatDictionary.Clear();
            _stringDictionary.Clear();
            _byteStringDictionary.Clear();
        }

        public void Save(string path)
        {
            Save(this, path);
        }
    }
}