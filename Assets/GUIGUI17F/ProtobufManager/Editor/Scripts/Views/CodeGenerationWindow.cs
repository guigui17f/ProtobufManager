using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    public class CodeGenerationWindow : EditorWindow
    {
        private static readonly Dictionary<string, TargetLanguage> ChoiceLanguageDictionary = new Dictionary<string, TargetLanguage>
        {
            { "C++", TargetLanguage.Cpp },
            { "C#", TargetLanguage.CSharp },
            { "Java", TargetLanguage.Java },
            { "Kotlin", TargetLanguage.Kotlin },
            { "Objective-C", TargetLanguage.Objc },
            { "PHP", TargetLanguage.PHP },
            { "Python", TargetLanguage.Python },
            { "Ruby", TargetLanguage.Ruby },
            { "Dart", TargetLanguage.Dart },
            { "Go", TargetLanguage.Go }
        };

        private static string _templatePath;

        private ScrollView _protoRoot;
        private CompatibleDropdownField _languageDropdown;
        private TextField _parameterField;
        private TextField _pathField;
        private List<string> _protoList;
        private string _lastImportDirectory;

        public static void ShowWindow(List<string> protoList)
        {
            CodeGenerationWindow window = GetWindow<CodeGenerationWindow>();
            window.titleContent = new GUIContent("Code Generation Window");
            window.minSize = new Vector2(640, 360);
            window.Show();
            window.InitializeProtoList(protoList);
        }

        private void CreateGUI()
        {
            if (string.IsNullOrEmpty(_templatePath))
            {
                _templatePath = GlobalPathUtility.GetVisualTemplatePath("code-generation-template.uxml");
            }
            _protoList = new List<string>();
            _lastImportDirectory = Application.dataPath;
            VisualElement root = rootVisualElement;
            VisualTreeAsset visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_templatePath);
            visualAsset.CloneTree(root);
            _protoRoot = root.Q<ScrollView>("proto-root");
            _languageDropdown = root.Q<CompatibleDropdownField>("language-dropdown");
            _parameterField = root.Q<TextField>("parameter-field");
            _pathField = root.Q<TextField>("path-field");
            root.Q<Button>("add-proto-button").RegisterCallback<ClickEvent>(OnAddProto);
            root.Q<Button>("browse-path-button").RegisterCallback<ClickEvent>(OnBrowsePath);
            root.Q<Button>("generate-button").RegisterCallback<ClickEvent>(OnGenerateCodes);
        }

        private void InitializeProtoList(List<string> protoList)
        {
            if (protoList != null)
            {
                foreach (string proto in protoList)
                {
                    string protoPath = GlobalPathUtility.GetRelativePath(proto);
                    if (!_protoList.Contains(protoPath))
                    {
                        _protoList.Add(protoPath);
                        LabelTagElement tag = new LabelTagElement(protoPath);
                        _protoRoot.Add(tag);
                        tag.RegisterCallback<DetachFromPanelEvent>(OnProtoTagDetachFromPanel);
                    }
                }
            }
        }

        private void OnProtoTagDetachFromPanel(DetachFromPanelEvent evt)
        {
            if (evt.target is LabelTagElement protoTag)
            {
                _protoList.Remove(protoTag.LabelText);
            }
        }

        private void OnAddProto(ClickEvent evt)
        {
            string proto = EditorUtility.OpenFilePanel("Choose Proto File", _lastImportDirectory, "proto");
            if (!string.IsNullOrEmpty(proto))
            {
                _lastImportDirectory = Path.GetDirectoryName(proto);
                string protoPath = GlobalPathUtility.GetRelativePath(proto);
                if (_protoList.Contains(protoPath))
                {
                    MessageDialog.DisplayDialog("Warning", "This proto already imported!", "OK");
                }
                else
                {
                    _protoList.Add(protoPath);
                    LabelTagElement tag = new LabelTagElement(protoPath);
                    _protoRoot.Add(tag);
                    tag.RegisterCallback<DetachFromPanelEvent>(OnProtoTagDetachFromPanel);
                }
            }
        }

        private void OnBrowsePath(ClickEvent evt)
        {
            string path = EditorUtility.SaveFolderPanel("Choose Save Folder", Application.dataPath, "Generated");
            if (!string.IsNullOrEmpty(path))
            {
                _pathField.SetValueWithoutNotify(GlobalPathUtility.GetRelativePath(path));
            }
        }

        private void OnGenerateCodes(ClickEvent evt)
        {
            if (_protoList.Count <= 0)
            {
                MessageDialog.DisplayDialog("Warning", "Please choose at least one proto file!", "OK");
                return;
            }
            string path = _pathField.value.Trim();
            if (string.IsNullOrEmpty(path))
            {
                MessageDialog.DisplayDialog("Warning", "Please choose the target folder where the codes should generated in!", "OK");
                return;
            }
            ScriptGenerationConfig config = new ScriptGenerationConfig
            {
                Language = ChoiceLanguageDictionary[_languageDropdown.value],
                ExtraParameters = _parameterField.value,
                Path = path
            };
            bool hasError = !ProtoCompilerUtility.GenerateScripts(_protoList, config);
            if (hasError)
            {
                MessageDialog.DisplayDialog("Warning", "Got compiler error information, if your scripts do not generated properly, check the console for details.", "OK");
            }
            EditorUtility.RevealInFinder(path);
        }
    }
}