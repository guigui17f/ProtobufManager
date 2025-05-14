using System.IO;
using UnityEditor;
using UnityEngine;

namespace GUIGUI17F.ProtobufManager
{
    public class ProtobufBarMenu
    {
        [MenuItem("Tools/Protobuf Manager/Download Compiler")]
        private static void DownloadCompiler()
        {
            Application.OpenURL("https://github.com/protocolbuffers/protobuf/releases");
        }

        [MenuItem("Tools/Protobuf Manager/Handle Protos/Create New Proto")]
        private static void CreateNewProto()
        {
            if (!ProtoCompilerUtility.IsCompilerExist())
            {
                return;
            }
            ProtobufEditWindow.ShowWindow(null);
        }

        [MenuItem("Tools/Protobuf Manager/Handle Protos/Load Previous Workspace")]
        private static void LoadPreviousWorkspace()
        {
            if (!ProtoCompilerUtility.IsCompilerExist())
            {
                return;
            }
            ProtoFileDefinition cacheData = ProtoEditorUtility.LoadEditorWindowCache(false);
            ProtobufEditWindow.ShowWindow(cacheData);
        }

        [MenuItem("Tools/Protobuf Manager/Handle Protos/Import from Proto")]
        private static void ImportFromProto()
        {
            if (!ProtoCompilerUtility.IsCompilerExist())
            {
                return;
            }
            string path = EditorUtility.OpenFilePanel("Choose Proto File", Application.dataPath, "proto");
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                ProtoFileDefinition cacheData = ProtoEditorUtility.ConvertProtoToCache(path);
                if (cacheData != null)
                {
                    ProtobufEditWindow.ShowWindow(cacheData);
                }
                else
                {
                    MessageDialog.DisplayDialog("Warning", "Parse failed, please choose a valid proto file.", "OK");
                }
            }
        }

        [MenuItem("Tools/Protobuf Manager/Handle Protos/Import from Json")]
        private static void ImportFromJson()
        {
            if (!ProtoCompilerUtility.IsCompilerExist())
            {
                return;
            }
            ImportFromJsonWindow.ShowWindow();
        }

        [MenuItem("Tools/Protobuf Manager/Generate Codes")]
        private static void GenerateCodes()
        {
            if (!ProtoCompilerUtility.IsCompilerExist())
            {
                return;
            }
            CodeGenerationWindow.ShowWindow(null);
        }

        [MenuItem("Tools/Protobuf Manager/Utilities/Manage ImportPath")]
        private static void ManageImportPath()
        {
            ImportPathEditWindow.ShowWindow(null);
        }

        [MenuItem("Tools/Protobuf Manager/Utilities/Open PersistentDataPath")]
        private static void OpenPersistentPath()
        {
            if (Directory.Exists(Application.persistentDataPath))
            {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }
            else
            {
                Debug.LogError("The persistentDataPath doesn't exist!");
            }
        }
    }
}