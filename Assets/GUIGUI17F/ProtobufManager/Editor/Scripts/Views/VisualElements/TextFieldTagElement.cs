using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    public class TextFieldTagElement : ClosableElement
    {
        public string InputText
        {
            get => _inputField.value;
            set => _inputField.value = value;
        }

        public bool IsReadOnly
        {
            get => _inputField.isReadOnly;
            set => _inputField.isReadOnly = value;
        }

        private TextField _inputField;

        public TextFieldTagElement() : this(false)
        {
        }

        public TextFieldTagElement(bool warningBeforeClose, string closeWarningText = null) : base(warningBeforeClose, closeWarningText)
        {
            _inputField = new TextField();
            _inputField.style.minWidth = new StyleLength(60);
            _inputField.RegisterCallback<FocusOutEvent>(OnInputFieldFocusOut);
            _container.Add(_inputField);
        }

        public void SetValueWithoutNotify(string valueText)
        {
            _inputField.SetValueWithoutNotify(valueText);
        }

        private void OnInputFieldFocusOut(FocusOutEvent evt)
        {
            _inputField.SetValueWithoutNotify(InputText.Trim());
        }

        public new class UxmlFactory : UxmlFactory<TextFieldTagElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription _inputText = new UxmlStringAttributeDescription { name = "input-text" };
            private UxmlBoolAttributeDescription _warningBeforeClose = new UxmlBoolAttributeDescription { name = "warning-before-close" };
            private UxmlStringAttributeDescription _closeWarningText = new UxmlStringAttributeDescription { name = "close-warning-text" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is TextFieldTagElement element)
                {
                    element._inputField.SetValueWithoutNotify(_inputText.GetValueFromBag(bag, cc));
                    element.WarningBeforeClose = _warningBeforeClose.GetValueFromBag(bag, cc);
                    element.CloseWarningText = _closeWarningText.GetValueFromBag(bag, cc);
                }
            }
        }
    }
}