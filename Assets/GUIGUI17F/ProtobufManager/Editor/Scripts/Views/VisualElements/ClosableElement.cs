using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// VisualElement with a close button
    /// </summary>
    public class ClosableElement : VisualElement
    {
        private const string DefaultWarningText = "This action cannot be undo, continue?";
        
        protected VisualElement _container;
        protected VisualElement _lastParent;
        
        public override VisualElement contentContainer => _container;
        
        public float FlexGrow
        {
            get => _container.style.flexGrow.value;
            set => _container.style.flexGrow = new StyleFloat(value);
        }
        
        public Align AlignItems
        {
            get => style.alignItems.value;
            set => style.alignItems = new StyleEnum<Align>(value);
        }
        
        public bool WarningBeforeClose { get; set; }
        public string CloseWarningText { get; set; }

        public ClosableElement() : this(false)
        {
        }

        public ClosableElement(bool warningBeforeClose, string closeWarningText = null)
        {
            WarningBeforeClose = warningBeforeClose;
            CloseWarningText = closeWarningText;

            StyleEnum<FlexDirection> rowDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            StyleLength padding = new StyleLength(3);
            StyleLength margin = new StyleLength(1);
            style.flexDirection = rowDirection;
            style.paddingLeft = padding;
            style.paddingRight = padding;
            style.paddingTop = padding;
            style.paddingBottom = padding;
            style.marginLeft = margin;
            style.marginRight = margin;
            style.marginTop = margin;
            style.marginBottom = margin;
            _container = new VisualElement();
            _container.style.flexDirection = rowDirection;
            hierarchy.Add(_container);
            
            Button button = new Button { text = "X" };
            button.RegisterCallback<ClickEvent>(OnClickCloseButton);
            hierarchy.Add(button);
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);
            if (evt.eventTypeId == AttachToPanelEvent.TypeId())
            {
                _lastParent = parent;
            }
        }

        private void OnClickCloseButton(ClickEvent evt)
        {
            if (WarningBeforeClose)
            {
                if (MessageDialog.DisplayDialog("Warning", string.IsNullOrEmpty(CloseWarningText) ? DefaultWarningText : CloseWarningText, "OK", "Cancel"))
                {
                    RemoveFromHierarchy();
                    CustomUndoRedoManager.Instance.RecordForUndo(HandleUndoElementDeletion);
                }
            }
            else
            {
                RemoveFromHierarchy();
                CustomUndoRedoManager.Instance.RecordForUndo(HandleUndoElementDeletion);
            }
        }

        private void HandleUndoElementDeletion()
        {
            if (_lastParent != null)
            {
                _lastParent.Add(this);
            }
        }

        public new class UxmlFactory : UxmlFactory<ClosableElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlBoolAttributeDescription _warningBeforeClose = new UxmlBoolAttributeDescription { name = "warning-before-close" };
            private UxmlStringAttributeDescription _closeWarningText = new UxmlStringAttributeDescription { name = "close-warning-text" };
            private UxmlFloatAttributeDescription _flexGrow = new UxmlFloatAttributeDescription { name = "flex-grow" };
            private UxmlEnumAttributeDescription<Align> _alignItems = new UxmlEnumAttributeDescription<Align> { name = "align-items" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is ClosableElement element)
                {
                    element.WarningBeforeClose = _warningBeforeClose.GetValueFromBag(bag, cc);
                    element.CloseWarningText = _closeWarningText.GetValueFromBag(bag, cc);
                    element.FlexGrow = _flexGrow.GetValueFromBag(bag, cc);
                    element.AlignItems = _alignItems.GetValueFromBag(bag, cc);
                }
            }
        }
    }
}