using System;
using System.Collections.Generic;
using UnityEngine;

namespace Architect
{
    [CreateAssetMenu(fileName = "architect", menuName = "New Architect", order = 0)]
    public class ArchitectMeshObject : ScriptableObject
    {
        public Mesh mesh;
        public string meshName;
        public string meshDescription;

        // Modded struct cannot be deserialized.
        public int[] offsetGroupAVertices;
        public Vector3 offsetGroupAPivot;
        public Vector3 offsetGroupAScaleLimit;

        public struct AdjustGroup
        {
            public int[] Vertices;
            public Vector3 Pivot;
            public Vector3 ScaleLimit;
            public HashSet<int> VerticesSet;

            public AdjustGroup(int[] vertices, Vector3 pivot, Vector3 scaleLimit)
            {
                Vertices = vertices;
                Pivot = pivot;
                ScaleLimit = scaleLimit;
                VerticesSet = new HashSet<int>(vertices);
            }

            public Vector3 ApplyOffset(Vector3 x, Vector3 scale, Vector3 groupScale, Vector3 groupOffset)
            {
                var o = Vector3.Scale(Pivot, scale);
                var v = Vector3.Scale(o - x, groupScale);
                // return Vector3.Scale(o - x, groupScale) * -1 - o;
                return -v + Vector3.Scale(groupOffset, scale);
            }
        }

        public enum Groups { A, B, C, D, E }

        public AdjustGroup GetAdjustGroup(Groups group)
        {
            switch (group)
            {
                case Groups.A:
                    return new AdjustGroup(offsetGroupAVertices, offsetGroupAPivot, offsetGroupAScaleLimit);
                default:
                    throw new ArgumentOutOfRangeException(nameof(@group), @group, null);
            }
        }

        public IEnumerable<AdjustGroup> GetAdjustGroups()
        {
            if (offsetGroupAVertices.Length > 0)
                yield return GetAdjustGroup(Groups.A);
        }
    }
}