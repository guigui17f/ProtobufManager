using System;
using System.Text;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// specific logic to handle the relative and absolute name of a protobuf type
    /// </summary>
    public struct TypeNameString
    {
        public string CurrentTypeName { get; private set; }
        public string PreviousTypeName { get; private set; }
        public string CurrentInHierarchyName { get; private set; }
        public string PreviousInHierarchyName { get; private set; }

        private Func<string> _parentNameGetter;
        private StringBuilder _builder;

        public TypeNameString(Func<string> parentNameGetter, StringBuilder builder)
        {
            CurrentTypeName = string.Empty;
            PreviousTypeName = string.Empty;
            CurrentInHierarchyName = string.Empty;
            PreviousInHierarchyName = string.Empty;
            _parentNameGetter = parentNameGetter;
            _builder = builder;
        }

        public void UpdateName(string typeName, bool overridePrevious)
        {
            PreviousTypeName = CurrentTypeName;
            CurrentTypeName = typeName;
            PreviousInHierarchyName = CurrentInHierarchyName;
            CurrentInHierarchyName = GetInHierarchyName(CurrentTypeName);
            if (overridePrevious)
            {
                PreviousTypeName = CurrentTypeName;
                PreviousInHierarchyName = CurrentInHierarchyName;
            }
        }

        public string GetInHierarchyName(string typeName)
        {
            if (_parentNameGetter == null)
            {
                return typeName;
            }
            if (string.IsNullOrEmpty(typeName))
            {
                return string.Empty;
            }
            string parentPrefix = _parentNameGetter();
            if (string.IsNullOrEmpty(parentPrefix))
            {
                return string.Empty;
            }
            _builder.Clear();
            _builder.Append(parentPrefix);
            _builder.Append('.');
            _builder.Append(typeName);
            return _builder.ToString();
        }
    }
}