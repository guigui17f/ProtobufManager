using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    public class ProtobufEditWindow : EditorWindow
    {
        private static string _templatePath;

        private VisualElement _importRoot;
        private VisualElement _enumRoot;
        private VisualElement _messageRoot;
        private TextField _packageNameField;
        private TextField _namespaceField;
        private TextField _otherOptionsField;
        private CompatibleDropdownField _optimizeOptionField;
        private bool _initialized;
        private StringBuilder _builder;
        private List<string> _typeList;
        private List<ProtoMessageElement> _messageElementCache;
        private string _lastImportDirectory;

        public static void ShowWindow(ProtoFileDefinition cacheData)
        {
            ProtobufEditWindow window = GetWindow<ProtobufEditWindow>();
            window.titleContent = new GUIContent("Protobuf Edit Window");
            window.saveChangesMessage = "Do you want to save current workspace as the editor cache?";
            window.minSize = new Vector2(640, 480);
            window.Show();
            window.RecoverCacheData(cacheData);
        }

        public override void SaveChanges()
        {
            ProtoFileDefinition definition = GetDefinitionData();
            ProtoEditorUtility.SaveEditorWindowCache(definition, false);
            base.SaveChanges();
        }

        private void CreateGUI()
        {
            if (!_initialized)
            {
                ProtoEditorUtility.ResetBaseTime();
            }

            Initialize();

            //recover the workspace after the script compilation finished
            if (_initialized)
            {
                ProtoFileDefinition cacheData = ProtoEditorUtility.LoadEditorWindowCache(true);
                RecoverCacheData(cacheData);
            }
            _initialized = true;
        }

        private void Initialize()
        {
            _builder = new StringBuilder();
            _typeList = ProtoEditorUtility.GetBuiltInTypeList();
            _messageElementCache = new List<ProtoMessageElement>();
            string pluginDirectory = GlobalPathUtility.GetPluginPath();
            _lastImportDirectory = GlobalPathUtility.CombinePath(pluginDirectory, "Editor/Compiler/include");
            CompilationPipeline.compilationStarted += OnCompilationStarted;

            if (string.IsNullOrEmpty(_templatePath))
            {
                _templatePath = GlobalPathUtility.GetVisualTemplatePath("protobuf-edit-template.uxml");
            }
            VisualElement root = rootVisualElement;
            VisualTreeAsset visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_templatePath);
            visualAsset.CloneTree(root);
            _importRoot = root.Q<VisualElement>("import-root");
            _enumRoot = root.Q<VisualElement>("enum-root");
            _messageRoot = root.Q<VisualElement>("message-root");
            _packageNameField = root.Q<TextField>("package-name-field");
            _namespaceField = root.Q<TextField>("namespace-field");
            _optimizeOptionField = root.Q<CompatibleDropdownField>("optimize-option-field");
            _otherOptionsField = root.Q<TextField>("other-options-field");
            root.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanelEvent);
            _packageNameField.RegisterCallback<FocusOutEvent>(OnPackageNameFocusOut);
            _namespaceField.RegisterCallback<FocusOutEvent>(OnNamespaceFocusOut);
            _otherOptionsField.RegisterCallback<FocusOutEvent>(OnOtherOptionsFocusOut);
            root.Q<Button>("add-import-button").RegisterCallback<ClickEvent>(OnAddProtoImport);
            root.Q<Button>("add-enum-button").RegisterCallback<ClickEvent>(OnAddEnum);
            root.Q<Button>("add-message-button").RegisterCallback<ClickEvent>(OnAddMessage);
            root.Q<Button>("sort-element-button").RegisterCallback<ClickEvent>(OnClickSort);
            root.Q<Button>("revert-deletion-button").RegisterCallback<ClickEvent>(OnRevertElementDeletion);
            root.Q<Button>("generate-proto-button").RegisterCallback<ClickEvent>(OnGenerateProtoFile);
        }

        private void RecoverCacheData(ProtoFileDefinition cacheData)
        {
            if (cacheData != null)
            {
                hasUnsavedChanges = true;
                _packageNameField.SetValueWithoutNotify(cacheData.PackageName);
                List<string> importPaths = ProtobufManagerConfigs.GetImportPaths(true);
                foreach (string proto in cacheData.ImportProtos)
                {
                    string fullPath = ConvertImportProtoToFullPath(importPaths, proto);
                    if (!string.IsNullOrEmpty(fullPath))
                    {
                        List<string> protoTypes = ProtoEditorUtility.GetProtoTypes(fullPath);
                        if (protoTypes != null)
                        {
                            ProtoImportTagElement importElement = new ProtoImportTagElement(_typeList, proto, protoTypes);
                            _importRoot.Add(importElement);
                        }
                    }
                }
                ImportFileTypes(cacheData, _typeList);
                _namespaceField.SetValueWithoutNotify(cacheData.CsharpNamespace);
                _optimizeOptionField.index = cacheData.OptimizeType;
                _otherOptionsField.SetValueWithoutNotify(cacheData.OtherOptions);

                foreach (EnumDefinition enumDefinition in cacheData.EnumDefinitions)
                {
                    var element = ProtoEnumElement.CreateElement();
                    _enumRoot.Add(element.visualRoot);
                    element.enumElement.Initialize(_typeList, null, enumDefinition);
                }
                foreach (MessageDefinition message in cacheData.MessageDefinitions)
                {
                    var element = ProtoMessageElement.CreateElement();
                    _messageRoot.Add(element.visualRoot);
                    element.messageElement.Initialize(_typeList, null, message);
                }
            }
        }

        private void OnCompilationStarted(object context)
        {
            ProtoFileDefinition definition = GetDefinitionData();
            ProtoEditorUtility.SaveEditorWindowCache(definition, true);
        }

        private void OnDetachFromPanelEvent(DetachFromPanelEvent evt)
        {
            CompilationPipeline.compilationStarted -= OnCompilationStarted;
        }

        private void OnPackageNameFocusOut(FocusOutEvent evt)
        {
            _packageNameField.SetValueWithoutNotify(_packageNameField.value.Trim());
        }

        private void OnNamespaceFocusOut(FocusOutEvent evt)
        {
            _namespaceField.SetValueWithoutNotify(_namespaceField.value.Trim());
        }

        private void OnOtherOptionsFocusOut(FocusOutEvent evt)
        {
            _optimizeOptionField.SetValueWithoutNotify(_optimizeOptionField.value.Trim());
        }

        private void OnAddProtoImport(ClickEvent evt)
        {
            string path = EditorUtility.OpenFilePanel("Choose Proto File", _lastImportDirectory, "proto");
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                List<string> importPaths = ProtobufManagerConfigs.GetImportPaths(true);
                string proto = ConvertFullPathToImportProto(importPaths, path);
                if (string.IsNullOrEmpty(proto))
                {
                    MessageDialog.DisplayDialog("Warning", "The importing proto file must be under the \"Assets\" folder!", "OK");
                }
                else
                {
                    _lastImportDirectory = Path.GetDirectoryName(path);
                    List<string> protoTypes = ProtoEditorUtility.GetProtoTypes(path);
                    if (protoTypes == null)
                    {
                        MessageDialog.DisplayDialog("Warning", "No valid message type found, please check your proto file!", "OK");
                    }
                    else
                    {
                        bool valid = true;
                        foreach (string protoType in protoTypes)
                        {
                            if (_typeList.Contains(protoType))
                            {
                                valid = false;
                                break;
                            }
                        }
                        if (!valid)
                        {
                            MessageDialog.DisplayDialog("Warning", "Types in this proto file is duplicated with imported type list, won't import it.", "OK");
                        }
                        else
                        {
                            ProtoImportTagElement importElement = new ProtoImportTagElement(_typeList, proto, protoTypes);
                            _importRoot.Add(importElement);
                        }
                    }
                }
            }
        }

        private void OnAddEnum(ClickEvent evt)
        {
            hasUnsavedChanges = true;
            var element = ProtoEnumElement.CreateElement();
            element.enumElement.Initialize(_typeList, null);
            _enumRoot.Add(element.visualRoot);
        }

        private void OnAddMessage(ClickEvent evt)
        {
            hasUnsavedChanges = true;
            var element = ProtoMessageElement.CreateElement();
            element.messageElement.Initialize(_typeList, null);
            _messageRoot.Add(element.visualRoot);
        }

        private void OnClickSort(ClickEvent evt)
        {
            SortElements();
        }

        private void OnRevertElementDeletion(ClickEvent evt)
        {
            CustomUndoRedoManager.Instance.PerformUndo();
        }

        private void OnGenerateProtoFile(ClickEvent evt)
        {
            if (CheckValidation())
            {
                string path = EditorUtility.SaveFilePanel("Choose File Save Path", Application.dataPath, "file_name", "proto");
                if (!string.IsNullOrEmpty(path))
                {
                    ProtoFileDefinition definition = GetDefinitionData();
                    ProtoGenerationUtility.GenerateProtoFiles(definition, path);
                    bool openWindow = MessageDialog.DisplayDialog("Info", "Proto file generated, do you want to open the script generation window?", "OK", "Cancel");
                    if (openWindow)
                    {
                        List<string> protoList = new List<string> { path };
                        CodeGenerationWindow.ShowWindow(protoList);
                    }
                    else
                    {
                        EditorUtility.RevealInFinder(path);
                    }
                    SaveChanges();
                    Close();
                }
            }
            else
            {
                MessageDialog.DisplayDialog("Warning", "Data validation failed, check the console for detail information.", "OK");
            }
        }

        private bool CheckValidation()
        {
            _builder.Clear();
            bool pass = CheckOtherOptions(_builder);
            if (pass)
            {
                List<ProtoEnumElement> enums = _enumRoot.Query<ProtoEnumElement>().ToList();
                for (int i = 0; i < enums.Count; i++)
                {
                    if (!enums[i].CheckValidation(string.Empty, _builder))
                    {
                        pass = false;
                        break;
                    }
                }
            }
            if (pass)
            {
                _messageRoot.QueryChildrenElement(_messageElementCache);
                for (int i = 0; i < _messageElementCache.Count; i++)
                {
                    if (!_messageElementCache[i].CheckValidation(string.Empty, _builder))
                    {
                        pass = false;
                        break;
                    }
                }
            }
            if (!pass)
            {
                Debug.LogError(_builder.ToString());
            }
            return pass;
        }

        private ProtoFileDefinition GetDefinitionData()
        {
            SortElements();
            ProtoFileDefinition data = new ProtoFileDefinition
            {
                PackageName = _packageNameField.value,
                CsharpNamespace = _namespaceField.value,
                OptimizeType = _optimizeOptionField.index,
                OtherOptions = _otherOptionsField.value
            };
            _importRoot.Query<ProtoImportTagElement>().ForEach(item => data.ImportProtos.Add(item.ImportProto));
            _enumRoot.Query<ProtoEnumElement>().ForEach(item => data.EnumDefinitions.Add(item.GetDefinitionData()));
            _messageRoot.QueryChildrenElement(_messageElementCache).ForEach(item => data.MessageDefinitions.Add(item.GetDefinitionData()));
            return data;
        }

        private bool CheckOtherOptions(StringBuilder builder)
        {
            string otherOptions = _otherOptionsField.value;
            if (string.IsNullOrEmpty(otherOptions))
            {
                return true;
            }
            string[] options = otherOptions.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < options.Length; i++)
            {
                string option = options[i].Trim();
                if (!option.StartsWith("option") || !option.EndsWith(";"))
                {
                    builder.AppendLine("There is a syntax error in the OtherOptions field.");
                    return false;
                }
            }
            return true;
        }

        private void SortElements()
        {
            _enumRoot.Sort((a, b) =>
            {
                if (a.childCount == 0 && b.childCount == 0)
                {
                    return 0;
                }
                if (b.childCount == 0)
                {
                    return -1;
                }
                if (a.childCount == 0)
                {
                    return 1;
                }
                ProtoEnumElement x = a.Q<ProtoEnumElement>();
                ProtoEnumElement y = b.Q<ProtoEnumElement>();
                x.SortElements();
                y.SortElements();
                if (string.IsNullOrEmpty(x.EnumName) && string.IsNullOrEmpty(y.EnumName))
                {
                    return 0;
                }
                if (string.IsNullOrEmpty(y.EnumName))
                {
                    return -1;
                }
                if (string.IsNullOrEmpty(x.EnumName))
                {
                    return 1;
                }
                return string.CompareOrdinal(x.EnumName, y.EnumName);
            });
            _messageRoot.Sort((a, b) =>
            {
                if (a.childCount == 0 && b.childCount == 0)
                {
                    return 0;
                }
                if (b.childCount == 0)
                {
                    return -1;
                }
                if (a.childCount == 0)
                {
                    return 1;
                }
                ProtoMessageElement x = a.Q<ProtoMessageElement>();
                ProtoMessageElement y = b.Q<ProtoMessageElement>();
                x.SortElements();
                y.SortElements();
                if (string.IsNullOrEmpty(x.MessageName) && string.IsNullOrEmpty(y.MessageName))
                {
                    return 0;
                }
                if (string.IsNullOrEmpty(y.MessageName))
                {
                    return -1;
                }
                if (string.IsNullOrEmpty(x.MessageName))
                {
                    return 1;
                }
                return string.CompareOrdinal(x.MessageName, y.MessageName);
            });
        }

        private string ConvertImportProtoToFullPath(List<string> importPaths, string importProto)
        {
            foreach (string importPath in importPaths)
            {
                string fullPath = Path.Combine(importPath, importProto);
                if (File.Exists(fullPath))
                {
                    return fullPath.Replace('\\', '/');
                }
            }
            return string.Empty;
        }

        private string ConvertFullPathToImportProto(List<string> importPaths, string fullPath)
        {
            fullPath = GlobalPathUtility.GetRelativePath(fullPath);
            foreach (string importPath in importPaths)
            {
                if (fullPath.StartsWith(importPath))
                {
                    return fullPath.Substring(importPath.Length + 1);
                }
            }
            return string.Empty;
        }

        private void ImportFileTypes(ProtoFileDefinition fileData, List<string> typeList)
        {
            foreach (EnumDefinition enumData in fileData.EnumDefinitions)
            {
                if (!string.IsNullOrEmpty(enumData.EnumName))
                {
                    typeList.Add(enumData.EnumName);
                }
            }
            foreach (MessageDefinition messageData in fileData.MessageDefinitions)
            {
                ImportMessageTypes(messageData, string.Empty, typeList);
            }
        }

        private void ImportMessageTypes(MessageDefinition messageData, string parentName, List<string> typeList)
        {
            if (string.IsNullOrEmpty(messageData.MessageName))
            {
                return;
            }
            _builder.Clear();
            if (!string.IsNullOrEmpty(parentName))
            {
                _builder.Append(parentName);
                _builder.Append('.');
            }
            _builder.Append(messageData.MessageName);
            string messageName = _builder.ToString();
            typeList.Add(messageName);

            foreach (EnumDefinition enumData in messageData.EnumDefinitions)
            {
                _builder.Clear();
                _builder.Append(messageName);
                _builder.Append('.');
                _builder.Append(enumData.EnumName);
                typeList.Add(_builder.ToString());
            }
            foreach (MessageDefinition nestedType in messageData.MessageDefinitions)
            {
                ImportMessageTypes(nestedType, messageName, typeList);
            }
        }
    }
}