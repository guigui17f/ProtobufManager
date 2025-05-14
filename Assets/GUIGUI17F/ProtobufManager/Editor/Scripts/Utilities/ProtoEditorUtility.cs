using System;
using Google.Protobuf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Google.Protobuf.Reflection;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using FileOptions = Google.Protobuf.Reflection.FileOptions;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// utility for ProtobufManager Editor Window
    /// </summary>
    public static class ProtoEditorUtility
    {
        /// <summary>
        /// the maximum value allowed for protobuf field number
        /// </summary>
        public const int MaxFieldNumber = 536870911;

        /// <summary>
        /// the numbers 19000 through 19999 are reserved for the protobuf implementation
        /// </summary>
        public const int ProtoFirstReservedNumber = 19000;

        /// <summary>
        /// the numbers 19000 through 19999 are reserved for the protobuf implementation
        /// </summary>
        public const int ProtoLastReservedNumber = 19999;

        /// <summary>
        /// the protobuf built-in types
        /// </summary>
        public static readonly List<string> BuiltInTypes = new List<string> { "int32", "int64", "uint32", "uint64", "float", "double", "bool", "string", "bytes" };

        /// <summary>
        /// types used for the key of the protobuf map fields 
        /// </summary>
        public static readonly List<string> MapKeyTypes = new List<string> { "int32", "int64", "uint32", "uint64", "bool", "string" };

        private static readonly List<char> NumberChars = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private static StringBuilder _builder = new StringBuilder();
        private static string _workspaceCachePath;
        private static string _tempCachePath;
        private static DateTime _baseTime;

        /// <summary>
        /// reset the timer of the window shown time
        /// </summary>
        public static void ResetBaseTime()
        {
            _baseTime = DateTime.Now;
        }

        public static long GetWindowShownTime()
        {
            TimeSpan span = DateTime.Now - _baseTime;
            return (long)span.TotalMilliseconds;
        }

        /// <summary>
        /// get the protobuf built-in type list
        /// </summary>
        public static List<string> GetBuiltInTypeList()
        {
            List<string> types = new List<string>(BuiltInTypes);
            return types;
        }

        /// <summary>
        /// get the types in the target proto file
        /// </summary>
        public static List<string> GetProtoTypes(string protoPath)
        {
            FileDescriptorSet fileSet = ProtoCompilerUtility.GetFileDescription(protoPath);
            if (fileSet != null && fileSet.File.Count > 0)
            {
                return GetProtoTypes(fileSet.File[0]);
            }
            return null;
        }

        /// <summary>
        /// get the file paths of current user selections in the Unity Editor
        /// </summary>
        /// <param name="extensionName">file extension name used as the filter, use null to include all files</param>
        public static List<string> GetSelectionFilePaths(string extensionName)
        {
            List<string> filePaths = new List<string>();
            List<string> directories = new List<string>();
            string[] guids = Selection.assetGUIDs;
            bool selectAll = string.IsNullOrEmpty(extensionName);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (Directory.Exists(path))
                {
                    directories.Add(path);
                }
                else if (selectAll || Path.GetExtension(path) == extensionName)
                {
                    filePaths.Add(path);
                }
            }
            if (directories.Count > 0)
            {
                guids = AssetDatabase.FindAssets(string.Empty, directories.ToArray());
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    if (File.Exists(path) && (selectAll || Path.GetExtension(path) == extensionName))
                    {
                        filePaths.Add(path);
                    }
                }
            }
            return filePaths;
        }

        /// <summary>
        /// get the first matched element in every child element tree
        /// </summary>
        public static List<T> QueryChildrenElement<T>(this VisualElement root, List<T> containerList) where T : VisualElement
        {
            containerList.Clear();
            IEnumerable<VisualElement> children = root.Children();
            foreach (VisualElement element in children)
            {
                T child = element.Q<T>();
                if (child != null)
                {
                    containerList.Add(child);
                }
            }
            return containerList;
        }

        #region Validation

        public static bool PassNumberValidation(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            if (value.Length > 9)
            {
                return false;
            }
            for (int i = 0; i < value.Length; i++)
            {
                if (!NumberChars.Contains(value[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsValidFieldNumber(string typeName, string fieldType, string fieldName, int number, StringBuilder logBuilder)
        {
            if (number < 1)
            {
                logBuilder.Append($"{typeName} - {fieldType} {fieldName}: field contains invalid field number {number.ToString()}, number can't be smaller than 1!");
                return false;
            }
            if (number > MaxFieldNumber)
            {
                logBuilder.Append($"{typeName} - {fieldType} {fieldName}: field contains invalid field number {number.ToString()}, number can't be larger than {MaxFieldNumber.ToString()}!");
                return false;
            }
            if (number >= ProtoFirstReservedNumber && number <= ProtoLastReservedNumber)
            {
                logBuilder.Append($"{typeName} - {fieldType} {fieldName}: field contains invalid field number {number.ToString()}, number can't be between {ProtoFirstReservedNumber.ToString()} and {ProtoLastReservedNumber.ToString()}!");
                return false;
            }
            return true;
        }

        #endregion

        #region Handle Caches

        public static ProtoFileDefinition LoadEditorWindowCache(bool useTemp)
        {
            string path = useTemp ? GetEditorTempCachePath() : GetEditorWorkspaceCachePath();
            ProtoFileDefinition definition = null;
            if (File.Exists(path))
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    definition = ProtoFileDefinition.Parser.ParseFrom(stream);
                }
                if (useTemp)
                {
                    File.Delete(path);
                }
            }
            return definition;
        }

        public static void SaveEditorWindowCache(ProtoFileDefinition saveData, bool useTemp)
        {
            string path = useTemp ? GetEditorTempCachePath() : GetEditorWorkspaceCachePath();
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (FileStream stream = File.Create(path))
            {
                saveData.WriteTo(stream);
            }
        }

        public static ProtoFileDefinition ConvertProtoToCache(string protoPath)
        {
            FileDescriptorSet fileSet = ProtoCompilerUtility.GetFileDescription(protoPath);
            if (fileSet != null && fileSet.File.Count > 0)
            {
                FileDescriptorProto fileData = fileSet.File[0];
                ProtoFileDefinition fileDefinition = new ProtoFileDefinition();
                fileDefinition.ImportProtos.AddRange(fileData.Dependency);
                fileDefinition.PackageName = fileData.Package;

                if (fileData.Options != null)
                {
                    fileDefinition.CsharpNamespace = fileData.Options.CsharpNamespace;
                    switch (fileData.Options.OptimizeFor)
                    {
                        case FileOptions.Types.OptimizeMode.Speed:
                            fileDefinition.OptimizeType = (int)ProtoOptimizeType.Speed;
                            break;
                        case FileOptions.Types.OptimizeMode.CodeSize:
                            fileDefinition.OptimizeType = (int)ProtoOptimizeType.CodeSize;
                            break;
                        case FileOptions.Types.OptimizeMode.LiteRuntime:
                            fileDefinition.OptimizeType = (int)ProtoOptimizeType.LiteRuntime;
                            break;
                    }

                    if (File.Exists(protoPath))
                    {
                        _builder.Clear();
                        using (var fs = File.Open(protoPath, FileMode.Open, FileAccess.Read))
                        {
                            using (var sr = new StreamReader(fs, Encoding.UTF8))
                            {
                                while (!sr.EndOfStream)
                                {
                                    var line = sr.ReadLine();
                                    if (line != null 
                                        && line.StartsWith("option ") 
                                        && !line.StartsWith("option csharp_namespace") 
                                        && !line.StartsWith("option optimize_for"))
                                    {
                                        _builder.AppendLine(line);
                                    }
                                }
                            }
                        }
                        if (_builder.Length > 0)
                        {
                            fileDefinition.OtherOptions = _builder.ToString().Trim();
                        }
                    }
                }

                foreach (EnumDescriptorProto enumData in fileData.EnumType)
                {
                    fileDefinition.EnumDefinitions.Add(ConvertEnumData(enumData));
                }

                _builder.Clear();
                if (!string.IsNullOrEmpty(fileData.Package))
                {
                    _builder.Append('.');
                    _builder.Append(fileData.Package);
                }
                _builder.Append('.');
                string parentPrefix = _builder.ToString();
                List<string> selfTypes = GetProtoTypes(fileData);
                foreach (DescriptorProto messageData in fileData.MessageType)
                {
                    fileDefinition.MessageDefinitions.Add(ConvertMessageData(messageData, fileData.Package, parentPrefix, selfTypes));
                }
                return fileDefinition;
            }
            return null;
        }

        public static ProtoFileDefinition ConvertJsonToCache(string jsonText)
        {
            List<MessageDefinition> definitionList = new List<MessageDefinition>();
            Stack<JObject> objectStack = new Stack<JObject>();
            Stack<MessageDefinition> definitionStack = new Stack<MessageDefinition>();
            if (jsonText.StartsWith("{"))
            {
                JObject jsonObject;
                try
                {
                    jsonObject = JObject.Parse(jsonText);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return null;
                }
                objectStack.Push(jsonObject);
                definitionStack.Push(new MessageDefinition { MessageName = "NewMessageName" });
            }
            else if (jsonText.StartsWith("["))
            {
                JArray jsonArray;
                try
                {
                    jsonArray = JArray.Parse(jsonText);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return null;
                }
                foreach (JToken token in jsonArray)
                {
                    if (token.Type == JTokenType.Object)
                    {
                        objectStack.Push(token.ToObject<JObject>());
                        definitionStack.Push(new MessageDefinition { MessageName = "NewMessageName" });
                        break;
                    }
                }
            }

            JObject currentObject;
            MessageDefinition currentDefinition;
            JToken currentField;
            bool isRepeated;
            bool hasValidField;
            string messageName;
            string typeName;
            int fieldNumber;
            while (objectStack.Count > 0)
            {
                currentObject = objectStack.Pop();
                currentDefinition = definitionStack.Pop();
                foreach (JProperty property in currentObject.Properties())
                {
                    currentField = property.Value;
                    isRepeated = false;
                    fieldNumber = currentDefinition.NormalFields.Count + 1;
                    if (currentField.Type == JTokenType.Array)
                    {
                        isRepeated = true;
                        hasValidField = false;
                        foreach (JToken token in currentField)
                        {
                            if (token.Type != JTokenType.Array)
                            {
                                currentField = token;
                                hasValidField = true;
                                break;
                            }
                        }
                        if (!hasValidField)
                        {
                            continue;
                        }
                    }
                    if (currentField.Type == JTokenType.Object)
                    {
                        objectStack.Push(currentField.ToObject<JObject>());
                        messageName = GetModifiedMessageName(property.Name);
                        if (!definitionList.Exists(item => item.MessageName == messageName))
                        {
                            definitionStack.Push(new MessageDefinition { MessageName = messageName });
                        }
                        currentDefinition.NormalFields.Add(new FieldDefinition
                        {
                            IsRepeated = isRepeated,
                            FieldType = messageName,
                            FieldName = property.Name,
                            FieldNumber = fieldNumber
                        });
                    }
                    else
                    {
                        switch (currentField.Type)
                        {
                            case JTokenType.Integer:
                                typeName = "int32";
                                break;
                            case JTokenType.Float:
                                typeName = "float";
                                break;
                            case JTokenType.Boolean:
                                typeName = "bool";
                                break;
                            case JTokenType.Bytes:
                                typeName = "bytes";
                                break;
                            default:
                                typeName = "string";
                                break;
                        }
                        currentDefinition.NormalFields.Add(new FieldDefinition
                        {
                            IsRepeated = isRepeated,
                            FieldType = typeName,
                            FieldName = property.Name,
                            FieldNumber = fieldNumber
                        });
                    }
                }
                definitionList.Add(currentDefinition);
            }
            if (definitionList.Count > 0)
            {
                ProtoFileDefinition fileDefinition = new ProtoFileDefinition();
                fileDefinition.OptimizeType = (int)ProtoOptimizeType.LiteRuntime;
                fileDefinition.MessageDefinitions.AddRange(definitionList);
                return fileDefinition;
            }
            else
            {
                return null;
            }
        }

        private static string GetEditorWorkspaceCachePath()
        {
            if (string.IsNullOrEmpty(_workspaceCachePath))
            {
                string pluginDirectory = GlobalPathUtility.GetPluginPath();
                _workspaceCachePath = GlobalPathUtility.CombinePath(pluginDirectory, "Editor/Caches/editor_window_workspace_cache.pb");
            }
            return _workspaceCachePath;
        }

        private static string GetEditorTempCachePath()
        {
            if (string.IsNullOrEmpty(_tempCachePath))
            {
                string pluginDirectory = GlobalPathUtility.GetPluginPath();
                _tempCachePath = GlobalPathUtility.CombinePath(pluginDirectory, "Editor/Caches/editor_window_temp_cache.pb");
            }
            return _tempCachePath;
        }

        private static EnumDefinition ConvertEnumData(EnumDescriptorProto enumData)
        {
            EnumDefinition enumDefinition = new EnumDefinition();
            enumDefinition.EnumName = enumData.Name;
            if (enumData.Options != null)
            {
                enumDefinition.AllowAlias = enumData.Options.AllowAlias;
            }
            foreach (EnumValueDescriptorProto enumValue in enumData.Value)
            {
                enumDefinition.ValueNames.Add(enumValue.Name);
                enumDefinition.ValueNumbers.Add(enumValue.Number);
            }
            foreach (EnumDescriptorProto.Types.EnumReservedRange range in enumData.ReservedRange)
            {
                if (range.Start == range.End)
                {
                    enumDefinition.ReservedNumbers.Add(range.Start.ToString());
                }
                else
                {
                    string endValue = range.End == int.MaxValue ? "max" : range.End.ToString();
                    enumDefinition.ReservedNumbers.Add($"{range.Start.ToString()} to {endValue}");
                }
            }
            foreach (string reservedName in enumData.ReservedName)
            {
                enumDefinition.ReservedNames.Add($"\"{reservedName}\"");
            }
            return enumDefinition;
        }

        private static MessageDefinition ConvertMessageData(DescriptorProto messageData, string packageName, string parentPrefix, List<string> selfTypes)
        {
            _builder.Clear();
            _builder.Append(parentPrefix);
            _builder.Append(messageData.Name);
            _builder.Append('.');
            string selfPrefix = _builder.ToString();
            MessageDefinition messageDefinition = new MessageDefinition();
            messageDefinition.MessageName = messageData.Name;
            foreach (EnumDescriptorProto enumData in messageData.EnumType)
            {
                messageDefinition.EnumDefinitions.Add(ConvertEnumData(enumData));
            }
            Dictionary<string, MapDefinition> mapDictionary = new Dictionary<string, MapDefinition>();
            foreach (DescriptorProto nestedData in messageData.NestedType)
            {
                if (nestedData.Options != null && nestedData.Options.MapEntry)
                {
                    FieldDescriptorProto keyData = nestedData.Field.First(item => item.Name == "key");
                    FieldDescriptorProto valueData = nestedData.Field.First(item => item.Name == "value");
                    MapDefinition mapDefinition = new MapDefinition();
                    mapDefinition.KeyType = ConvertProtoType(keyData.Type, keyData.TypeName, packageName, selfTypes);
                    mapDefinition.ValueType = ConvertProtoType(valueData.Type, valueData.TypeName, packageName, selfTypes);
                    _builder.Clear();
                    _builder.Append(selfPrefix);
                    _builder.Append(nestedData.Name);
                    mapDictionary.Add(_builder.ToString(), mapDefinition);
                }
                else
                {
                    messageDefinition.MessageDefinitions.Add(ConvertMessageData(nestedData, packageName, selfPrefix, selfTypes));
                }
            }
            foreach (OneofDescriptorProto oneofData in messageData.OneofDecl)
            {
                OneofDefinition oneofDefinition = new OneofDefinition();
                oneofDefinition.OneofName = oneofData.Name;
                messageDefinition.OneofFields.Add(oneofDefinition);
            }
            foreach (FieldDescriptorProto fieldData in messageData.Field)
            {
                if (!string.IsNullOrEmpty(fieldData.TypeName) && mapDictionary.ContainsKey(fieldData.TypeName))
                {
                    MapDefinition mapDefinition = mapDictionary[fieldData.TypeName].Clone();
                    mapDefinition.MapName = fieldData.Name;
                    mapDefinition.FieldNumber = fieldData.Number;
                    messageDefinition.MapFields.Add(mapDefinition);
                }
                else
                {
                    FieldDefinition fieldDefinition = new FieldDefinition();
                    fieldDefinition.FieldType = ConvertProtoType(fieldData.Type, fieldData.TypeName, packageName, selfTypes);
                    fieldDefinition.FieldName = fieldData.Name;
                    fieldDefinition.FieldNumber = fieldData.Number;
                    if (fieldData.HasOneofIndex)
                    {
                        messageDefinition.OneofFields[fieldData.OneofIndex].OneofFields.Add(fieldDefinition);
                    }
                    else
                    {
                        fieldDefinition.IsRepeated = fieldData.Label == FieldDescriptorProto.Types.Label.Repeated;
                        messageDefinition.NormalFields.Add(fieldDefinition);
                    }
                }
            }
            foreach (DescriptorProto.Types.ReservedRange range in messageData.ReservedRange)
            {
                if (range.Start == range.End)
                {
                    messageDefinition.ReservedNumbers.Add(range.Start.ToString());
                }
                else
                {
                    string endValue = range.End == int.MaxValue ? "max" : range.End.ToString();
                    messageDefinition.ReservedNumbers.Add($"{range.Start.ToString()} to {endValue}");
                }
            }
            foreach (string reservedName in messageData.ReservedName)
            {
                messageDefinition.ReservedNames.Add($"\"{reservedName}\"");
            }
            return messageDefinition;
        }

        private static string ConvertProtoType(FieldDescriptorProto.Types.Type protoType, string typeName, string packageName, List<string> selfTypes)
        {
            switch (protoType)
            {
                case FieldDescriptorProto.Types.Type.Int32:
                case FieldDescriptorProto.Types.Type.Sint32:
                case FieldDescriptorProto.Types.Type.Sfixed32:
                    return "int32";
                case FieldDescriptorProto.Types.Type.Int64:
                case FieldDescriptorProto.Types.Type.Sint64:
                case FieldDescriptorProto.Types.Type.Sfixed64:
                    return "int64";
                case FieldDescriptorProto.Types.Type.Uint32:
                case FieldDescriptorProto.Types.Type.Fixed32:
                    return "uint32";
                case FieldDescriptorProto.Types.Type.Uint64:
                case FieldDescriptorProto.Types.Type.Fixed64:
                    return "uint64";
                case FieldDescriptorProto.Types.Type.Float:
                    return "float";
                case FieldDescriptorProto.Types.Type.Double:
                    return "double";
                case FieldDescriptorProto.Types.Type.Bool:
                    return "bool";
                case FieldDescriptorProto.Types.Type.String:
                    return "string";
                case FieldDescriptorProto.Types.Type.Bytes:
                    return "bytes";
                default:
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        if (typeName[0] == '.')
                        {
                            typeName = typeName.Substring(1);
                        }
                        if (!string.IsNullOrEmpty(packageName) && selfTypes.Contains(typeName))
                        {
                            typeName = typeName.Substring(packageName.Length + 1);
                        }
                    }
                    return typeName;
            }
        }

        private static List<string> GetProtoTypes(FileDescriptorProto fileData)
        {
            List<string> typeList = new List<string>();
            string packagePrefix = string.Empty;
            if (!string.IsNullOrEmpty(fileData.Package))
            {
                _builder.Clear();
                _builder.Append(fileData.Package);
                _builder.Append('.');
                packagePrefix = _builder.ToString();
            }
            foreach (EnumDescriptorProto enumData in fileData.EnumType)
            {
                _builder.Clear();
                _builder.Append(packagePrefix);
                _builder.Append(enumData.Name);
                typeList.Add(_builder.ToString());
            }
            foreach (DescriptorProto messageData in fileData.MessageType)
            {
                GetMessageTypes(messageData, fileData.Package, typeList);
            }
            return typeList;
        }

        private static void GetMessageTypes(DescriptorProto messageData, string parentName, List<string> typeList)
        {
            _builder.Clear();
            if (!string.IsNullOrEmpty(parentName))
            {
                _builder.Append(parentName);
                _builder.Append('.');
            }
            _builder.Append(messageData.Name);
            string messageName = _builder.ToString();
            typeList.Add(messageName);

            foreach (EnumDescriptorProto enumData in messageData.EnumType)
            {
                _builder.Clear();
                _builder.Append(messageName);
                _builder.Append('.');
                _builder.Append(enumData.Name);
                typeList.Add(_builder.ToString());
            }
            foreach (DescriptorProto nestedType in messageData.NestedType)
            {
                if (nestedType.Options == null || !nestedType.Options.MapEntry)
                {
                    GetMessageTypes(nestedType, messageName, typeList);
                }
            }
        }

        private static string GetModifiedMessageName(string originName)
        {
            _builder.Clear();
            _builder.Append(originName);
            for (int i = 0; i < _builder.Length; i++)
            {
                bool shouldUpper = false;
                while (i < _builder.Length && (_builder[i] == '_' || _builder[i] == '-'))
                {
                    _builder.Remove(i, 1);
                    shouldUpper = true;
                }
                if (shouldUpper && i < _builder.Length)
                {
                    _builder[i] = char.ToUpper(_builder[i]);
                }
            }
            if (_builder.Length > 0)
            {
                _builder[0] = char.ToUpper(_builder[0]);
            }
            return _builder.ToString();
        }

        #endregion
    }
}