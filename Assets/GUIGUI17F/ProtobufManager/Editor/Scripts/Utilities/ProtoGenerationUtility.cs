using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace GUIGUI17F.ProtobufManager
{
    public class ProtoGenerationUtility
    {
        private const string Indent = "  ";

        private static StringBuilder _builder = new StringBuilder();

        public static void GenerateProtoFiles(ProtoFileDefinition data, string path)
        {
            using (FileStream fs = File.Create(path))
            {
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.WriteLine("syntax = \"proto3\";");
                    if (!string.IsNullOrEmpty(data.PackageName))
                    {
                        writer.Write("package ");
                        writer.Write(data.PackageName);
                        writer.WriteLine(';');
                    }
                    foreach (string importProto in data.ImportProtos)
                    {
                        writer.Write("import \"");
                        writer.Write(importProto);
                        writer.WriteLine("\";");
                    }
                    if (!string.IsNullOrEmpty(data.CsharpNamespace))
                    {
                        writer.Write("option csharp_namespace = \"");
                        writer.Write(data.CsharpNamespace);
                        writer.WriteLine("\";");
                    }
                    switch ((ProtoOptimizeType)data.OptimizeType)
                    {
                        case ProtoOptimizeType.CodeSize:
                            writer.WriteLine("option optimize_for = CODE_SIZE;");
                            break;
                        case ProtoOptimizeType.LiteRuntime:
                            writer.WriteLine("option optimize_for = LITE_RUNTIME;");
                            break;
                    }
                    if (!string.IsNullOrEmpty(data.OtherOptions))
                    {
                        writer.WriteLine(data.OtherOptions);
                    }
                    foreach (EnumDefinition enumDefinition in data.EnumDefinitions)
                    {
                        writer.WriteLine();
                        WriteEnumDefinition(enumDefinition, 0, writer);
                    }
                    foreach (MessageDefinition messageDefinition in data.MessageDefinitions)
                    {
                        writer.WriteLine();
                        WriteMessageDefinition(messageDefinition, 0, writer, string.Empty);
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        private static void WriteEnumDefinition(EnumDefinition data, int indentCount, StreamWriter writer)
        {
            WriteIndent(indentCount, writer);
            writer.Write("enum ");
            writer.Write(data.EnumName);
            writer.WriteLine(" {");
            if (data.AllowAlias)
            {
                WriteIndent(indentCount + 1, writer);
                writer.WriteLine("option allow_alias = true;");
            }
            for (int i = 0; i < data.ValueNames.Count; i++)
            {
                WriteIndent(indentCount + 1, writer);
                writer.Write(data.ValueNames[i]);
                writer.Write(" = ");
                writer.Write(data.ValueNumbers[i]);
                writer.WriteLine(';');
            }
            if (data.ReservedNumbers.Count > 0)
            {
                WriteIndent(indentCount + 1, writer);
                writer.Write("reserved ");
                writer.Write(data.ReservedNumbers[0]);
                for (int i = 1; i < data.ReservedNumbers.Count; i++)
                {
                    writer.Write(", ");
                    writer.Write(data.ReservedNumbers[i]);
                }
                writer.WriteLine(';');
            }
            if (data.ReservedNames.Count > 0)
            {
                WriteIndent(indentCount + 1, writer);
                writer.Write("reserved ");
                writer.Write(data.ReservedNames[0]);
                for (int i = 1; i < data.ReservedNames.Count; i++)
                {
                    writer.Write(", ");
                    writer.Write(data.ReservedNames[i]);
                }
                writer.WriteLine(';');
            }
            WriteIndent(indentCount, writer);
            writer.WriteLine('}');
        }

        private static void WriteMessageDefinition(MessageDefinition data, int indentCount, StreamWriter writer, string parentPrefix)
        {
            _builder.Clear();
            if (!string.IsNullOrEmpty(parentPrefix))
            {
                _builder.Append(parentPrefix);
            }
            _builder.Append(data.MessageName);
            _builder.Append('.');
            string selfPrefix = _builder.ToString();

            WriteIndent(indentCount, writer);
            writer.Write("message ");
            writer.Write(data.MessageName);
            writer.WriteLine(" {");
            foreach (EnumDefinition enumDefinition in data.EnumDefinitions)
            {
                WriteEnumDefinition(enumDefinition, indentCount + 1, writer);
            }
            foreach (MessageDefinition messageDefinition in data.MessageDefinitions)
            {
                WriteMessageDefinition(messageDefinition, indentCount + 1, writer, selfPrefix);
            }
            List<object> fieldList = new List<object>();
            while (data.NormalFields.Count > 0 || data.MapFields.Count > 0 || data.OneofFields.Count > 0)
            {
                int normalNumber = data.NormalFields.Count > 0 ? data.NormalFields[0].FieldNumber : int.MaxValue;
                int mapNumber = data.MapFields.Count > 0 ? data.MapFields[0].FieldNumber : int.MaxValue;
                int oneofNumber = data.OneofFields.Count > 0 ? data.OneofFields[0].OneofFields[0].FieldNumber : int.MaxValue;
                if (normalNumber < mapNumber && normalNumber < oneofNumber)
                {
                    fieldList.Add(data.NormalFields[0]);
                    data.NormalFields.RemoveAt(0);
                }
                else if (mapNumber < normalNumber && mapNumber < oneofNumber)
                {
                    fieldList.Add(data.MapFields[0]);
                    data.MapFields.RemoveAt(0);
                }
                else if (data.OneofFields.Count > 0)
                {
                    fieldList.Add(data.OneofFields[0]);
                    data.OneofFields.RemoveAt(0);
                }
            }
            foreach (object field in fieldList)
            {
                if (field is FieldDefinition fieldDefinition)
                {
                    WriteIndent(indentCount + 1, writer);
                    if (fieldDefinition.IsRepeated)
                    {
                        writer.Write("repeated ");
                    }
                    writer.Write(ModifyTypeName(fieldDefinition.FieldType, selfPrefix));
                    writer.Write(' ');
                    writer.Write(fieldDefinition.FieldName);
                    writer.Write(" = ");
                    writer.Write(fieldDefinition.FieldNumber);
                    writer.WriteLine(';');
                }
                else if (field is MapDefinition mapDefinition)
                {
                    WriteIndent(indentCount + 1, writer);
                    writer.Write("map<");
                    writer.Write(mapDefinition.KeyType);
                    writer.Write(", ");
                    writer.Write(ModifyTypeName(mapDefinition.ValueType, selfPrefix));
                    writer.Write("> ");
                    writer.Write(mapDefinition.MapName);
                    writer.Write(" = ");
                    writer.Write(mapDefinition.FieldNumber);
                    writer.WriteLine(';');
                }
                else
                {
                    OneofDefinition oneofDefinition = field as OneofDefinition;
                    WriteIndent(indentCount + 1, writer);
                    writer.Write("oneof ");
                    writer.Write(oneofDefinition.OneofName);
                    writer.WriteLine(" {");
                    foreach (FieldDefinition oneofField in oneofDefinition.OneofFields)
                    {
                        WriteIndent(indentCount + 2, writer);
                        writer.Write(ModifyTypeName(oneofField.FieldType, selfPrefix));
                        writer.Write(' ');
                        writer.Write(oneofField.FieldName);
                        writer.Write(" = ");
                        writer.Write(oneofField.FieldNumber);
                        writer.WriteLine(';');
                    }
                    WriteIndent(indentCount + 1, writer);
                    writer.WriteLine("}");
                }
            }
            WriteIndent(indentCount, writer);
            writer.WriteLine('}');
        }

        private static void WriteIndent(int count, StreamWriter writer)
        {
            for (int i = 0; i < count; i++)
            {
                writer.Write(Indent);
            }
        }

        private static string ModifyTypeName(string typeName, string trimPrefix)
        {
            if (string.IsNullOrEmpty(trimPrefix))
            {
                return typeName;
            }
            if (typeName.StartsWith(trimPrefix))
            {
                return typeName.Substring(trimPrefix.Length);
            }
            return typeName;
        }
    }
}