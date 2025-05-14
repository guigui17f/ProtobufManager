using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using GUIGUI17F.ProtobufManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;

namespace GUIGUI17F.Test
{
    public class TestWindow : EditorWindow
    {
        [MenuItem("Test/ShowWindow")]
        private static void ShowWindow()
        {
            TestWindow window = GetWindow<TestWindow>();
            window.titleContent = new GUIContent("ShowWindow");
            window.Show();
        }

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualTreeAsset visualAsset = Resources.Load<VisualTreeAsset>("test-window");
            visualAsset.CloneTree(root);
            root.Q<Button>("test-button").RegisterCallback<ClickEvent>(OnClick);
        }

        private void OnClick(ClickEvent evt)
        {
        }

        private void TestProtoPlayerPrefs()
        {
            ProtoPlayerPrefsWrapper wrapper = new ProtoPlayerPrefsWrapper();
            wrapper.SetInt("TestInt",1);
            Debug.Log(wrapper.GetInt("TestInt"));
            Debug.Log(wrapper.HasInt("TestInt"));
            wrapper.DeleteInt("TestInt");
            Debug.Log(wrapper.HasInt("TestInt"));
            
            wrapper.SetFloat("TestFloat",2.0f);
            Debug.Log(wrapper.GetFloat("TestFloat"));
            Debug.Log(wrapper.HasFloat("TestFloat"));
            wrapper.DeleteFloat("TestFloat");
            Debug.Log(wrapper.GetFloat("TestFloat"));
            
            wrapper.SetString("TestString","AAA");
            Debug.Log(wrapper.GetString("TestString"));
            Debug.Log(wrapper.HasString("TestString"));
            wrapper.DeleteString("TestString");
            Debug.Log(wrapper.HasString("TestString"));

            byte[] data = new byte[10];
            ByteString protoData = ByteString.CopyFrom(data);
            
            wrapper.SetByteString("TestByteString", protoData);
            Debug.Log(wrapper.GetByteString("TestByteString").Length);
            Debug.Log(wrapper.HasByteString("TestByteString"));
            wrapper.DeleteByteString("TestByteString");
            Debug.Log(wrapper.HasByteString("TestByteString"));
            
            wrapper.SetBytes("TestByteString", data);
            Debug.Log(wrapper.GetBytes("TestByteString").Length);
            Debug.Log(wrapper.HasByteString("TestByteString"));
            wrapper.DeleteByteString("TestByteString");
            Debug.Log(wrapper.HasByteString("TestByteString"));
            
            wrapper.SetString("TestSave","Save");

            string path = Path.Combine(Application.persistentDataPath, "cache.pb");
            wrapper.Save(path);
            ProtoPlayerPrefsWrapper newWrapper = ProtoPlayerPrefsWrapper.Load(path);
            Debug.Log(newWrapper.GetString("TestSave"));
        }

        private void GenerateProtos()
        {
            ProtoCompilerUtility.GenerateScripts(
                new List<string>
                {
                    "Assets/GUIGUI17F/ProtobufManager/Editor/Protos/enum_definition.proto",
                    "Assets/GUIGUI17F/ProtobufManager/Editor/Protos/map_definition.proto",
                    "Assets/GUIGUI17F/ProtobufManager/Editor/Protos/message_definition.proto",
                    "Assets/GUIGUI17F/ProtobufManager/Editor/Protos/field_definition.proto",
                    "Assets/GUIGUI17F/ProtobufManager/Editor/Protos/oneof_definition.proto",
                    "Assets/GUIGUI17F/ProtobufManager/Editor/Protos/proto_file_definition.proto"
                },
                new ScriptGenerationConfig
                {
                    Language = TargetLanguage.CSharp,
                    Path = "Assets/GUIGUI17F/ProtobufManager/Editor/Scripts/Models/StorageData"
                });
        }
    }
}