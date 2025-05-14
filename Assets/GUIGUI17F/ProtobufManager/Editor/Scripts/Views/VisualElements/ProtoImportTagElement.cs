using System.Collections.Generic;
using UnityEngine.UIElements;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// special tag VisualElement to represent an import content in a protobuf file
    /// </summary>
    public class ProtoImportTagElement : LabelTagElement
    {
        public string ImportProto { get; private set; }

        private List<string> _typeList;
        private List<string> _protoTypes;

        public ProtoImportTagElement()
        {
        }

        public ProtoImportTagElement(List<string> typeList, string importProto, List<string> protoTypes) : base($"import \"{importProto}\"")
        {
            _typeList = typeList;
            _protoTypes = protoTypes;
            ImportProto = importProto;
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);
            if (_typeList != null && _protoTypes != null)
            {
                if (evt.eventTypeId == AttachToPanelEvent.TypeId())
                {
                    foreach (string protoType in _protoTypes)
                    {
                        if (!_typeList.Contains(protoType))
                        {
                            _typeList.Add(protoType);
                        }
                    }
                }
                else if (evt.eventTypeId == DetachFromPanelEvent.TypeId())
                {
                    foreach (string protoType in _protoTypes)
                    {
                        _typeList.Remove(protoType);
                    }
                }
            }
        }
    }
}