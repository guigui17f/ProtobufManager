using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    public class ImportFromJsonWindow : EditorWindow
    {
        private static string _templatePath;

        private TextField _jsonField;

        public static void ShowWindow()
        {
            ImportFromJsonWindow window = GetWindow<ImportFromJsonWindow>();
            window.titleContent = new GUIContent("Import From Json Window");
            window.minSize = new Vector2(480, 360);
            window.Show();
        }

        private void CreateGUI()
        {
            if (string.IsNullOrEmpty(_templatePath))
            {
                _templatePath = GlobalPathUtility.GetVisualTemplatePath("import-from-json-template.uxml");
            }
            VisualElement root = rootVisualElement;
            VisualTreeAsset visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_templatePath);
            visualAsset.CloneTree(root);
            _jsonField = root.Q<TextField>("json-field");
            root.Q<Button>("import-button").RegisterCallback<ClickEvent>(OnImportFromJson);
        }

        private void OnImportFromJson(ClickEvent evt)
        {
            string jsonText = _jsonField.value.Trim();
            if (!string.IsNullOrEmpty(jsonText))
            {
                ProtoFileDefinition definition = ProtoEditorUtility.ConvertJsonToCache(jsonText);
                if (definition != null)
                {
                    ProtobufEditWindow.ShowWindow(definition);
                    Close();
                }
                else
                {
                    MessageDialog.DisplayDialog("Warning", "Parse failed, please input a valid json text!", "OK");
                }
            }
            else
            {
                MessageDialog.DisplayDialog("Warning", "Please input a valid json text!", "OK");
            }
        }
    }
}