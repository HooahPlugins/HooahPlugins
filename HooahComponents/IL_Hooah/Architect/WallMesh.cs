using System.Linq;
using HooahUtility.Model;
using MessagePack;
using UnityEngine;

namespace Architect
{
    public class WallMesh : MonoBehaviour, IFormData
    {
        public ArchitectMeshGroup meshGroups;

        private int _meshType = 0;
        private bool _flipNormal = false;
        private Vector3 _pivot = Vector3.zero;
        private Vector3 _scale = Vector3.one;
        private Vector3 _groupAScale = Vector3.one;
        private Vector3 _groupAOffset = Vector3.zero;

#if AI || HS2
        [Key(0)]
        public int MeshType
        {
            get => _meshType;
            set
            {
                _meshType = value;
                GenerateMesh();
            }
        }

        [Key(1)]
        public Vector3 Pivot
        {
            get => _pivot;
            set
            {
                _pivot = value;
                GenerateMesh();
            }
        }

        [Key(2)]
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                GenerateMesh();
            }
        }

        [Key(3)]
        public bool FlipNormal
        {
            get => _flipNormal;
            set
            {
                _flipNormal = value;
                GenerateMesh();
            }
        }

        [Key(10)]
        public Vector3 GroupAScale
        {
            get => _groupAScale;
            set
            {
                _groupAScale = value;
                GenerateMesh();
            }
        }

        [Key(11)]
        public Vector3 GroupAOffset
        {
            get => _groupAOffset;
            set
            {
                _groupAOffset = value;
                GenerateMesh();
            }
        }


#endif
        private Mesh _mesh;
        private MeshRenderer _renderer;
        private MeshFilter _meshFilter;

        void Start()
        {
            if (!_renderer) _renderer = gameObject.GetComponent<MeshRenderer>();
            if (!_meshFilter) _meshFilter = gameObject.GetComponent<MeshFilter>();
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;
        }

        public void GenerateMesh()
        {
            _mesh.Clear();
            var architectMesh = meshGroups.meshes.ElementAtOrDefault(_meshType);
            if (architectMesh == null) return;
            var sampleMesh = architectMesh.mesh;
            if (sampleMesh == null) return;
            // DO THE NORMAL FLIP WITH THE FUCKING SHADHER FUCK FOFF
            var p = Vector3.Scale(_pivot, _scale);
            var meshAdjustGroup = architectMesh.GetAdjustGroups().ToArray(); // prevent duplicated iteration.
            _mesh.vertices = sampleMesh.vertices.Select(
                (x, i) =>
                {
                    x.Scale(_scale);

                    foreach (var adjustGroup in meshAdjustGroup)
                        if (adjustGroup.VerticesSet.Contains(i))
                            x += adjustGroup.ApplyOffset(x, _scale, _groupAScale, _groupAOffset);

                    return x + p;
                }).ToArray();

            _mesh.triangles = sampleMesh.triangles;
            _mesh.normals = sampleMesh.normals.Select((x, i) => x * (_flipNormal ? -1 : 1)).ToArray();
            _mesh.tangents = sampleMesh.tangents;
            _mesh.uv = sampleMesh.uv;
        }

#if UNITY_EDITOR
        [ExecuteInEditMode]
        // Update is called once per frame
        void Update()
        {
            GenerateMesh();
        }
#else
#endif
    }
}
