using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// general utility to show message dialog
    /// </summary>
    public class MessageDialog : EditorWindow
    {
        private static string _templatePath;
        private static bool _dialogResult;

        public static bool DisplayDialog(string title, string message, string ok, string cancel = "")
        {
#if UNITY_EDITOR_OSX
            _dialogResult = EditorUtility.DisplayDialog(title, message, ok, cancel);
#else
            MessageDialog window = GetWindow<MessageDialog>();
            window.titleContent = new GUIContent(title);
            window.Initialize(message, ok, cancel);
            window.minSize = new Vector2(480, 160);
            window.maxSize = new Vector2(500, 180);
            _dialogResult = false;
            window.ShowModal();
#endif
            return _dialogResult;
        }

        private void Initialize(string message, string ok, string cancel)
        {
            if (string.IsNullOrEmpty(_templatePath))
            {
                string pluginDirectory = GlobalPathUtility.GetPluginPath();
                _templatePath = GlobalPathUtility.CombinePath(pluginDirectory, "Editor/Templates/message-dialog-template.uxml");
            }
            VisualElement root = rootVisualElement;
            VisualTreeAsset visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_templatePath);
            visualAsset.CloneTree(root);
            root.Q<Label>("message-label").text = message;
            Button okButton = root.Q<Button>("ok-button");
            okButton.text = ok;
            okButton.RegisterCallback<ClickEvent>(OnClickOK);
            Button cancelButton = root.Q<Button>("cancel-button");
            if (string.IsNullOrEmpty(cancel))
            {
                cancelButton.RemoveFromHierarchy();
            }
            else
            {
                cancelButton.text = cancel;
                cancelButton.RegisterCallback<ClickEvent>(OnClickCancel);
            }
        }

        private void OnClickOK(ClickEvent evt)
        {
            _dialogResult = true;
            Close();
        }

        private void OnClickCancel(ClickEvent evt)
        {
            _dialogResult = false;
            Close();
        }
    }
}