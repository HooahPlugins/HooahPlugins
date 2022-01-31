using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace HooahUtility
{
    [BepInPlugin(Constant.GUID, Constant.NAME, Constant.VERSION)]
    public class HooahUtilityPlugin : BaseUnityPlugin
    {
        public static HooahUtilityPlugin Instance;
        public ManualLogSource Log => this.Logger;

        private void Start()
        {
            Instance = this;
            Harmony.CreateAndPatchAll(typeof(HooahUtilityPlugin));
        }
    }
}
