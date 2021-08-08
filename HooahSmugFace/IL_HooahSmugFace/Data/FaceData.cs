using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AIChara;
using UnityEngine;

namespace HooahSmugFace.IL_HooahSmugFace.Data
{
    public class FaceData
    {
        public enum FacePart
        {
            Head, EyeLashes, Tongue, Tooth, Tears,
            EyeBaseL, EyeBaseR
        }

        private static readonly (FacePart, string)[] FaceSet =
        {
            (FacePart.Head, "o_head"),
            (FacePart.EyeLashes, "o_eyelashes"),
            (FacePart.Tongue, "o_tang"),
            (FacePart.Tooth, "o_tooth"),
            (FacePart.Tears, "o_namida"),
            (FacePart.EyeBaseL, "o_eyebase_L"),
            (FacePart.EyeBaseR, "o_eyebase_R")
        };

        private static readonly Dictionary<FacePart, string> FaceToMesh;
        private static readonly Dictionary<string, FacePart> MeshToFace;

        static FaceData()
        {
            FaceToMesh = FaceSet.ToDictionary(x => x.Item1, x => x.Item2);
            MeshToFace = FaceSet.ToDictionary(x => x.Item2, x => x.Item1);
        }

        public struct VertexArray
        {
            public Vector3[] Normal;
            public Vector3[] Offset;
            public Vector3[] Tangent;
        }

        public struct OriginalVertex
        {
            public Vector3[] Normal;
            public Vector3[] Position;
            public Vector4[] Tangent;
        }

        public static Dictionary<int, List<FaceData>> Registry = new Dictionary<int, List<FaceData>>();
        public Dictionary<FacePart, VertexArray> VertexArrays = new Dictionary<FacePart, VertexArray>();
        public int FaceID;
        public string Key;
        public int Version;

        public static Dictionary<int, Dictionary<FacePart, OriginalVertex>> OriginalVertexRegistry =
            new Dictionary<int, Dictionary<FacePart, OriginalVertex>>();

        public static void CacheOriginalMesh(int key, CmpFace face)
        {
            if (!OriginalVertexRegistry.ContainsKey(key))
            {
                OriginalVertexRegistry[key] = TargetMeshes(face)
                    .Where(x => MeshToFace.ContainsKey(x.name))
                    .ToDictionary(
                        smr => MeshToFace[smr.name],
                        smr =>
                        {
                            Mesh sharedMesh;
                            return new OriginalVertex()
                            {
                                Position = (sharedMesh = smr.sharedMesh).vertices,
                                Normal = sharedMesh.normals,
                                Tangent = sharedMesh.tangents
                            };
                        });
            }
        }

        public static bool TryGetData(int key, out List<FaceData> result)
        {
            return Registry.TryGetValue(key, out result);
        }

        public static void Register(int key, FaceData data)
        {
            if (!Registry.ContainsKey(key)) Registry.Add(key, new List<FaceData>());
            Registry[key].Add(data);
        }

        private static Vector3[] ReadVectorArray(BinaryReader reader)
        {
            var len = reader.ReadInt32();
            return len <= 0
                ? new Vector3[] { }
                : Enumerable.Range(0, len)
                    .Select(x => new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()))
                    .ToArray();
        }

        public static void Load()
        {
            foreach (var file in Directory.GetFiles(Config.DefaultFaceOffsetPath))
                using (var reader = new BinaryReader(File.Open(file, FileMode.Open)))
                    Read(reader);

            FaceOffset.Init();
        }

        public static void Read(BinaryReader reader)
        {
            try
            {
                var faceData = new FaceData
                {
                    Version = reader.ReadInt32(),
                    FaceID = reader.ReadInt32(),
                    Key = reader.ReadString(),
                };

                var count = reader.ReadInt32();
                foreach (var _ in Enumerable.Range(0, count))
                {
                    var part = (FacePart) reader.ReadInt32();
                    var array = new VertexArray
                    {
                        Offset = ReadVectorArray(reader),
                        Normal = ReadVectorArray(reader),
                        Tangent = ReadVectorArray(reader)
                    };
                    faceData.VertexArrays[part] = array;
                }

                Register(faceData.FaceID, faceData);
            }
            catch (Exception e)
            {
                HooahSmugFacePlugin._logger.LogError(e);
            }
        }

        private static SkinnedMeshRenderer[] TargetMeshes(CmpFace cmp) => cmp
            .GetComponentsInChildren<SkinnedMeshRenderer>()
            .Where(x => MeshToFace.ContainsKey(x.name))
            .ToArray();

        public void Apply(CmpFace cmp)
        {
            var meshes = TargetMeshes(cmp);

            foreach (var smr in meshes)
            {
                var key = MeshToFace[smr.name];
                if (smr == null || !VertexArrays.TryGetValue(key, out var a)) continue;
                try
                {
                    smr.sharedMesh.AddBlendShapeFrame(Key, 100f, a.Offset, null, null);
                }
                catch (ArgumentException arg)
                {
                    // fuck off
                }
            }
        }
    }
}
