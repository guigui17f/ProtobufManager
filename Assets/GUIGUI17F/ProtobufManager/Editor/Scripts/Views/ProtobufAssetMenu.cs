using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace GUIGUI17F.ProtobufManager
{
    public class ProtobufAssetMenu
    {
        [MenuItem("Assets/Protobuf Manager/Import from Proto")]
        private static void ImportFromProto()
        {
            if (!ProtoCompilerUtility.IsCompilerExist())
            {
                return;
            }
            List<string> pathList = ProtoEditorUtility.GetSelectionFilePaths(".proto");
            if (pathList.Count > 0)
            {
                ProtoFileDefinition cacheData = ProtoEditorUtility.ConvertProtoToCache(pathList[0]);
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

        [MenuItem("Assets/Protobuf Manager/Generate Codes")]
        private static void GenerateCodes()
        {
            if (!ProtoCompilerUtility.IsCompilerExist())
            {
                return;
            }
            List<string> pathList = ProtoEditorUtility.GetSelectionFilePaths(".proto");
            if (pathList.Count > 0)
            {
                CodeGenerationWindow.ShowWindow(pathList);
            }
        }

        [MenuItem("Assets/Protobuf Manager/Add to ImportPath")]
        private static void AddImportPath()
        {
            List<string> pathList = new List<string>();
            string[] guids = Selection.assetGUIDs;
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (Directory.Exists(path))
                {
                    pathList.Add(path);
                }
            }
            if (pathList.Count > 0)
            {
                ImportPathEditWindow.ShowWindow(pathList);
            }
        }
    }
}