using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    public class ProtoFieldElement : TypeNameValueElement
    {
        private static readonly List<string> FieldRules = new List<string> { "singular", "repeated" };

        public bool IsRepeated
        {
            get => _ruleDropdown.value == "repeated";
            set => _ruleDropdown.value = value ? "repeated" : "singular";
        }

        private CompatibleDropdownField _ruleDropdown;

        public ProtoFieldElement() : this(null)
        {
        }

        public ProtoFieldElement(List<string> typeChoices) : base(typeChoices)
        {
            _ruleDropdown = new CompatibleDropdownField(FieldRules, 0);
            _ruleDropdown.style.width = new StyleLength(75);
            _container.Insert(0, _ruleDropdown);
        }

        public void SetRuleWithoutNotify(bool isRepeated)
        {
            _ruleDropdown.SetValueWithoutNotify(isRepeated ? "repeated" : "singular");
        }

        public bool CheckValidation(string typeName, StringBuilder logBuilder)
        {
            bool pass = base.CheckValidation(typeName, "message field", logBuilder);
            if (!pass)
            {
                return false;
            }
            if (!ProtoEditorUtility.IsValidFieldNumber(typeName, "message field", DataName, int.Parse(DataValue), logBuilder))
            {
                return false;
            }
            return true;
        }

        public override FieldDefinition GetDefinitionData()
        {
            FieldDefinition data = base.GetDefinitionData();
            data.IsRepeated = IsRepeated;
            return data;
        }

        public new class UxmlFactory : UxmlFactory<ProtoFieldElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlBoolAttributeDescription _isRepeated = new UxmlBoolAttributeDescription { name = "is-repeated" };
            private UxmlStringAttributeDescription _dataName = new UxmlStringAttributeDescription { name = "data-name" };
            private UxmlStringAttributeDescription _dataValue = new UxmlStringAttributeDescription { name = "data-value" };
            private UxmlBoolAttributeDescription _warningBeforeClose = new UxmlBoolAttributeDescription { name = "warning-before-close" };
            private UxmlStringAttributeDescription _closeWarningText = new UxmlStringAttributeDescription { name = "close-warning-text" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is ProtoFieldElement element)
                {
                    bool isRepeated = _isRepeated.GetValueFromBag(bag, cc);
                    element._ruleDropdown.SetValueWithoutNotify(isRepeated ? "repeated" : "singular");
                    element._nameField.SetValueWithoutNotify(_dataName.GetValueFromBag(bag, cc));
                    element._valueField.SetValueWithoutNotify(_dataValue.GetValueFromBag(bag, cc));
                    element.WarningBeforeClose = _warningBeforeClose.GetValueFromBag(bag, cc);
                    element.CloseWarningText = _closeWarningText.GetValueFromBag(bag, cc);
                }
            }
        }
    }
}