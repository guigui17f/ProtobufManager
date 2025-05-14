using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    public class ProtoMapElement : NameValueElement
    {
        public string KeyType
        {
            get => _keyDropdown.value;
            set => _keyDropdown.value = value;
        }

        public string ValueType
        {
            get => _valueDropdown.value;
            set => _valueDropdown.value = value;
        }

        private CompatibleDropdownField _keyDropdown;
        private CompatibleDropdownField _valueDropdown;
        private List<string> _valueTypeList;

        public ProtoMapElement() : this(null)
        {
        }

        public ProtoMapElement(List<string> valueChoices)
        {
            _valueTypeList = valueChoices;

            Label label = new Label("map <");
            _container.Insert(0, label);

            _keyDropdown = new CompatibleDropdownField(ProtoEditorUtility.MapKeyTypes, 0);
            _keyDropdown.style.width = new StyleLength(60);
            _container.Insert(1, _keyDropdown);

            label = new Label(",");
            _container.Insert(2, label);

            _valueDropdown = new CompatibleDropdownField();
            _valueDropdown.style.minWidth = new StyleLength(60);
            _valueDropdown.style.maxWidth = new StyleLength(200);
            if (valueChoices != null && valueChoices.Count > 0)
            {
                _valueDropdown.SetupChoices(valueChoices);
                _valueDropdown.SetValueWithoutNotify(valueChoices[0]);
            }
            _container.Insert(3, _valueDropdown);

            label = new Label(">");
            label.style.marginRight = new StyleLength(5);
            _container.Insert(4, label);
        }

        public void SetKeyTypeWithoutNotify(string keyType)
        {
            _keyDropdown.SetValueWithoutNotify(keyType);
        }

        public void SetValueTypeWithoutNotify(string valueType)
        {
            _valueDropdown.SetValueWithoutNotify(valueType);
        }

        public bool CheckValidation(string typeName, StringBuilder logBuilder)
        {
            bool pass = CheckValidation(typeName, "map field", logBuilder);
            if (!pass)
            {
                return false;
            }
            if (string.IsNullOrEmpty(KeyType))
            {
                logBuilder.AppendLine($"{typeName} - map field {DataName}: key type is empty!");
                return false;
            }
            if (string.IsNullOrEmpty(ValueType))
            {
                logBuilder.AppendLine($"{typeName} - map field {DataName}: value type is empty!");
                return false;
            }
            if (_valueTypeList != null && !_valueTypeList.Contains(ValueType))
            {
                logBuilder.AppendLine($"{typeName} - map field {DataName}: field has illegal value type {ValueType} which doesn't exist in the type list!");
                return false;
            }
            if (!ProtoEditorUtility.IsValidFieldNumber(typeName, "map field", DataName, int.Parse(DataValue), logBuilder))
            {
                return false;
            }
            return true;
        }

        public MapDefinition GetDefinitionData()
        {
            bool hasValue = int.TryParse(DataValue, out int dataValue);
            MapDefinition data = new MapDefinition
            {
                KeyType = KeyType,
                ValueType = ValueType,
                MapName = DataName,
                FieldNumber = hasValue ? dataValue : 0
            };
            return data;
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);
            if (_valueTypeList != null)
            {
                if (evt.eventTypeId == AttachToPanelEvent.TypeId())
                {
                    if (!_valueTypeList.Contains(ValueType))
                    {
                        _valueDropdown.SetValueWithoutNotify(string.Empty);
                    }
                    ManagerWindowSignalCenter.TypeChangedSignal.AddListener(OnTypeChanged);
                }
                else if (evt.eventTypeId == DetachFromPanelEvent.TypeId())
                {
                    ManagerWindowSignalCenter.TypeChangedSignal.RemoveListener(OnTypeChanged);
                }
            }
        }

        private void OnTypeChanged(string oldType, string newType)
        {
            if (_valueDropdown.value == oldType)
            {
                _valueDropdown.SetValueWithoutNotify(newType);
            }
        }

        public new class UxmlFactory : UxmlFactory<ProtoMapElement, UxmlTraits>
        {
        }
    }
}