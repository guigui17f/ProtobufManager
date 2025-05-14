using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    public class ProtoMessageElement : VisualElement
    {
        private static string _templatePath;

        public VisualElement VisualRoot { get; private set; }
        public string MessageName => _nameField.value;

        private Button _foldButton;
        private Label _shrinkLabel;
        private VisualElement _detailRoot;
        private VisualElement _enumRoot;
        private VisualElement _messageRoot;
        private VisualElement _fieldRoot;
        private TextField _nameField;
        private TextField _reservedField;
        private StringBuilder _builder;
        private List<string> _fieldNames;
        private List<int> _fieldNumbers;
        private List<string> _typeList;
        private List<ProtoMessageElement> _messageElementCache;
        private TypeNameString _lastName;

        public static (VisualElement visualRoot, ProtoMessageElement messageElement) CreateElement()
        {
            if (string.IsNullOrEmpty(_templatePath))
            {
                _templatePath = GlobalPathUtility.GetVisualTemplatePath("message-template.uxml");
            }
            VisualTreeAsset visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_templatePath);
            VisualElement visualRoot = visualAsset.Instantiate();
            ProtoMessageElement messageElement = visualRoot.Q<ProtoMessageElement>();
            messageElement.VisualRoot = visualRoot;
            return (visualRoot, messageElement);
        }

        public void Initialize(List<string> typeList, Func<string> parentNameGetter)
        {
            _typeList = typeList;
            _builder = new StringBuilder();
            _fieldNames = new List<string>();
            _fieldNumbers = new List<int>();
            _messageElementCache = new List<ProtoMessageElement>();
            _lastName = new TypeNameString(parentNameGetter, _builder);
            _foldButton = this.Q<Button>("fold-button");
            VisualElement foldRoot = this.Q<VisualElement>("fold-root");
            _shrinkLabel = foldRoot.Q<Label>("shrink-label");
            _detailRoot = foldRoot.Q<VisualElement>("detail-root");
            _enumRoot = _detailRoot.Q<VisualElement>("enum-root");
            _messageRoot = _detailRoot.Q<VisualElement>("message-root");
            _fieldRoot = _detailRoot.Q<VisualElement>("field-root");
            _nameField = _detailRoot.Q<TextField>("name-field");
            _reservedField = _detailRoot.Q<TextField>("reserved-field");
            _foldButton.RegisterCallback<ClickEvent>(OnClickFold);
            _nameField.RegisterCallback<FocusOutEvent>(OnNameFieldFocusOut);
            this.Q<Button>("add-field-button").RegisterCallback<ClickEvent>(OnAddField);
            this.Q<Button>("add-map-button").RegisterCallback<ClickEvent>(OnAddMap);
            this.Q<Button>("add-oneof-button").RegisterCallback<ClickEvent>(OnAddOneof);
            this.Q<Button>("add-enum-button").RegisterCallback<ClickEvent>(OnAddEnum);
            this.Q<Button>("add-message-button").RegisterCallback<ClickEvent>(OnAddMessage);
            this.Q<Button>("sort-button").RegisterCallback<ClickEvent>(OnClickSort);
            _shrinkLabel.style.display = DisplayStyle.None;
        }

        public void Initialize(List<string> typeList, Func<string> parentNameGetter, MessageDefinition definition)
        {
            Initialize(typeList, parentNameGetter);
            UpdateMessageName(definition.MessageName, true);
            foreach (EnumDefinition enumDefinition in definition.EnumDefinitions)
            {
                var element = ProtoEnumElement.CreateElement();
                _enumRoot.Add(element.visualRoot);
                element.enumElement.Initialize(typeList, GetInHierarchyMessageName, enumDefinition);
            }
            foreach (MessageDefinition message in definition.MessageDefinitions)
            {
                var element = ProtoMessageElement.CreateElement();
                _messageRoot.Add(element.visualRoot);
                element.messageElement.Initialize(typeList, GetInHierarchyMessageName, message);
            }
            foreach (FieldDefinition field in definition.NormalFields)
            {
                ProtoFieldElement element = new ProtoFieldElement(typeList);
                element.SetRuleWithoutNotify(field.IsRepeated);
                element.SetTypeWithoutNotify(field.FieldType);
                element.SetNameWithoutNotify(field.FieldName);
                element.SetValueWithoutNotify(field.FieldNumber.ToString());
                _fieldRoot.Add(element);
            }
            foreach (MapDefinition map in definition.MapFields)
            {
                ProtoMapElement element = new ProtoMapElement(typeList);
                element.SetKeyTypeWithoutNotify(map.KeyType);
                element.SetValueTypeWithoutNotify(map.ValueType);
                element.SetNameWithoutNotify(map.MapName);
                element.SetValueWithoutNotify(map.FieldNumber.ToString());
                _fieldRoot.Add(element);
            }
            foreach (OneofDefinition oneof in definition.OneofFields)
            {
                var element = ProtoOneofElement.CreateElement();
                element.oneofElement.Initialize(typeList, oneof);
                _fieldRoot.Add(element.visualRoot);
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
            SortElements();
        }

        public bool CheckValidation(string typeName, StringBuilder logBuilder)
        {
            if (string.IsNullOrEmpty(MessageName))
            {
                logBuilder.AppendLine($"{typeName}: message name is empty!");
                return false;
            }

            _builder.Clear();
            if (!string.IsNullOrEmpty(typeName))
            {
                _builder.Append(typeName);
                _builder.Append('.');
            }
            _builder.Append(MessageName);
            string selfTypeName = _builder.ToString();
            if (ProtoEditorUtility.BuiltInTypes.Contains(MessageName))
            {
                logBuilder.AppendLine($"{selfTypeName}: message name is illegal!");
                return false;
            }
            List<ProtoEnumElement> enums = _enumRoot.Query<ProtoEnumElement>().ToList();
            for (int i = 0; i < enums.Count; i++)
            {
                if (!enums[i].CheckValidation(selfTypeName, logBuilder))
                {
                    return false;
                }
            }
            _messageRoot.QueryChildrenElement(_messageElementCache);
            for (int i = 0; i < _messageElementCache.Count; i++)
            {
                if (!_messageElementCache[i].CheckValidation(selfTypeName, logBuilder))
                {
                    return false;
                }
            }

            _fieldNames.Clear();
            _fieldNumbers.Clear();
            List<ProtoFieldElement> fields = _fieldRoot.Query<ProtoFieldElement>().ToList();
            for (int i = 0; i < fields.Count; i++)
            {
                if (!fields[i].CheckValidation(selfTypeName, logBuilder))
                {
                    return false;
                }
                if (_fieldNames.Contains(fields[i].DataName))
                {
                    logBuilder.AppendLine($"{selfTypeName} - message field {fields[i].DataName}: message contains duplicated field name!");
                    return false;
                }
                _fieldNames.Add(fields[i].DataName);

                int number = int.Parse(fields[i].DataValue);
                if (_fieldNumbers.Contains(number))
                {
                    logBuilder.AppendLine($"{selfTypeName} - message field {fields[i].DataName}: message contains duplicated field number {fields[i].DataValue}!");
                    return false;
                }
                _fieldNumbers.Add(number);
            }
            List<ProtoMapElement> maps = _fieldRoot.Query<ProtoMapElement>().ToList();
            for (int i = 0; i < maps.Count; i++)
            {
                if (!maps[i].CheckValidation(selfTypeName, logBuilder))
                {
                    return false;
                }
                if (_fieldNames.Contains(maps[i].DataName))
                {
                    logBuilder.AppendLine($"{selfTypeName} - map field {maps[i].DataName}: message contains duplicated field name!");
                    return false;
                }
                _fieldNames.Add(maps[i].DataName);

                int number = int.Parse(maps[i].DataValue);
                if (_fieldNumbers.Contains(number))
                {
                    logBuilder.AppendLine($"{selfTypeName} - map field {maps[i].DataName}: message contains duplicated field number {maps[i].DataValue}!");
                    return false;
                }
                _fieldNumbers.Add(number);
            }
            List<ProtoOneofElement> oneofs = _fieldRoot.Query<ProtoOneofElement>().ToList();
            for (int i = 0; i < oneofs.Count; i++)
            {
                if (!oneofs[i].CheckValidation(selfTypeName, logBuilder, out List<string> oneofFieldNames, out List<int> oneofFieldNumbers))
                {
                    return false;
                }
                if (_fieldNames.Contains(oneofs[i].OneofName))
                {
                    logBuilder.AppendLine($"{selfTypeName} - oneof {oneofs[i].OneofName}: oneof name duplicated with message field name!");
                    return false;
                }
                _fieldNames.Add(oneofs[i].OneofName);

                foreach (string fieldName in oneofFieldNames)
                {
                    if (_fieldNames.Contains(fieldName))
                    {
                        logBuilder.AppendLine($"{selfTypeName} - oneof {oneofs[i].OneofName}: message contains duplicated field name {fieldName} in oneof data!");
                        return false;
                    }
                }
                _fieldNames.AddRange(oneofFieldNames);

                foreach (int fieldNumber in oneofFieldNumbers)
                {
                    if (_fieldNumbers.Contains(fieldNumber))
                    {
                        logBuilder.AppendLine($"{selfTypeName} - oneof {oneofs[i].OneofName}: message contains duplicated field number {fieldNumber.ToString()} in oneof data!");
                        return false;
                    }
                }
                _fieldNumbers.AddRange(oneofFieldNumbers);
            }
            return true;
        }

        public MessageDefinition GetDefinitionData()
        {
            SortElements();
            MessageDefinition data = new MessageDefinition { MessageName = MessageName };
            _enumRoot.Query<ProtoEnumElement>().ForEach(item => data.EnumDefinitions.Add(item.GetDefinitionData()));
            _messageRoot.QueryChildrenElement(_messageElementCache).ForEach(item => data.MessageDefinitions.Add(item.GetDefinitionData()));
            _fieldRoot.Query<ProtoFieldElement>().ForEach(item => data.NormalFields.Add(item.GetDefinitionData()));
            _fieldRoot.Query<ProtoMapElement>().ForEach(item => data.MapFields.Add(item.GetDefinitionData()));
            _fieldRoot.Query<ProtoOneofElement>().ForEach(item => data.OneofFields.Add(item.GetDefinitionData()));
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
            _fieldRoot.Sort((a, b) =>
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
                int x = int.MaxValue;
                if (a is ProtoFieldElement fieldA)
                {
                    if (!string.IsNullOrEmpty(fieldA.DataValue))
                    {
                        x = int.Parse(fieldA.DataValue);
                    }
                }
                else if (a is ProtoMapElement mapA)
                {
                    if (!string.IsNullOrEmpty(mapA.DataValue))
                    {
                        x = int.Parse(mapA.DataValue);
                    }
                }
                else
                {
                    ProtoOneofElement oneofA = a.Q<ProtoOneofElement>();
                    oneofA.SortElements();
                    TypeNameValueElement oneofAField = oneofA.Q<TypeNameValueElement>();
                    if (oneofAField != null && !string.IsNullOrEmpty(oneofAField.DataValue))
                    {
                        x = int.Parse(oneofAField.DataValue);
                    }
                }
                int y = int.MaxValue;
                if (b is ProtoFieldElement fieldB)
                {
                    if (!string.IsNullOrEmpty(fieldB.DataValue))
                    {
                        y = int.Parse(fieldB.DataValue);
                    }
                }
                else if (b is ProtoMapElement mapB)
                {
                    if (!string.IsNullOrEmpty(mapB.DataValue))
                    {
                        y = int.Parse(mapB.DataValue);
                    }
                }
                else
                {
                    ProtoOneofElement oneofB = b.Q<ProtoOneofElement>();
                    oneofB.SortElements();
                    TypeNameValueElement oneofBField = oneofB.Q<TypeNameValueElement>();
                    if (oneofBField != null && !string.IsNullOrEmpty(oneofBField.DataValue))
                    {
                        y = int.Parse(oneofBField.DataValue);
                    }
                }
                return x - y;
            });
        }

        public void HandleParentNameChanged()
        {
            _lastName.UpdateName(MessageName, false);
            UpdateTypeList(false, false);
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);
            if (_typeList != null)
            {
                if (evt.eventTypeId == AttachToPanelEvent.TypeId())
                {
                    _lastName.UpdateName(MessageName, true);
                    if (!string.IsNullOrEmpty(_lastName.CurrentInHierarchyName))
                    {
                        if (_typeList.Contains(_lastName.CurrentInHierarchyName))
                        {
                            _builder.Clear();
                            _builder.Append(MessageName);
                            _builder.Append(ProtoEditorUtility.GetWindowShownTime());
                            UpdateMessageName(_builder.ToString(), true);
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
                _builder.Append("message ");
                _builder.Append(MessageName);
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
            UpdateMessageName(MessageName.Trim(), false);
            UpdateTypeList(true, true);
        }

        private void OnAddField(ClickEvent evt)
        {
            ProtoFieldElement element = new ProtoFieldElement(_typeList);
            _fieldRoot.Add(element);
        }

        private void OnAddMap(ClickEvent evt)
        {
            ProtoMapElement element = new ProtoMapElement(_typeList);
            _fieldRoot.Add(element);
        }

        private void OnAddOneof(ClickEvent evt)
        {
            var element = ProtoOneofElement.CreateElement();
            element.oneofElement.Initialize(_typeList);
            _fieldRoot.Add(element.visualRoot);
        }

        private void OnAddEnum(ClickEvent evt)
        {
            var element = ProtoEnumElement.CreateElement();
            element.enumElement.Initialize(_typeList, GetInHierarchyMessageName);
            _enumRoot.Add(element.visualRoot);
        }

        private void OnAddMessage(ClickEvent evt)
        {
            var element = CreateElement();
            element.messageElement.Initialize(_typeList, GetInHierarchyMessageName);
            _messageRoot.Add(element.visualRoot);
        }

        private void OnClickSort(ClickEvent evt)
        {
            SortElements();
        }

        private void UpdateMessageName(string newName, bool overridePrevious)
        {
            _nameField.SetValueWithoutNotify(newName);
            _lastName.UpdateName(newName, overridePrevious);
        }

        private void UpdateTypeList(bool revertDuplicated, bool updateChildren)
        {
            if (_lastName.CurrentInHierarchyName != _lastName.PreviousInHierarchyName)
            {
                string previousName = _lastName.PreviousInHierarchyName;
                _typeList.Remove(_lastName.PreviousInHierarchyName);
                if (_typeList.Contains(_lastName.CurrentInHierarchyName))
                {
                    if (revertDuplicated)
                    {
                        UpdateMessageName(_lastName.PreviousTypeName, true);
                        MessageDialog.DisplayDialog("Warning", $"Message name {_lastName.CurrentInHierarchyName} is duplicate with an existing type, value has been reverted.", "OK");
                    }
                    else
                    {
                        _builder.Clear();
                        _builder.Append(MessageName);
                        _builder.Append(ProtoEditorUtility.GetWindowShownTime());
                        UpdateMessageName(_builder.ToString(), true);
                    }
                }
                string currentName = _lastName.CurrentInHierarchyName;

                if (!string.IsNullOrEmpty(currentName))
                {
                    _typeList.Add(currentName);
                }
                if (currentName != previousName)
                {
                    if (!string.IsNullOrEmpty(previousName))
                    {
                        ManagerWindowSignalCenter.TypeChangedSignal.Dispatch(previousName, currentName);
                    }
                    if (updateChildren)
                    {
                        this.Query<ProtoEnumElement>().ForEach(item => item.HandleParentNameChanged());
                        this.Query<ProtoMessageElement>().ForEach(item => item.HandleParentNameChanged());
                    }
                }
            }
        }

        private string GetInHierarchyMessageName()
        {
            return _lastName.GetInHierarchyName(MessageName);
        }

        public new class UxmlFactory : UxmlFactory<ProtoMessageElement, UxmlTraits>
        {
        }
    }
}