using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    public class ProtoEnumElement : VisualElement
    {
        private static string _templatePath;

        public VisualElement VisualRoot { get; private set; }
        public string EnumName => _nameField.value;

        private Button _foldButton;
        private Label _shrinkLabel;
        private VisualElement _detailRoot;
        private VisualElement _elementRoot;
        private TextField _nameField;
        private Toggle _aliasToggle;
        private TextField _reservedField;
        private StringBuilder _builder;
        private List<string> _valueNames;
        private List<int> _valueNumbers;
        private List<string> _typeList;
        private TypeNameString _lastName;

        public static (VisualElement visualRoot, ProtoEnumElement enumElement) CreateElement()
        {
            if (string.IsNullOrEmpty(_templatePath))
            {
                _templatePath = GlobalPathUtility.GetVisualTemplatePath("enum-template.uxml");
            }
            VisualTreeAsset visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_templatePath);
            VisualElement visualRoot = visualAsset.Instantiate();
            ProtoEnumElement enumElement = visualRoot.Q<ProtoEnumElement>();
            enumElement.VisualRoot = visualRoot;
            return (visualRoot, enumElement);
        }

        public void Initialize(List<string> typeList, Func<string> parentNameGetter)
        {
            _typeList = typeList;
            _builder = new StringBuilder();
            _valueNames = new List<string>();
            _valueNumbers = new List<int>();
            _lastName = new TypeNameString(parentNameGetter, _builder);
            _foldButton = this.Q<Button>("fold-button");
            VisualElement foldRoot = this.Q<VisualElement>("fold-root");
            _shrinkLabel = foldRoot.Q<Label>("shrink-label");
            _detailRoot = foldRoot.Q<VisualElement>("detail-root");
            _elementRoot = _detailRoot.Q<VisualElement>("element-root");
            _nameField = _detailRoot.Q<TextField>("name-field");
            _aliasToggle = _detailRoot.Q<Toggle>("allow-alias-toggle");
            _reservedField = _detailRoot.Q<TextField>("reserved-field");
            _foldButton.RegisterCallback<ClickEvent>(OnClickFold);
            _nameField.RegisterCallback<FocusOutEvent>(OnNameFieldFocusOut);
            _detailRoot.Q<Button>("add-button").RegisterCallback<ClickEvent>(OnClickAdd);
            _detailRoot.Q<Button>("sort-button").RegisterCallback<ClickEvent>(OnClickSort);
            _shrinkLabel.style.display = DisplayStyle.None;
        }

        public void Initialize(List<string> typeList, Func<string> parentNameGetter, EnumDefinition definition)
        {
            Initialize(typeList, parentNameGetter);
            UpdateEnumName(definition.EnumName, true);
            _aliasToggle.SetValueWithoutNotify(definition.AllowAlias);
            for (int i = 0; i < definition.ValueNames.Count; i++)
            {
                NameValueElement element = new NameValueElement();
                element.SetNameWithoutNotify(definition.ValueNames[i]);
                element.SetValueWithoutNotify(definition.ValueNumbers[i].ToString());
                _elementRoot.Add(element);
            }
            _builder.Clear();
            foreach (string reservedNumber in definition.ReservedNumbers)
            {
                _builder.Append(reservedNumber);
                _builder.Append(',');
            }
            foreach (string reservedName in definition.ReservedNames)
            {
                _builder.Append(reservedName);
                _builder.Append(',');
            }
            if (_builder.Length > 0)
            {
                _builder.Remove(_builder.Length - 1, 1);
            }
            _reservedField.SetValueWithoutNotify(_builder.ToString());
        }

        public bool CheckValidation(string typeName, StringBuilder logBuilder)
        {
            if (string.IsNullOrEmpty(EnumName))
            {
                logBuilder.AppendLine($"{typeName}: enum name is empty!");
                return false;
            }

            _builder.Clear();
            if (!string.IsNullOrEmpty(typeName))
            {
                _builder.Append(typeName);
                _builder.Append('.');
            }
            _builder.Append(EnumName);
            string selfTypeName = _builder.ToString();
            if (ProtoEditorUtility.BuiltInTypes.Contains(EnumName))
            {
                logBuilder.AppendLine($"{selfTypeName}: enum name is illegal!");
                return false;
            }
            List<NameValueElement> values = _elementRoot.Query<NameValueElement>().ToList();
            if (values.Count <= 0)
            {
                logBuilder.AppendLine($"{selfTypeName}: enum contains no value!");
                return false;
            }
            _valueNames.Clear();
            _valueNumbers.Clear();
            bool allowAlias = _aliasToggle.value;
            bool hasZeroNumber = false;
            bool hasAlias = false;
            for (int i = 0; i < values.Count; i++)
            {
                if (!values[i].CheckValidation(selfTypeName, "enum value", logBuilder))
                {
                    return false;
                }
                if (_valueNames.Contains(values[i].DataName))
                {
                    logBuilder.AppendLine($"{selfTypeName} - {values[i].DataName}: enum contains duplicated value name!");
                    return false;
                }
                _valueNames.Add(values[i].DataName);

                int number = int.Parse(values[i].DataValue);
                hasZeroNumber |= (number == 0);
                if (_valueNumbers.Contains(number))
                {
                    if (!allowAlias)
                    {
                        logBuilder.AppendLine($"{selfTypeName} - {values[i].DataName}: enum contains duplicated value {values[i].DataValue}!");
                        return false;
                    }
                    hasAlias = true;
                }
                else
                {
                    _valueNumbers.Add(number);
                }
            }
            if (!hasZeroNumber)
            {
                logBuilder.AppendLine($"{selfTypeName}: enum must contain a 0 value!");
                return false;
            }
            if (allowAlias && !hasAlias)
            {
                logBuilder.AppendLine($"{selfTypeName}: enum contains no alias values, please turn off the \"allow_alias\" option!");
                return false;
            }
            return true;
        }

        public EnumDefinition GetDefinitionData()
        {
            SortElements();
            EnumDefinition data = new EnumDefinition { EnumName = EnumName, AllowAlias = _aliasToggle.value };
            _elementRoot.Query<NameValueElement>().ForEach(item =>
            {
                data.ValueNames.Add(item.DataName);
                bool hasValue = int.TryParse(item.DataValue, out int dataValue);
                data.ValueNumbers.Add(hasValue ? dataValue : 0);
            });
            if (!string.IsNullOrEmpty(_reservedField.value))
            {
                string[] reserves = _reservedField.value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < reserves.Length; i++)
                {
                    reserves[i] = reserves[i].Trim();
                    if (reserves[i].Length > 0)
                    {
                        if (reserves[i][0] == '"')
                        {
                            data.ReservedNames.Add(reserves[i]);
                        }
                        else
                        {
                            data.ReservedNumbers.Add(reserves[i]);
                        }
                    }
                }
            }
            return data;
        }

        public void SortElements()
        {
            _elementRoot.Sort((a, b) =>
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
                NameValueElement x = a as NameValueElement;
                NameValueElement y = b as NameValueElement;
                if (string.IsNullOrEmpty(x.DataValue) && string.IsNullOrEmpty(y.DataValue))
                {
                    return 0;
                }
                if (string.IsNullOrEmpty(y.DataValue))
                {
                    return -1;
                }
                if (string.IsNullOrEmpty(x.DataValue))
                {
                    return 1;
                }
                return int.Parse(x.DataValue) - int.Parse(y.DataValue);
            });
        }

        public void HandleParentNameChanged()
        {
            _lastName.UpdateName(EnumName, false);
            UpdateTypeList(false);
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);
            if (_typeList != null)
            {
                if (evt.eventTypeId == AttachToPanelEvent.TypeId())
                {
                    _lastName.UpdateName(EnumName, true);
                    if (!string.IsNullOrEmpty(_lastName.CurrentInHierarchyName))
                    {
                        if (_typeList.Contains(_lastName.CurrentInHierarchyName))
                        {
                            _builder.Clear();
                            _builder.Append(EnumName);
                            _builder.Append(ProtoEditorUtility.GetWindowShownTime());
                            UpdateEnumName(_builder.ToString(), true);
                        }
                        _typeList.Add(_lastName.CurrentInHierarchyName);
                    }
                }
                else if (evt.eventTypeId == DetachFromPanelEvent.TypeId())
                {
                    _typeList.Remove(_lastName.CurrentInHierarchyName);
                }
            }
        }

        private void OnClickFold(ClickEvent evt)
        {
            if (_foldButton.text == "-")
            {
                _builder.Clear();
                _builder.Append("enum ");
                _builder.Append(EnumName);
                _builder.Append(" { …… }");
                _shrinkLabel.text = _builder.ToString();
                _detailRoot.style.display = DisplayStyle.None;
                _shrinkLabel.style.display = DisplayStyle.Flex;
                _foldButton.text = "+";
            }
            else
            {
                _shrinkLabel.style.display = DisplayStyle.None;
                _detailRoot.style.display = DisplayStyle.Flex;
                _foldButton.text = "-";
            }
        }

        private void OnNameFieldFocusOut(FocusOutEvent evt)
        {
            UpdateEnumName(EnumName.Trim(), false);
            UpdateTypeList(true);
        }

        private void OnClickAdd(ClickEvent evt)
        {
            NameValueElement element = new NameValueElement();
            _elementRoot.Add(element);
        }

        private void OnClickSort(ClickEvent evt)
        {
            SortElements();
        }

        private void UpdateEnumName(string newName, bool overridePrevious)
        {
            _nameField.SetValueWithoutNotify(newName);
            _lastName.UpdateName(newName, overridePrevious);
        }

        private void UpdateTypeList(bool revertDuplicated)
        {
            if (_lastName.CurrentInHierarchyName != _lastName.PreviousInHierarchyName)
            {
                string previousName = _lastName.PreviousInHierarchyName;
                _typeList.Remove(_lastName.PreviousInHierarchyName);
                if (_typeList.Contains(_lastName.CurrentInHierarchyName))
                {
                    if (revertDuplicated)
                    {
                        UpdateEnumName(_lastName.PreviousTypeName, true);
                        MessageDialog.DisplayDialog("Warning", $"Enum name {_lastName.CurrentInHierarchyName} is duplicate with an existing type, value has been reverted.", "OK");
                    }
                    else
                    {
                        _builder.Clear();
                        _builder.Append(EnumName);
                        _builder.Append(ProtoEditorUtility.GetWindowShownTime());
                        UpdateEnumName(_builder.ToString(), true);
                    }
                }
                string currentName = _lastName.CurrentInHierarchyName;

                if (!string.IsNullOrEmpty(currentName))
                {
                    _typeList.Add(currentName);
                }
                if (currentName != previousName && !string.IsNullOrEmpty(previousName))
                {
                    ManagerWindowSignalCenter.TypeChangedSignal.Dispatch(previousName, currentName);
                }
            }
        }

        public new class UxmlFactory : UxmlFactory<ProtoEnumElement, UxmlTraits>
        {
        }
    }
}