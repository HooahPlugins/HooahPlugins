using System.IO;
using BepInEx;

namespace HooahSmugFace.IL_HooahSmugFace
{
    public static class Config
    {
        public static readonly string DefaultFaceOffsetPath =
            Path.Combine(Paths.GameRootPath, "UserData\\FaceOffset");
    }
}
