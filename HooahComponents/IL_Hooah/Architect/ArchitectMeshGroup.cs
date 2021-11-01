using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Architect
{
    [CreateAssetMenu(fileName = "meshgroup", menuName = "Architect Mesh Group", order = 0)]
    public class ArchitectMeshGroup : ScriptableObject
    {
        public List<ArchitectMeshObject> meshes;
        public string groupName;

        public Mesh GetMesh(int index)
        {
            return meshes.ElementAtOrDefault(index)?.mesh;
        }
    }
}