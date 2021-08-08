using System;
using System.Linq;
using System.Reflection;
using AIChara;
using HarmonyLib;
using HooahSmugFace.IL_HooahSmugFace.Data;
using UnityEngine;

namespace HooahSmugFace.IL_HooahSmugFace
{
    public static class FaceOffset
    {
        private static Type _coroutineType;
        private static FieldInfo _headIDField;
        private static FieldInfo _thisField;

        public static void Init()
        {
            var instance = new Harmony(@"IL_SoNoHead");
            _coroutineType = typeof(ChaControl)
                .GetNestedTypes(AccessTools.all)
                .FirstOrDefault(x => x.GetFields().Length == 4 && x.Name.StartsWith("<ChangeHeadAsync>"));

            if (_coroutineType == null) return;

            _headIDField = _coroutineType.GetField("_headId");
            _thisField = _coroutineType.GetFields().FirstOrDefault(x => x.Name.Contains("__this"));
            var method = AccessTools.Method(_coroutineType, "MoveNext");
            instance.Patch(method, null, new HarmonyMethod(typeof(FaceOffset), nameof(RegisterBlendShapeToFace)));
        }

        public static void RegisterBlendShapeToFace(object __instance)
        {
            if (!(_thisField.GetValue(__instance) is ChaControl chaControl)) return;
            var id = (int) _headIDField.GetValue(__instance);
            if (!FaceData.TryGetData(id, out var list)) return;
            var face = chaControl.cmpFace;
            FaceData.CacheOriginalMesh(id, face);
            foreach (var faceData in list) faceData.Apply(face);
        }
    }
}
