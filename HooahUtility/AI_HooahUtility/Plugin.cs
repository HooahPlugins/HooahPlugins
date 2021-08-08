using BepInEx;
using BepInEx.Configuration;
using BepInEx.Harmony;
using BepInEx.Logging;
using HarmonyLib;

namespace HooahUtility
{
    [BepInPlugin(Constant.GUID, Constant.NAME, Constant.VERSION)]
    public class HooahUtilityPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            Harmony.CreateAndPatchAll(typeof(HooahUtilityPlugin));
        }
    }
}