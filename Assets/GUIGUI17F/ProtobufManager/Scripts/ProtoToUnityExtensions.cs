using UnityEngine;

namespace GUIGUI17F.ProtobufManager
{
    public static class ProtoToUnityExtensions
    {
        public static Vector2 ToUnity(this ProtoVector2 origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new Vector2(origin.X, origin.Y);
        }

        public static Vector2Int ToUnity(this ProtoVector2Int origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new Vector2Int(origin.X, origin.Y);
        }

        public static Vector3 ToUnity(this ProtoVector3 origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new Vector3(origin.X, origin.Y, origin.Z);
        }

        public static Vector3Int ToUnity(this ProtoVector3Int origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new Vector3Int(origin.X, origin.Y, origin.Z);
        }

        public static Vector4 ToUnity(this ProtoVector4 origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new Vector4(origin.X, origin.Y, origin.Z, origin.W);
        }

        public static Quaternion ToUnity(this ProtoQuaternion origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new Quaternion(origin.X, origin.Y, origin.Z, origin.W);
        }

        public static Matrix4x4 ToUnity(this ProtoMatrix4x4 origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new Matrix4x4(origin.Column0.ToUnity(), origin.Column1.ToUnity(), origin.Column2.ToUnity(), origin.Column3.ToUnity());
        }

        public static Color ToUnity(this ProtoColor origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new Color(origin.R, origin.G, origin.B, origin.A);
        }

        public static Color32 ToUnity(this ProtoColor32 origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new Color32((byte)origin.R, (byte)origin.G, (byte)origin.B, (byte)origin.A);
        }

        public static Rect ToUnity(this ProtoRect origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new Rect(origin.X, origin.Y, origin.Width, origin.Height);
        }

        public static RectInt ToUnity(this ProtoRectInt origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new RectInt(origin.XMin, origin.YMin, origin.Width, origin.Height);
        }

        public static Bounds ToUnity(this ProtoBounds origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new Bounds(origin.Center.ToUnity(), origin.Size.ToUnity());
        }

        public static BoundsInt ToUnity(this ProtoBoundsInt origin)
        {
            if (origin == null)
            {
                return default;
            }
            return new BoundsInt(origin.Position.ToUnity(), origin.Size.ToUnity());
        }
    }
}