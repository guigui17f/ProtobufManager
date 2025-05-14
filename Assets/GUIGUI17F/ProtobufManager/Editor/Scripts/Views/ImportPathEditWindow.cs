using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// editor window used for manage the protobuf ImportPath configs
    /// </summary>
    public class ImportPathEditWindow : EditorWindow
    {
        private static string _templatePath;

        private VisualElement _pathRoot;
        private List<string> _defaultImportPaths;
        private List<string> _currentImportPaths;

        public static void ShowWindow(List<string> pathList)
        {
            ImportPathEditWindow window = GetWindow<ImportPathEditWindow>();
            window.titleContent = new GUIContent("ImportPath Edit Window");
            window.minSize = new Vector2(480, 240);
            window.Show();
            window.AddImportPaths(pathList);
        }

        private void CreateGUI()
        {
            _defaultImportPaths = ProtobufManagerConfigs.GetDefaultImportPaths();
            _currentImportPaths = ProtobufManagerConfigs.GetImportPaths(false);
            if (string.IsNullOrEmpty(_templatePath))
            {
                _templatePath = GlobalPathUtility.GetVisualTemplatePath("import-path-edit-template.uxml");
            }
            VisualElement root = rootVisualElement;
            VisualTreeAsset visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_templatePath);
            visualAsset.CloneTree(root);
            _pathRoot = root.Q<VisualElement>("path-root");
            root.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            root.Q<Label>("google-path-label").text = _defaultImportPaths[0];
            root.Q<Label>("unity-path-label").text = _defaultImportPaths[1];
            root.Q<Button>("add-button").RegisterCallback<ClickEvent>(OnAddImportPath);
            foreach (string path in _currentImportPaths)
            {
                AddImportPathTag(path);
            }
        }

        private void AddImportPaths(List<string> pathList)
        {
            if (pathList != null)
            {
                foreach (string path in pathList)
                {
                    string importPath = GlobalPathUtility.GetRelativePath(path);
                    if (!_currentImportPaths.Contains(importPath) && !_defaultImportPaths.Contains(importPath))
                    {
                        _currentImportPaths.Add(importPath);
                        AddImportPathTag(importPath);
                    }
                }
            }
        }

        private void OnAddImportPath(ClickEvent evt)
        {
            string path = EditorUtility.OpenFolderPanel("Choose Import Path", Application.dataPath, string.Empty);
            if (!string.IsNullOrEmpty(path))
            {
                path = GlobalPathUtility.GetRelativePath(path);
                if (_defaultImportPaths.Contains(path) || _currentImportPaths.Contains(path))
                {
                    MessageDialog.DisplayDialog("Warning", "This path already exist in the list!", "OK");
                }
                else
                {
                    _currentImportPaths.Add(path);
                    AddImportPathTag(path);
                }
            }
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            ProtobufManagerConfigs configs = ProtobufManagerConfigs.GetConfigData();
            configs.ImportPaths.Clear();
            configs.ImportPaths.AddRange(_currentImportPaths);
            EditorUtility.SetDirty(configs);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void AddImportPathTag(string path)
        {
            LabelTagElement tag = new LabelTagElement(path);
            _pathRoot.Add(tag);
            tag.RegisterCallback<DetachFromPanelEvent>(OnTagDetachFromRoot);
        }

        private void OnTagDetachFromRoot(DetachFromPanelEvent evt)
        {
            if (evt.target is LabelTagElement tag)
            {
                _currentImportPaths.Remove(tag.LabelText);
            }
        }
    }
}