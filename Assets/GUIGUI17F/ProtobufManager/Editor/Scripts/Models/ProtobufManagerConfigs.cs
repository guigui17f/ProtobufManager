using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// ProtobufManager config file
    /// </summary>
    public class ProtobufManagerConfigs : ScriptableObject
    {
        private static string _configPath;
        private static List<string> _defaultImportPaths;

        public List<string> ImportPaths;

        public static ProtobufManagerConfigs GetConfigData()
        {
            if (string.IsNullOrEmpty(_configPath))
            {
                string pluginDirectory = GlobalPathUtility.GetPluginPath();
                _configPath = GlobalPathUtility.CombinePath(pluginDirectory, "Editor/ProtobufManagerConfigs.asset");
            }
            return AssetDatabase.LoadAssetAtPath<ProtobufManagerConfigs>(_configPath);
        }

        public static List<string> GetDefaultImportPaths()
        {
            if (_defaultImportPaths == null)
            {
                _defaultImportPaths = new List<string>();
                string pluginDirectory = GlobalPathUtility.GetPluginPath();
                _defaultImportPaths.Add(GlobalPathUtility.CombinePath(pluginDirectory, "Editor/Compiler/include"));
                _defaultImportPaths.Add(GlobalPathUtility.CombinePath(pluginDirectory, "Editor/Compiler/unity_extensions"));
                _defaultImportPaths.Add("Assets");
            }
            return _defaultImportPaths;
        }

        public static List<string> GetImportPaths(bool includeDefault)
        {
            List<string> importPaths = new List<string>();
            if (includeDefault)
            {
                importPaths.AddRange(GetDefaultImportPaths());
            }
            ProtobufManagerConfigs configs = GetConfigData();
            foreach (string path in configs.ImportPaths)
            {
                if (!string.IsNullOrEmpty(path) && !importPaths.Contains(path))
                {
                    importPaths.Add(path);
                }
            }
            importPaths.Sort((a, b) => b.Length - a.Length);
            return importPaths;
        }
    }
}