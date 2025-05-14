using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    public class ProtoOneofElement : VisualElement
    {
        private static string _templatePath;

        public VisualElement VisualRoot { get; private set; }
        public string OneofName => _nameField.value;

        private Button _foldButton;
        private Label _shrinkLabel;
        private VisualElement _detailRoot;
        private VisualElement _elementRoot;
        private TextField _nameField;
        private StringBuilder _builder;
        private List<string> _typeChoices;
        private List<string> _fieldNames;
        private List<int> _fieldNumbers;

        public static (VisualElement visualRoot, ProtoOneofElement oneofElement) CreateElement()
        {
            if (string.IsNullOrEmpty(_templatePath))
            {
                _templatePath = GlobalPathUtility.GetVisualTemplatePath("oneof-template.uxml");
            }
            VisualTreeAsset visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_templatePath);
            VisualElement visualRoot = visualAsset.Instantiate();
            ProtoOneofElement oneofElement = visualRoot.Q<ProtoOneofElement>();
            oneofElement.VisualRoot = visualRoot;
            return (visualRoot, oneofElement);
        }

        public void Initialize(List<string> typeChoices)
        {
            _typeChoices = typeChoices;
            _builder = new StringBuilder();
            _fieldNames = new List<string>();
            _fieldNumbers = new List<int>();
            _foldButton = this.Q<Button>("fold-button");
            VisualElement foldRoot = this.Q<VisualElement>("fold-root");
            _shrinkLabel = foldRoot.Q<Label>("shrink-label");
            _detailRoot = foldRoot.Q<VisualElement>("detail-root");
            _elementRoot = _detailRoot.Q<VisualElement>("element-root");
            _nameField = _detailRoot.Q<TextField>("name-field");
            _foldButton.RegisterCallback<ClickEvent>(OnClickFold);
            _nameField.RegisterCallback<FocusOutEvent>(OnNameFieldFocusOut);
            _detailRoot.Q<Button>("add-button").RegisterCallback<ClickEvent>(OnClickAdd);
            _detailRoot.Q<Button>("sort-button").RegisterCallback<ClickEvent>(OnClickSort);
            _shrinkLabel.style.display = DisplayStyle.None;
        }

        public void Initialize(List<string> typeChoices, OneofDefinition definition)
        {
            Initialize(typeChoices);
            _nameField.SetValueWithoutNotify(definition.OneofName);
            foreach (FieldDefinition field in definition.OneofFields)
            {
                TypeNameValueElement element = new TypeNameValueElement(typeChoices);
                element.SetTypeWithoutNotify(field.FieldType);
                element.SetNameWithoutNotify(field.FieldName);
                element.SetValueWithoutNotify(field.FieldNumber.ToString());
                _elementRoot.Add(element);
            }
        }

        public bool CheckValidation(string typeName, StringBuilder logBuilder, out List<string> fieldNames, out List<int> fieldNumbers)
        {
            _fieldNames.Clear();
            _fieldNumbers.Clear();
            fieldNames = _fieldNames;
            fieldNumbers = _fieldNumbers;
            if (string.IsNullOrEmpty(OneofName))
            {
                logBuilder.AppendLine($"{typeName}: oneof name is empty!");
                return false;
            }

            _builder.Clear();
            _builder.Append(typeName);
            _builder.Append(' ');
            _builder.Append("oneof");
            _builder.Append(' ');
            _builder.Append(OneofName);
            string selfTypeName = _builder.ToString();
            if (ProtoEditorUtility.BuiltInTypes.Contains(OneofName))
            {
                logBuilder.AppendLine($"{selfTypeName}: oneof name is illegal!");
                return false;
            }
            List<TypeNameValueElement> fields = _elementRoot.Query<TypeNameValueElement>().ToList();
            if (fields.Count <= 0)
            {
                logBuilder.AppendLine($"{selfTypeName}: data contains no field!");
                return false;
            }

            for (int i = 0; i < fields.Count; i++)
            {
                if (!fields[i].CheckValidation(selfTypeName, "field", logBuilder))
                {
                    return false;
                }
                if (_fieldNames.Contains(fields[i].DataName))
                {
                    logBuilder.Append($"{selfTypeName} - field {fields[i].DataName}: data contains duplicated field name!");
                    return false;
                }
                _fieldNames.Add(fields[i].DataName);

                int number = int.Parse(fields[i].DataValue);
                if (_fieldNumbers.Contains(number))
                {
                    logBuilder.Append($"{selfTypeName} - field {fields[i].DataName}: data contains duplicated field number {fields[i].DataValue}!");
                    return false;
                }
                if (!ProtoEditorUtility.IsValidFieldNumber(selfTypeName, "field", fields[i].DataName, number, logBuilder))
                {
                    return false;
                }
                _fieldNumbers.Add(number);
            }
            return true;
        }

        public OneofDefinition GetDefinitionData()
        {
            SortElements();
            OneofDefinition data = new OneofDefinition { OneofName = OneofName };
            _elementRoot.Query<TypeNameValueElement>().ForEach(item => data.OneofFields.Add(item.GetDefinitionData()));
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
                TypeNameValueElement x = a as TypeNameValueElement;
                TypeNameValueElement y = b as TypeNameValueElement;
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

        private void OnClickFold(ClickEvent evt)
        {
            if (_foldButton.text == "-")
            {
                _builder.Clear();
                _builder.Append("oneof ");
                _builder.Append(OneofName);
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
            _nameField.SetValueWithoutNotify(OneofName.Trim());
        }

        private void OnClickAdd(ClickEvent evt)
        {
            TypeNameValueElement element = new TypeNameValueElement(_typeChoices);
            _elementRoot.Add(element);
        }

        private void OnClickSort(ClickEvent evt)
        {
            SortElements();
        }

        public new class UxmlFactory : UxmlFactory<ProtoOneofElement, UxmlTraits>
        {
        }
    }
}