using System.Text;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// VisualElement with a name TextField and a value TextField
    /// </summary>
    public class NameValueElement : ClosableElement
    {
        public string DataName
        {
            get => _nameField.value;
            set => _nameField.value = value;
        }

        public string DataValue
        {
            get => _valueField.value;
            set => _valueField.value = value;
        }

        protected TextField _nameField;
        protected TextField _valueField;

        public NameValueElement()
        {
            _nameField = new TextField();
            _nameField.style.minWidth = new StyleLength(60);
            _nameField.RegisterCallback<FocusOutEvent>(OnNameFieldFocusOut);
            _container.Add(_nameField);

            Label label = new Label("=");
            _container.Add(label);

            _valueField = new TextField();
            _valueField.style.minWidth = new StyleLength(60);
            _valueField.RegisterCallback<ChangeEvent<string>>(OnValueFieldChange);
            _container.Add(_valueField);

            label = new Label(";");
            label.style.marginRight = new StyleLength(5);
            _container.Add(label);
        }

        public void SetNameWithoutNotify(string dataName)
        {
            _nameField.SetValueWithoutNotify(dataName);
        }

        public void SetValueWithoutNotify(string dataValue)
        {
            _valueField.SetValueWithoutNotify(dataValue);
        }

        public virtual bool CheckValidation(string typeName, string fieldType, StringBuilder logBuilder)
        {
            if (string.IsNullOrEmpty(DataName))
            {
                logBuilder.AppendLine($"{typeName}: {fieldType} name is empty!");
                return false;
            }
            if (string.IsNullOrEmpty(DataValue))
            {
                logBuilder.AppendLine($"{typeName} - {fieldType} {DataName}: field value is empty!");
                return false;
            }
            return true;
        }

        private void OnNameFieldFocusOut(FocusOutEvent evt)
        {
            _nameField.SetValueWithoutNotify(DataName.Trim());
        }

        private void OnValueFieldChange(ChangeEvent<string> evt)
        {
            if (!ProtoEditorUtility.PassNumberValidation(evt.newValue))
            {
                _valueField.SetValueWithoutNotify(evt.previousValue);
            }
        }

        public new class UxmlFactory : UxmlFactory<NameValueElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription _dataName = new UxmlStringAttributeDescription { name = "data-name" };
            private UxmlStringAttributeDescription _dataValue = new UxmlStringAttributeDescription { name = "data-value" };
            private UxmlBoolAttributeDescription _warningBeforeClose = new UxmlBoolAttributeDescription { name = "warning-before-close" };
            private UxmlStringAttributeDescription _closeWarningText = new UxmlStringAttributeDescription { name = "close-warning-text" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is NameValueElement element)
                {
                    element._nameField.SetValueWithoutNotify(_dataName.GetValueFromBag(bag, cc));
                    element._valueField.SetValueWithoutNotify(_dataValue.GetValueFromBag(bag, cc));
                    element.WarningBeforeClose = _warningBeforeClose.GetValueFromBag(bag, cc);
                    element.CloseWarningText = _closeWarningText.GetValueFromBag(bag, cc);
                }
            }
        }
    }
}