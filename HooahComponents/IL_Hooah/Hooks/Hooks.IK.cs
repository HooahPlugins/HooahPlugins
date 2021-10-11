#if AI || HS2
using HarmonyLib;
using UniRx;

#endif

namespace HooahComponents.Hooks
{
    public static class IK
    {
#if AI || HS2
        public static Subject<EyeLookController> controller;

        static IK()
        {
            controller = new Subject<EyeLookController>();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(EyeLookController), "LateUpdate")]
        public static void EyeUpdateCalc(EyeLookController __instance)
        {
            if (__instance != null && HijackLook._updatePositions.TryGetValue(__instance, out var p))
                __instance.eyeLookScript.EyeUpdateCalc(p, 1);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(NeckLookControllerVer2), "LateUpdate")]
        public static void NeckUpdateCalc(NeckLookControllerVer2 __instance)
        {
            if (__instance != null && HijackNeck._updatePositions.TryGetValue(__instance, out var p))
                __instance.neckLookScript.NeckUpdateCalc(p, 1);
        }
#endif
    }
}