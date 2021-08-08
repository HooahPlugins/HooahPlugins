using UnityEngine;

namespace HooahUtility.Utility
{
    public class ImageUtility
    {
        public static readonly CubemapFace[] LinearCubemap = {
            CubemapFace.PositiveX,
            CubemapFace.NegativeX,
            CubemapFace.PositiveY,
            CubemapFace.NegativeY,
            CubemapFace.PositiveZ,
            CubemapFace.NegativeZ
        };

        // public void SetLightCookieFromBytes(Light lightObject, byte[] imageBytes)
        // {
        //     if (imageBytes.Length <= 0) return;
        //
        //     var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        //     tex.LoadImage(imageBytes);
        //
        //     // RenderTexture rt = new RenderTexture(tex.width, tex.height, 0)
        //     // {
        //     //     useMipMap = false, 
        //     //     filterMode = FilterMode.Trilinear, 
        //     //     wrapMode = TextureWrapMode.Clamp
        //     // };
        //     // RenderTexture.active = rt;
        //     // Graphics.Blit(tex, rt);
        //     
        //     lightObject.cookie = tex;
        // }
    }
}