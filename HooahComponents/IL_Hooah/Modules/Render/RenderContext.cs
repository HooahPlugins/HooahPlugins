using UnityEngine;

namespace HooahComponents.Modules.Render
{
    /*
     * Render Context
     * Used for rendering with resources.
     * todo: create render texture manager
     * todo: remove all rendered texture in memory when new scene has loaded.
     * todo: create cache texture in the directory
     * todo: embed cached texture into the scene or make it referenced.
     * todo: key-value array for dynamic generation
     * todo: text generation
     */
    public class RenderContext : ScriptableObject
    {
        public Material material;
    }
}