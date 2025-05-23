// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/oneof_definition.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace GUIGUI17F.ProtobufManager {

  /// <summary>Holder for reflection information generated from Protos/oneof_definition.proto</summary>
  public static partial class OneofDefinitionReflection {

    #region Descriptor
    /// <summary>File descriptor for Protos/oneof_definition.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static OneofDefinitionReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Ch1Qcm90b3Mvb25lb2ZfZGVmaW5pdGlvbi5wcm90bxIaZ3VpZ3VpMTdmLnBy",
            "b3RvYnVmX21hbmFnZXIaHVByb3Rvcy9maWVsZF9kZWZpbml0aW9uLnByb3Rv",
            "ImgKD09uZW9mRGVmaW5pdGlvbhISCgpvbmVvZl9uYW1lGAEgASgJEkEKDG9u",
            "ZW9mX2ZpZWxkcxgCIAMoCzIrLmd1aWd1aTE3Zi5wcm90b2J1Zl9tYW5hZ2Vy",
            "LkZpZWxkRGVmaW5pdGlvbkIeSAOqAhlHVUlHVUkxN0YuUHJvdG9idWZNYW5h",
            "Z2VyYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::GUIGUI17F.ProtobufManager.FieldDefinitionReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::GUIGUI17F.ProtobufManager.OneofDefinition), global::GUIGUI17F.ProtobufManager.OneofDefinition.Parser, new[]{ "OneofName", "OneofFields" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class OneofDefinition : pb::IMessage<OneofDefinition>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<OneofDefinition> _parser = new pb::MessageParser<OneofDefinition>(() => new OneofDefinition());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<OneofDefinition> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::GUIGUI17F.ProtobufManager.OneofDefinitionReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public OneofDefinition() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public OneofDefinition(OneofDefinition other) : this() {
      oneofName_ = other.oneofName_;
      oneofFields_ = other.oneofFields_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public OneofDefinition Clone() {
      return new OneofDefinition(this);
    }

    /// <summary>Field number for the "oneof_name" field.</summary>
    public const int OneofNameFieldNumber = 1;
    private string oneofName_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string OneofName {
      get { return oneofName_; }
      set {
        oneofName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "oneof_fields" field.</summary>
    public const int OneofFieldsFieldNumber = 2;
    private static readonly pb::FieldCodec<global::GUIGUI17F.ProtobufManager.FieldDefinition> _repeated_oneofFields_codec
        = pb::FieldCodec.ForMessage(18, global::GUIGUI17F.ProtobufManager.FieldDefinition.Parser);
    private readonly pbc::RepeatedField<global::GUIGUI17F.ProtobufManager.FieldDefinition> oneofFields_ = new pbc::RepeatedField<global::GUIGUI17F.ProtobufManager.FieldDefinition>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::GUIGUI17F.ProtobufManager.FieldDefinition> OneofFields {
      get { return oneofFields_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as OneofDefinition);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(OneofDefinition other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (OneofName != other.OneofName) return false;
      if(!oneofFields_.Equals(other.oneofFields_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (OneofName.Length != 0) hash ^= OneofName.GetHashCode();
      hash ^= oneofFields_.GetHashCode();
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
      if (OneofName.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(OneofName);
      }
      oneofFields_.WriteTo(output, _repeated_oneofFields_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (OneofName.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(OneofName);
      }
      oneofFields_.WriteTo(ref output, _repeated_oneofFields_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (OneofName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(OneofName);
      }
      size += oneofFields_.CalculateSize(_repeated_oneofFields_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(OneofDefinition other) {
      if (other == null) {
        return;
      }
      if (other.OneofName.Length != 0) {
        OneofName = other.OneofName;
      }
      oneofFields_.Add(other.oneofFields_);
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
            OneofName = input.ReadString();
            break;
          }
          case 18: {
            oneofFields_.AddEntriesFrom(input, _repeated_oneofFields_codec);
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
            OneofName = input.ReadString();
            break;
          }
          case 18: {
            oneofFields_.AddEntriesFrom(ref input, _repeated_oneofFields_codec);
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
