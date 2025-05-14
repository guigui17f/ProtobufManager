using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    public class LabelTagElement : ClosableElement
    {
        public string LabelText
        {
            get => _textLabel.text;
            set => _textLabel.text = value;
        }

        private Label _textLabel;

        public LabelTagElement() : this(string.Empty)
        {
        }

        public LabelTagElement(string mainText, bool warningBeforeClose = false, string closeWarningText = null) : base(warningBeforeClose, closeWarningText)
        {
            style.alignItems = new StyleEnum<Align>(Align.Center);
            _textLabel = new Label(mainText);
            _container.Add(_textLabel);
        }

        public new class UxmlFactory : UxmlFactory<LabelTagElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription _labelText = new UxmlStringAttributeDescription { name = "label-text" };
            private UxmlBoolAttributeDescription _warningBeforeClose = new UxmlBoolAttributeDescription { name = "warning-before-close" };
            private UxmlStringAttributeDescription _closeWarningText = new UxmlStringAttributeDescription { name = "close-warning-text" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is LabelTagElement element)
                {
                    element._textLabel.text = _labelText.GetValueFromBag(bag, cc);
                    element.WarningBeforeClose = _warningBeforeClose.GetValueFromBag(bag, cc);
                    element.CloseWarningText = _closeWarningText.GetValueFromBag(bag, cc);
                }
            }
        }
    }
}