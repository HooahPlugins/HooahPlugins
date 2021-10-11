using System.Collections.Generic;
using UnityEngine;

namespace HooahComponents.StudioExtension
{
    public class MaterialPresetGroup : ScriptableObject
    {
        // Structure
        // there is few types of variants
        // just textures and materials and shaders.
        // this preset group is just for saving .
        // the hint should be included with the preset group.
        // there should be some sort of validations
        public Material baseMaterial;
        public Material[] materials;
        public string[] names;
    }
}