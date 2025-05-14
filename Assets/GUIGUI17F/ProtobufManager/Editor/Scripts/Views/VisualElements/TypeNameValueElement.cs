using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// VisualElement with a type DropdownField, a name TextField and a value TextField
    /// </summary>
    public class TypeNameValueElement : NameValueElement
    {
        public string DataType
        {
            get => _typeDropdown.value;
            set => _typeDropdown.value = value;
        }

        protected CompatibleDropdownField _typeDropdown;
        private List<string> _typeList;

        public TypeNameValueElement() : this(null)
        {
        }

        public TypeNameValueElement(List<string> typeChoices)
        {
            _typeList = typeChoices;

            _typeDropdown = new CompatibleDropdownField();
            _typeDropdown.style.minWidth = new StyleLength(60);
            _typeDropdown.style.maxWidth = new StyleLength(200);
            if (typeChoices != null && typeChoices.Count > 0)
            {
                _typeDropdown.SetupChoices(typeChoices);
                _typeDropdown.SetValueWithoutNotify(typeChoices[0]);
            }
            _container.Insert(0, _typeDropdown);
        }

        public void SetTypeWithoutNotify(string typeText)
        {
            _typeDropdown.SetValueWithoutNotify(typeText);
        }

        public override bool CheckValidation(string typeName, string fieldType, StringBuilder logBuilder)
        {
            bool pass = base.CheckValidation(typeName, fieldType, logBuilder);
            if (!pass)
            {
                return false;
            }
            if (string.IsNullOrEmpty(DataType))
            {
                logBuilder.AppendLine($"{typeName} - {fieldType} {DataName}: field type is empty!");
                return false;
            }
            if (_typeList != null && !_typeList.Contains(DataType))
            {
                logBuilder.AppendLine($"{typeName} - {fieldType} {DataName}: field has illegal type {DataType} which doesn't exist in the type list!");
                return false;
            }
            return true;
        }

        public virtual FieldDefinition GetDefinitionData()
        {
            bool hasValue = int.TryParse(DataValue, out int dataValue);
            FieldDefinition data = new FieldDefinition
            {
                IsRepeated = false,
                FieldType = DataType,
                FieldName = DataName,
                FieldNumber = hasValue ? dataValue : 0
            };
            return data;
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);
            if (_typeList != null)
            {
                if (evt.eventTypeId == AttachToPanelEvent.TypeId())
                {
                    if (!_typeList.Contains(DataType))
                    {
                        _typeDropdown.SetValueWithoutNotify(string.Empty);
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
            if (_typeDropdown.value == oldType)
            {
                _typeDropdown.SetValueWithoutNotify(newType);
            }
        }

        public new class UxmlFactory : UxmlFactory<TypeNameValueElement, UxmlTraits>
        {
        }
    }
}