using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GUIGUI17F.ProtobufManager
{
    public class GlobalPathUtility
    {
        private static string _dataPath = Application.dataPath.Replace('\\', '/');
        private static StringBuilder _builder = new StringBuilder();
        private static string _projectDirectory;
        private static string _pluginDirectory;

        /// <summary>
        /// get the absolute path of current project (parent of the Assets directory)
        /// </summary>
        public static string GetProjectPath()
        {
            if (string.IsNullOrEmpty(_projectDirectory))
            {
                string path = Application.dataPath;
                _projectDirectory = path.Substring(0, path.Length - 7);
            }
            return _projectDirectory;
        }

        /// <summary>
        /// get the relative path of the ProtobufManager plugin
        /// </summary>
        public static string GetPluginPath()
        {
            if (string.IsNullOrEmpty(_pluginDirectory))
            {
                _pluginDirectory = GetPluginPath("proto_player_prefs", "Editor/Compiler/unity_extensions/proto_player_prefs.proto");
            }
            return _pluginDirectory;
        }

        /// <summary>
        /// get the VisualTreeAsset load path of the ProtobufManager plugin
        /// </summary>
        public static string GetVisualTemplatePath(string templateName)
        {
            string pluginDirectory = GetPluginPath();
            return CombinePath(pluginDirectory, "Editor/Templates", templateName);
        }

        public static string GetRelativePath(string path)
        {
            path = path.Replace('\\', '/');
            if (path.StartsWith(_dataPath))
            {
                path = path.Substring(_dataPath.Length - 6);
            }
            return path;
        }

        public static string CombinePath(params string[] paths)
        {
            _builder.Clear();
            _builder.Append(Path.Combine(paths));
            _builder.Replace('\\', '/');
            return _builder.ToString();
        }

        private static string GetPluginPath(string anchorFileName, string fileRelativePath)
        {
            fileRelativePath = fileRelativePath.Replace('\\', '/');
            string[] guids = AssetDatabase.FindAssets(anchorFileName);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]).Replace('\\', '/');
                if (path.Contains(fileRelativePath))
                {
                    return path.Substring(0, path.Length - fileRelativePath.Length - 1);
                }
            }
            Debug.LogError("Cannot find plugin root directory, please reimport the Protobuf Manager!");
            return null;
        }
    }
}