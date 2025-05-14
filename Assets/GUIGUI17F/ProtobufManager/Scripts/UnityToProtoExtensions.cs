using GUIGUI17F.ProtobufManager;

namespace UnityEngine
{
    public static class UnityToProtoExtensions
    {
        public static ProtoVector2 ToProto(this Vector2 origin)
        {
            return new ProtoVector2
            {
                X = origin.x,
                Y = origin.y
            };
        }

        public static ProtoVector2Int ToProto(this Vector2Int origin)
        {
            return new ProtoVector2Int
            {
                X = origin.x,
                Y = origin.y
            };
        }

        public static ProtoVector3 ToProto(this Vector3 origin)
        {
            return new ProtoVector3
            {
                X = origin.x,
                Y = origin.y,
                Z = origin.z
            };
        }

        public static ProtoVector3Int ToProto(this Vector3Int origin)
        {
            return new ProtoVector3Int
            {
                X = origin.x,
                Y = origin.y,
                Z = origin.z
            };
        }

        public static ProtoVector4 ToProto(this Vector4 origin)
        {
            return new ProtoVector4
            {
                X = origin.x,
                Y = origin.y,
                Z = origin.z,
                W = origin.w
            };
        }

        public static ProtoQuaternion ToProto(this Quaternion origin)
        {
            return new ProtoQuaternion
            {
                X = origin.x,
                Y = origin.y,
                Z = origin.z,
                W = origin.w
            };
        }

        public static ProtoMatrix4x4 ToProto(this Matrix4x4 origin)
        {
            return new ProtoMatrix4x4
            {
                Column0 = origin.GetColumn(0).ToProto(),
                Column1 = origin.GetColumn(1).ToProto(),
                Column2 = origin.GetColumn(2).ToProto(),
                Column3 = origin.GetColumn(3).ToProto()
            };
        }

        public static ProtoColor ToProto(this Color origin)
        {
            return new ProtoColor
            {
                R = origin.r,
                G = origin.g,
                B = origin.b,
                A = origin.a
            };
        }

        public static ProtoColor32 ToProto(this Color32 origin)
        {
            return new ProtoColor32
            {
                R = origin.r,
                G = origin.g,
                B = origin.b,
                A = origin.a
            };
        }

        public static ProtoRect ToProto(this Rect origin)
        {
            return new ProtoRect
            {
                X = origin.x,
                Y = origin.y,
                Width = origin.width,
                Height = origin.height
            };
        }

        public static ProtoRectInt ToProto(this RectInt origin)
        {
            return new ProtoRectInt
            {
                XMin = origin.xMin,
                YMin = origin.yMin,
                Width = origin.width,
                Height = origin.height
            };
        }

        public static ProtoBounds ToProto(this Bounds origin)
        {
            return new ProtoBounds
            {
                Center = origin.center.ToProto(),
                Size = origin.size.ToProto()
            };
        }

        public static ProtoBoundsInt ToProto(this BoundsInt origin)
        {
            return new ProtoBoundsInt
            {
                Position = origin.position.ToProto(),
                Size = origin.size.ToProto()
            };
        }
    }
}