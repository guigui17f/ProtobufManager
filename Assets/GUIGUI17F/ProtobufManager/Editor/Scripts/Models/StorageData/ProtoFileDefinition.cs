// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/proto_file_definition.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace GUIGUI17F.ProtobufManager {

  /// <summary>Holder for reflection information generated from Protos/proto_file_definition.proto</summary>
  public static partial class ProtoFileDefinitionReflection {

    #region Descriptor
    /// <summary>File descriptor for Protos/proto_file_definition.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ProtoFileDefinitionReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CiJQcm90b3MvcHJvdG9fZmlsZV9kZWZpbml0aW9uLnByb3RvEhpndWlndWkx",
            "N2YucHJvdG9idWZfbWFuYWdlchocUHJvdG9zL2VudW1fZGVmaW5pdGlvbi5w",
            "cm90bxofUHJvdG9zL21lc3NhZ2VfZGVmaW5pdGlvbi5wcm90byKcAgoTUHJv",
            "dG9GaWxlRGVmaW5pdGlvbhIVCg1pbXBvcnRfcHJvdG9zGAEgAygJEhQKDHBh",
            "Y2thZ2VfbmFtZRgCIAEoCRIYChBjc2hhcnBfbmFtZXNwYWNlGAMgASgJEhUK",
            "DW9wdGltaXplX3R5cGUYBCABKAUSRAoQZW51bV9kZWZpbml0aW9ucxgFIAMo",
            "CzIqLmd1aWd1aTE3Zi5wcm90b2J1Zl9tYW5hZ2VyLkVudW1EZWZpbml0aW9u",
            "EkoKE21lc3NhZ2VfZGVmaW5pdGlvbnMYBiADKAsyLS5ndWlndWkxN2YucHJv",
            "dG9idWZfbWFuYWdlci5NZXNzYWdlRGVmaW5pdGlvbhIVCg1vdGhlcl9vcHRp",
            "b25zGAcgASgJQh5IA6oCGUdVSUdVSTE3Ri5Qcm90b2J1Zk1hbmFnZXJiBnBy",
            "b3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::GUIGUI17F.ProtobufManager.EnumDefinitionReflection.Descriptor, global::GUIGUI17F.ProtobufManager.MessageDefinitionReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::GUIGUI17F.ProtobufManager.ProtoFileDefinition), global::GUIGUI17F.ProtobufManager.ProtoFileDefinition.Parser, new[]{ "ImportProtos", "PackageName", "CsharpNamespace", "OptimizeType", "EnumDefinitions", "MessageDefinitions", "OtherOptions" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class ProtoFileDefinition : pb::IMessage<ProtoFileDefinition>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ProtoFileDefinition> _parser = new pb::MessageParser<ProtoFileDefinition>(() => new ProtoFileDefinition());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ProtoFileDefinition> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::GUIGUI17F.ProtobufManager.ProtoFileDefinitionReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ProtoFileDefinition() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ProtoFileDefinition(ProtoFileDefinition other) : this() {
      importProtos_ = other.importProtos_.Clone();
      packageName_ = other.packageName_;
      csharpNamespace_ = other.csharpNamespace_;
      optimizeType_ = other.optimizeType_;
      enumDefinitions_ = other.enumDefinitions_.Clone();
      messageDefinitions_ = other.messageDefinitions_.Clone();
      otherOptions_ = other.otherOptions_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ProtoFileDefinition Clone() {
      return new ProtoFileDefinition(this);
    }

    /// <summary>Field number for the "import_protos" field.</summary>
    public const int ImportProtosFieldNumber = 1;
    private static readonly pb::FieldCodec<string> _repeated_importProtos_codec
        = pb::FieldCodec.ForString(10);
    private readonly pbc::RepeatedField<string> importProtos_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<string> ImportProtos {
      get { return importProtos_; }
    }

    /// <summary>Field number for the "package_name" field.</summary>
    public const int PackageNameFieldNumber = 2;
    private string packageName_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string PackageName {
      get { return packageName_; }
      set {
        packageName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "csharp_namespace" field.</summary>
    public const int CsharpNamespaceFieldNumber = 3;
    private string csharpNamespace_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string CsharpNamespace {
      get { return csharpNamespace_; }
      set {
        csharpNamespace_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "optimize_type" field.</summary>
    public const int OptimizeTypeFieldNumber = 4;
    private int optimizeType_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int OptimizeType {
      get { return optimizeType_; }
      set {
        optimizeType_ = value;
      }
    }

    /// <summary>Field number for the "enum_definitions" field.</summary>
    public const int EnumDefinitionsFieldNumber = 5;
    private static readonly pb::FieldCodec<global::GUIGUI17F.ProtobufManager.EnumDefinition> _repeated_enumDefinitions_codec
        = pb::FieldCodec.ForMessage(42, global::GUIGUI17F.ProtobufManager.EnumDefinition.Parser);
    private readonly pbc::RepeatedField<global::GUIGUI17F.ProtobufManager.EnumDefinition> enumDefinitions_ = new pbc::RepeatedField<global::GUIGUI17F.ProtobufManager.EnumDefinition>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::GUIGUI17F.ProtobufManager.EnumDefinition> EnumDefinitions {
      get { return enumDefinitions_; }
    }

    /// <summary>Field number for the "message_definitions" field.</summary>
    public const int MessageDefinitionsFieldNumber = 6;
    private static readonly pb::FieldCodec<global::GUIGUI17F.ProtobufManager.MessageDefinition> _repeated_messageDefinitions_codec
        = pb::FieldCodec.ForMessage(50, global::GUIGUI17F.ProtobufManager.MessageDefinition.Parser);
    private readonly pbc::RepeatedField<global::GUIGUI17F.ProtobufManager.MessageDefinition> messageDefinitions_ = new pbc::RepeatedField<global::GUIGUI17F.ProtobufManager.MessageDefinition>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::GUIGUI17F.ProtobufManager.MessageDefinition> MessageDefinitions {
      get { return messageDefinitions_; }
    }

    /// <summary>Field number for the "other_options" field.</summary>
    public const int OtherOptionsFieldNumber = 7;
    private string otherOptions_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string OtherOptions {
      get { return otherOptions_; }
      set {
        otherOptions_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ProtoFileDefinition);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ProtoFileDefinition other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!importProtos_.Equals(other.importProtos_)) return false;
      if (PackageName != other.PackageName) return false;
      if (CsharpNamespace != other.CsharpNamespace) return false;
      if (OptimizeType != other.OptimizeType) return false;
      if(!enumDefinitions_.Equals(other.enumDefinitions_)) return false;
      if(!messageDefinitions_.Equals(other.messageDefinitions_)) return false;
      if (OtherOptions != other.OtherOptions) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= importProtos_.GetHashCode();
      if (PackageName.Length != 0) hash ^= PackageName.GetHashCode();
      if (CsharpNamespace.Length != 0) hash ^= CsharpNamespace.GetHashCode();
      if (OptimizeType != 0) hash ^= OptimizeType.GetHashCode();
      hash ^= enumDefinitions_.GetHashCode();
      hash ^= messageDefinitions_.GetHashCode();
      if (OtherOptions.Length != 0) hash ^= OtherOptions.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      importProtos_.WriteTo(output, _repeated_importProtos_codec);
      if (PackageName.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(PackageName);
      }
      if (CsharpNamespace.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(CsharpNamespace);
      }
      if (OptimizeType != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(OptimizeType);
      }
      enumDefinitions_.WriteTo(output, _repeated_enumDefinitions_codec);
      messageDefinitions_.WriteTo(output, _repeated_messageDefinitions_codec);
      if (OtherOptions.Length != 0) {
        output.WriteRawTag(58);
        output.WriteString(OtherOptions);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      importProtos_.WriteTo(ref output, _repeated_importProtos_codec);
      if (PackageName.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(PackageName);
      }
      if (CsharpNamespace.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(CsharpNamespace);
      }
      if (OptimizeType != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(OptimizeType);
      }
      enumDefinitions_.WriteTo(ref output, _repeated_enumDefinitions_codec);
      messageDefinitions_.WriteTo(ref output, _repeated_messageDefinitions_codec);
      if (OtherOptions.Length != 0) {
        output.WriteRawTag(58);
        output.WriteString(OtherOptions);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      size += importProtos_.CalculateSize(_repeated_importProtos_codec);
      if (PackageName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(PackageName);
      }
      if (CsharpNamespace.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(CsharpNamespace);
      }
      if (OptimizeType != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(OptimizeType);
      }
      size += enumDefinitions_.CalculateSize(_repeated_enumDefinitions_codec);
      size += messageDefinitions_.CalculateSize(_repeated_messageDefinitions_codec);
      if (OtherOptions.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(OtherOptions);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ProtoFileDefinition other) {
      if (other == null) {
        return;
      }
      importProtos_.Add(other.importProtos_);
      if (other.PackageName.Length != 0) {
        PackageName = other.PackageName;
      }
      if (other.CsharpNamespace.Length != 0) {
        CsharpNamespace = other.CsharpNamespace;
      }
      if (other.OptimizeType != 0) {
        OptimizeType = other.OptimizeType;
      }
      enumDefinitions_.Add(other.enumDefinitions_);
      messageDefinitions_.Add(other.messageDefinitions_);
      if (other.OtherOptions.Length != 0) {
        OtherOptions = other.OtherOptions;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
      if ((tag & 7) == 4) {
        // Abort on any end group tag.
        return;
      }
      switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            importProtos_.AddEntriesFrom(input, _repeated_importProtos_codec);
            break;
          }
          case 18: {
            PackageName = input.ReadString();
            break;
          }
          case 26: {
            CsharpNamespace = input.ReadString();
            break;
          }
          case 32: {
            OptimizeType = input.ReadInt32();
            break;
          }
          case 42: {
            enumDefinitions_.AddEntriesFrom(input, _repeated_enumDefinitions_codec);
            break;
          }
          case 50: {
            messageDefinitions_.AddEntriesFrom(input, _repeated_messageDefinitions_codec);
            break;
          }
          case 58: {
            OtherOptions = input.ReadString();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
      if ((tag & 7) == 4) {
        // Abort on any end group tag.
        return;
      }
      switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            importProtos_.AddEntriesFrom(ref input, _repeated_importProtos_codec);
            break;
          }
          case 18: {
            PackageName = input.ReadString();
            break;
          }
          case 26: {
            CsharpNamespace = input.ReadString();
            break;
          }
          case 32: {
            OptimizeType = input.ReadInt32();
            break;
          }
          case 42: {
            enumDefinitions_.AddEntriesFrom(ref input, _repeated_enumDefinitions_codec);
            break;
          }
          case 50: {
            messageDefinitions_.AddEntriesFrom(ref input, _repeated_messageDefinitions_codec);
            break;
          }
          case 58: {
            OtherOptions = input.ReadString();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
