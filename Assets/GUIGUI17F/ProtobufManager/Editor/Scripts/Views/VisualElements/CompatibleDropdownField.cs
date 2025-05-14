using System;
using System.Collections.Generic;
#if !UNITY_2021_2_OR_NEWER
using System.Reflection;
#endif
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// use this to provide DropdownField element support in old UI Toolkit version
    /// </summary>
    public class CompatibleDropdownField : PopupField<string>
    {
#if !UNITY_2021_2_OR_NEWER
        private static readonly PropertyInfo DropdownChoicesInfo = typeof(BasePopupField<string, string>).GetProperty("choices", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif

        private static void SetupDropdownChoices(BasePopupField<string, string> dropdown, List<string> choiceList)
        {
#if !UNITY_2021_2_OR_NEWER
            //use reflection to bypass the access limitation in old UI Toolkit version
            DropdownChoicesInfo.SetValue(dropdown, choiceList);
#else
            dropdown.choices = choiceList;
#endif
        }

        public CompatibleDropdownField() : this(null)
        {
        }

        public CompatibleDropdownField(string label) : base(label)
        {
        }

        public CompatibleDropdownField(List<string> choiceList, string defaultValue, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
            : this(null, choiceList, defaultValue, formatSelectedValueCallback, formatListItemCallback)
        {
        }

        public CompatibleDropdownField(string label, List<string> choiceList, string defaultValue, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
            : base(label, choiceList, defaultValue, formatSelectedValueCallback, formatListItemCallback)
        {
        }

        public CompatibleDropdownField(List<string> choiceList, int defaultIndex, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
            : this(null, choiceList, defaultIndex, formatSelectedValueCallback, formatListItemCallback)
        {
        }

        public CompatibleDropdownField(string label, List<string> choiceList, int defaultIndex, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
            : base(label, choiceList, defaultIndex, formatSelectedValueCallback, formatListItemCallback)
        {
        }

        public void SetupChoices(List<string> choiceList)
        {
            SetupDropdownChoices(this, choiceList);
        }

        public new class UxmlFactory : UxmlFactory<CompatibleDropdownField, UxmlTraits>
        {
        }

        public new class UxmlTraits : BaseField<string>.UxmlTraits
        {
            private UxmlIntAttributeDescription _index = new UxmlIntAttributeDescription { name = "index" };
            private UxmlStringAttributeDescription _choices = new UxmlStringAttributeDescription { name = "choices" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                CompatibleDropdownField dropdownField = (CompatibleDropdownField)ve;
                List<string> choiceList = ParseChoiceList(_choices.GetValueFromBag(bag, cc));
                if (choiceList != null)
                {
                    SetupDropdownChoices(dropdownField, choiceList);
                }
                dropdownField.index = _index.GetValueFromBag(bag, cc);
            }

            private static List<string> ParseChoiceList(string choicesFromBag)
            {
                if (string.IsNullOrEmpty(choicesFromBag.Trim()))
                {
                    return null;
                }
                string[] choiceValues = choicesFromBag.Split(',');
                if (choiceValues.Length > 0)
                {
                    List<string> choiceList = new List<string>();
                    for (int i = 0; i < choiceValues.Length; i++)
                    {
                        choiceList.Add(choiceValues[i].Trim());
                    }
                    return choiceList;
                }
                return null;
            }
        }
    }
}